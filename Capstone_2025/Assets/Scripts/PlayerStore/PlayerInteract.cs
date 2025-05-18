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

    private BasketInventory carriedBasket; // ���� ��� �ִ� �ٱ����� ���� ������Ʈ
    private GameObject currentBasketSpot;
    public GameObject basketPrefab;        // �ٱ��� ������ (Inspector���� ����)

    private StorageInventory nearbyStorage;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            // 0. �ٱ��� �κ��丮 ���� ������ �� �ݱ�
            if (BasketInventoryUIManager.Instance.IsOpen)
            {
                BasketInventoryUIManager.Instance.CloseBasketUI();

                Debug.Log("[E] �ٱ��� �κ��丮 ����");
                return;
            }

            // *���ڿ� �ٱ��ϰ� ���ÿ� ���� ���� �� �� ��� �ݱ�
            if (PlayerStoreBoxInventoryUIManager.Instance.IsOpen() &&
                BasketInventoryUIManager.Instance.IsOpen)
            {
                PlayerStoreBoxInventoryUIManager.Instance.CloseUI();
                BasketInventoryUIManager.Instance.CloseBasketUI();
                Debug.Log("[E] ���� + �ٱ��� UI �ݱ�");
                return;
            }

            //1.�ٱ��� ���
            if (nearbyBasket != null && !HeldItemManager.Instance.IsHoldingItem())
            {
                Sprite sprite = nearbyBasket.GetComponent<SpriteRenderer>().sprite;
                HeldItemManager.Instance.ShowHeldItem(sprite, "basket");

                // ����
                carriedBasket = nearbyBasket;
                carriedBasket.originalPosition = carriedBasket.transform.position;

                // �ٱ��� ������Ʈ ��Ȱ��ȭ (�ı� X)
                carriedBasket.gameObject.SetActive(false);
                nearbyBasket = null;

                Debug.Log("[E] �ٱ��� ��� (��Ȱ��ȭ)");
                return;
            }

            // 2. �ٱ��� �������� (Spot�� ������� ��)
            if (HeldItemManager.Instance.GetHeldItemName() == "basket" && carriedBasket != null && currentBasketSpot != null)
            {
                // ���� �ٱ��� ������Ʈ �ٽ� Ȱ��ȭ
                carriedBasket.transform.position = currentBasketSpot.transform.position;
                carriedBasket.gameObject.SetActive(true);

                // ��� �ִ� ���� ����
                HeldItemManager.Instance.HideHeldItem();
                carriedBasket = null;

                Debug.Log("[E] �ٱ��� �������� (���� ������Ʈ ����)");
                return;
            }

            // 2. �ٱ��� ��� ���۱� ��ó�� ���� ��
            if (HeldItemManager.Instance.GetHeldItemName() == "basket" && isNearMaker && currentMaker != null)
                {
                BasketInventory basket = carriedBasket;
                if (basket == null)
                {
                    Debug.LogWarning("carriedBasket�� null�Դϴ�.");
                    return;
                }

                    // 2-1. ����� ������ ����
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
                                Debug.Log($"[E] ���۱⿡�� ����� {resultName} ���� �� �ٱ��Ͽ� �����");

                                // UI ������Ʈ (�̹� ���� ������ �ݿ���)
                                BasketInventoryUIManager.Instance.UpdateUI();
                            }
                            else
                            {
                                Debug.Log("[E] �ٱ��ϰ� ���� ���� ������� ������ �� �����ϴ�.");
                            }
                            return;
                        }
                    }

                    // 2-2. ����� ������ �κ��丮�� ����
                    BasketInventoryUIManager.Instance.OpenBasketUI(basket, HeldItemManager.Instance.player);
                    Debug.Log("[E] �ٱ��� �κ��丮 ����");
                    return;

                }

            // ���� UI ���� ���� ��� �� �ݱ�
            if (PlayerStoreBoxInventoryUIManager.Instance.IsOpen())
            {
                PlayerStoreBoxInventoryUIManager.Instance.CloseUI();
                return;
            }

            // �ٱ��� + ���� ���ÿ� ����
            if (HeldItemManager.Instance.GetHeldItemName() == "basket" && carriedBasket != null && nearbyStorage != null)
            {
                BasketInventoryUIManager.Instance.OpenBasketUI(carriedBasket, HeldItemManager.Instance.player);
                PlayerStoreBoxInventoryUIManager.Instance.OpenUI(nearbyStorage);
                Debug.Log("[E] �ٱ���+���� UI ���� ����");
                return;
            }

            // ���� ����
            if (nearbyStorage != null)
            {
                PlayerStoreBoxInventoryUIManager.Instance.OpenUI(nearbyStorage);
                Debug.Log("[E] ���� �κ��丮 ����");
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isNearMaker && BasketInventoryUIManager.Instance.IsOpen)
            {
                if (currentMaker == null)
                {
                    Debug.LogWarning("[���� ����] currentMaker�� null�Դϴ�!");
                    return;
                }

                Debug.Log($"[Space] ���� �õ� - makerId: {currentMaker.makerId}");
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
            Debug.Log($"����: {currentMaker.makerId}");
        }

        if (other.CompareTag("Basket"))
        {
            nearbyBasket = other.GetComponent<BasketInventory>();
        }

        if (other.CompareTag("BasketSpot"))
        {
            currentBasketSpot = other.gameObject;
        }

        if (other.CompareTag("StorageBox")) // �� Tag ���� �ʿ�
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
            Debug.Log($"��Ż: {maker.makerId}");
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
