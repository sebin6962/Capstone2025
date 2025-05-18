using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BasketInventoryUIManager : MonoBehaviour
{
    public static BasketInventoryUIManager Instance;

    public GameObject basketPanel;
    public RectTransform panelTransform;
    public List<BasketInventorySlot> slots;
    public Vector3 offset = new(0, 2.5f, 0);

    private BasketInventory currentBasket;
    private Transform player;

    public bool IsOpen { get; private set; } = false;

    private HashSet<string> selectedItemNames = new();

    private List<int> selectedSlotIndices = new();

    [Header("제작 관련")]
    public RectTransform progressBarPrefab;
    public GameObject resultItemPrefab;
    public Transform worldCanvasParent;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void OpenBasketUI(BasketInventory basket, Transform playerTransform)
    {
        currentBasket = basket;
        player = playerTransform;

        // 로드
        currentBasket.Load();

        // UI 갱신
        UpdateUI();

        basketPanel.SetActive(true);
        IsOpen = true;
    }

    public void CloseBasketUI()
    {
        basketPanel.SetActive(false);
        IsOpen = false;
    }

    private void LateUpdate()
    {
        if (IsOpen && player != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(player.position + offset);
            panelTransform.position = screenPos;
        }
    }

    public void UpdateUI()
    {
        Dictionary<string, int> itemCounts = new();
        Dictionary<string, Sprite> itemSprites = new();

        for (int i = 0; i < currentBasket.itemNames.Count; i++)
        {
            string name = currentBasket.itemNames[i];
            Sprite sprite = currentBasket.itemSprites[i];

            if (!itemCounts.ContainsKey(name))
            {
                itemCounts[name] = 1;
                itemSprites[name] = sprite;
            }
            else
            {
                itemCounts[name]++;
            }
        }

        int index = 0;
        foreach (var pair in itemCounts)
        {
            if (index >= slots.Count) break;
            slots[index].SetItem(pair.Key, itemSprites[pair.Key], pair.Value);
            slots[index].slotIndex = index;
            index++;
        }

        for (int i = index; i < slots.Count; i++)
        {
            slots[i].Clear();
        }
    }

    public void OnSlotClicked(int index)
    {
        if (index >= slots.Count) return;

        //string itemName = slots[index].itemName;
        var slot = slots[index];

        if (string.IsNullOrEmpty(slot.itemName))
        {
            Debug.LogWarning("[슬롯 선택 오류] 유효하지 않은 빈 슬롯이 선택되었습니다.");
            return;
        }
        string itemName = slot.itemName;

        if (PlayerStoreBoxInventoryUIManager.Instance.IsOpen())
        {
            int removeIndex = currentBasket.itemNames.IndexOf(itemName);
            if (removeIndex != -1)
            {
                currentBasket.itemNames.RemoveAt(removeIndex);
                currentBasket.itemSprites.RemoveAt(removeIndex);
                StorageInventory.Instance.AddItem(itemName, 1);
                currentBasket.Save();
                UpdateUI();
                PlayerStoreBoxInventoryUIManager.Instance.UpdateSlots();
            }
            return;
        }

        if (PlayerInteract.Instance.IsNearMaker())
        {
            if (selectedSlotIndices.Contains(index))
            {
                selectedSlotIndices.Remove(index);
                slot.Deselect();
                Debug.Log($"[선택 해제] {itemName}");
            }
            else
            {
                selectedSlotIndices.Add(index);
                slot.Select();
                Debug.Log($"[선택] {itemName}");
            }
        }

    }

    public void StartCrafting(MakerInfo maker)
    {
        // 선택된 아이템 이름 리스트 생성
        List<string> selectedItemNames = new();
        foreach (int idx in selectedSlotIndices)
        {
            selectedItemNames.Add(slots[idx].itemName);

            if (!string.IsNullOrEmpty(name))
                selectedItemNames.Add(name);
        }

        if (selectedSlotIndices.Count == 0)
        {
            Debug.LogWarning("[제작 실패] 선택된 아이템 없음");
            return;
        }

        // 슬롯에서 이름 및 스프라이트 추출
        int selectedSlotIndex = selectedSlotIndices[0];
        string resultItemName = slots[selectedSlotIndex].itemName;
        Sprite resultSprite = slots[selectedSlotIndex].itemSprite;

        // 유효한 아이템인지 검사
        if (string.IsNullOrEmpty(resultItemName))
        {
            Debug.LogWarning("[제작 실패] 유효하지 않은 아이템입니다");
            return;
        }

        // 재료 제거
        foreach (int idx in selectedSlotIndices)
        {
            string name = slots[idx].itemName;
            int removeIndex = currentBasket.itemNames.IndexOf(name);
            if (removeIndex != -1)
            {
                currentBasket.itemNames.RemoveAt(removeIndex);
                currentBasket.itemSprites.RemoveAt(removeIndex);
            }
            currentBasket.Save();
            selectedSlotIndices.Clear();
            UpdateUI();
            CloseBasketUI();

            StartCoroutine(ShowProgressAndSpawnItem(maker, selectedItemNames));
        }
    }

    private IEnumerator ShowProgressAndSpawnItem(MakerInfo maker, List<string> selectedItemNames)
    {
        Transform makerTransform = maker.transform;

        RectTransform progressBar = Instantiate(progressBarPrefab, worldCanvasParent);
        Vector3 worldPos = makerTransform.position + new Vector3(0f, 1.2f, 0f);
        progressBar.position = worldPos;

        if (Camera.main != null)
            progressBar.position += Camera.main.transform.forward * 0.01f;

        Transform fill = progressBar.transform.Find("Fill");
        if (fill == null)
        {
            Debug.LogError("프리팹 안에 'Fill' 오브젝트가 없습니다!");
            yield break;
        }

        Image fillImage = fill.GetComponent<Image>();
        fillImage.fillAmount = 0f;

        float duration = 3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fillImage.fillAmount = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        Vector3 progressBarPos = progressBar.position;
        Destroy(progressBar.gameObject);

        Sprite resultSprite = CraftingRecipeManager.Instance.GetResultSprite(maker.makerId, selectedItemNames);

        if (resultSprite == null)
        {
            Debug.LogWarning("레시피 일치 실패 또는 스프라이트 로드 실패 → 기본결과 사용");
            resultSprite = Resources.Load<Sprite>("Sprites/기본결과");
        }

        GameObject result = Instantiate(resultItemPrefab, progressBarPos, Quaternion.identity);
        SpriteRenderer sr = result.GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            sr.sprite = resultSprite;
        }
        else
        {
            Debug.LogError("[제작 오류] SpriteRenderer가 프리팹에 없습니다!");
        }

        maker.currentResultObject = result;

        ResultItem ri = result.AddComponent<ResultItem>();
        ri.itemName = resultSprite.name;

        Debug.Log($"[제작 완료] {resultSprite.name} 생성됨");
    }

    public bool TryAddItemToBasket(string itemName, Sprite sprite)
    {
        if (currentBasket == null) return false;
        bool added = currentBasket.AddItem(itemName, sprite);
        if (added) UpdateUI(); // 추가 성공 시 UI 갱신
        return added;
    }

    public void SetPlayer(Transform playerTransform)
    {
        this.player = playerTransform;
    }

    public bool CanSelectSlot()
    {
        return PlayerInteract.Instance.IsNearMaker(); // 제작기와 닿은 상태만
    }

    public void OnItemSelected(string itemName, int slotIndex)
    {
        if (!selectedSlotIndices.Contains(slotIndex))
            selectedSlotIndices.Add(slotIndex);
    }

    public void OnItemDeselected(string itemName, int slotIndex)
    {
        selectedSlotIndices.Remove(slotIndex);
    }
}
