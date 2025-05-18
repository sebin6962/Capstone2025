using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public static PlayerInteract Instance;
    public KeyCode interactKey = KeyCode.E;

    private bool isNearMaker = false;
    public MakerInfo currentMaker;

    private BasketInventory nearbyBasket;

    private BasketInventory carriedBasket; // 현재 들고 있는 바구니의 실제 컴포넌트
    private GameObject currentBasketSpot;
    public GameObject basketPrefab;        // 바구니 프리팹 (Inspector에서 연결)

    private StorageInventory nearbyStorage;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            // 0. 바구니 인벤토리 열려 있으면 → 닫기
            if (BasketInventoryUIManager.Instance.IsOpen)
            {
                BasketInventoryUIManager.Instance.CloseBasketUI();

                Debug.Log("[E] 바구니 인벤토리 닫힘");
                return;
            }

            // *상자와 바구니가 동시에 열려 있을 때 → 모두 닫기
            if (PlayerStoreBoxInventoryUIManager.Instance.IsOpen() &&
                BasketInventoryUIManager.Instance.IsOpen)
            {
                PlayerStoreBoxInventoryUIManager.Instance.CloseUI();
                BasketInventoryUIManager.Instance.CloseBasketUI();
                Debug.Log("[E] 상자 + 바구니 UI 닫기");
                return;
            }

            //1.바구니 들기
            if (nearbyBasket != null && !HeldItemManager.Instance.IsHoldingItem())
            {
                Sprite sprite = nearbyBasket.GetComponent<SpriteRenderer>().sprite;
                HeldItemManager.Instance.ShowHeldItem(sprite, "basket");

                // 저장
                carriedBasket = nearbyBasket;
                carriedBasket.originalPosition = carriedBasket.transform.position;

                // 바구니 오브젝트 비활성화 (파괴 X)
                carriedBasket.gameObject.SetActive(false);
                nearbyBasket = null;

                Debug.Log("[E] 바구니 들기 (비활성화)");
                return;
            }

            // 2. 바구니 내려놓기 (Spot에 닿아있을 때)
            if (HeldItemManager.Instance.GetHeldItemName() == "basket" && carriedBasket != null && currentBasketSpot != null)
            {
                // 원래 바구니 오브젝트 다시 활성화
                carriedBasket.transform.position = currentBasketSpot.transform.position;
                carriedBasket.gameObject.SetActive(true);

                // 들고 있는 상태 해제
                HeldItemManager.Instance.HideHeldItem();
                carriedBasket = null;

                Debug.Log("[E] 바구니 내려놓기 (기존 오브젝트 복귀)");
                return;
            }

            // 2. 바구니 들고 제작기 근처에 있을 때
            if (HeldItemManager.Instance.GetHeldItemName() == "basket" && isNearMaker && currentMaker != null)
                {
                BasketInventory basket = carriedBasket;
                if (basket == null)
                {
                    Debug.LogWarning("carriedBasket이 null입니다.");
                    return;
                }

                    // 2-1. 결과물 있으면 수거
                    if (currentMaker.currentResultObject != null)
                    {
                        var resultSpriteRenderer = currentMaker.currentResultObject.GetComponent<SpriteRenderer>();
                        if (resultSpriteRenderer != null)
                        {
                            Sprite resultSprite = resultSpriteRenderer.sprite;
                            string resultName = resultSprite.name;

                            bool added = basket.AddItem(resultName, resultSprite);
                            if (added)
                            {
                                Destroy(currentMaker.currentResultObject);
                                currentMaker.currentResultObject = null;
                                Debug.Log($"[E] 제작기에서 결과물 {resultName} 수거 → 바구니에 저장됨");

                                // UI 업데이트 (이미 열려 있으면 반영됨)
                                BasketInventoryUIManager.Instance.UpdateUI();
                            }
                            else
                            {
                                Debug.Log("[E] 바구니가 가득 차서 결과물을 저장할 수 없습니다.");
                            }
                            return;
                        }
                    }

                    // 2-2. 결과물 없으면 인벤토리만 열기
                    BasketInventoryUIManager.Instance.OpenBasketUI(basket, HeldItemManager.Instance.player);
                    Debug.Log("[E] 바구니 인벤토리 열기");
                    return;

                }

            // 상자 UI 열려 있을 경우 → 닫기
            if (PlayerStoreBoxInventoryUIManager.Instance.IsOpen())
            {
                PlayerStoreBoxInventoryUIManager.Instance.CloseUI();
                return;
            }

            // 바구니 + 상자 동시에 열기
            if (HeldItemManager.Instance.GetHeldItemName() == "basket" && carriedBasket != null && nearbyStorage != null)
            {
                BasketInventoryUIManager.Instance.OpenBasketUI(carriedBasket, HeldItemManager.Instance.player);
                PlayerStoreBoxInventoryUIManager.Instance.OpenUI(nearbyStorage);
                Debug.Log("[E] 바구니+상자 UI 동시 열기");
                return;
            }

            // 상자 열기
            if (nearbyStorage != null)
            {
                PlayerStoreBoxInventoryUIManager.Instance.OpenUI(nearbyStorage);
                Debug.Log("[E] 상자 인벤토리 열기");
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isNearMaker && BasketInventoryUIManager.Instance.IsOpen)
            {
                if (currentMaker == null)
                {
                    Debug.LogWarning("[제작 실패] currentMaker가 null입니다!");
                    return;
                }

                Debug.Log($"[Space] 제작 시도 - makerId: {currentMaker.makerId}");
                BasketInventoryUIManager.Instance.StartCrafting(currentMaker);
                return;
            }
        }
    }

    public bool IsNearMaker()
    {
        return isNearMaker;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        var maker = other.GetComponent<MakerInfo>();
        if (maker != null)
        {
            currentMaker = maker;
            isNearMaker = true;
            Debug.Log($"접근: {currentMaker.makerId}");
        }

        if (other.CompareTag("Basket"))
        {
            nearbyBasket = other.GetComponent<BasketInventory>();
        }

        if (other.CompareTag("BasketSpot"))
        {
            currentBasketSpot = other.gameObject;
        }

        if (other.CompareTag("StorageBox")) // 꼭 Tag 설정 필요
        {
            nearbyStorage = other.GetComponent<StorageInventory>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var maker = other.GetComponent<MakerInfo>();
        if (maker != null && currentMaker == maker)
        {
            isNearMaker = false;
            currentMaker = null;
            Debug.Log($"이탈: {maker.makerId}");
        }

        if (other.CompareTag("Basket"))
        {
            if (nearbyBasket == other.GetComponent<BasketInventory>())
                nearbyBasket = null;
        }

        if (other.CompareTag("BasketSpot"))
        {
            if (currentBasketSpot == other.gameObject)
                currentBasketSpot = null;
        }

        if (other.CompareTag("StorageBox"))
        {
            if (nearbyStorage == other.GetComponent<StorageInventory>())
                nearbyStorage = null;
        }
    }
}
