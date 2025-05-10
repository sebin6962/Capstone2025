using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupInventoryUIManager : MonoBehaviour
{
    public GameObject panel;
    public List<StorageInventorySlot> slots;

    public static PopupInventoryUIManager Instance;

    public Transform floatingImageTarget; // 플레이어 머리 위에 따라갈 대상 (예: Player의 empty 자식 오브젝트)
    public RectTransform floatingItemContainer; // 그리드 컨테이너
    public RectTransform floatingItemImagePrefab; // 아이콘 프리팹 (Image + RectTransform)
    public int maxFloatingItems = 4;

    private List<RectTransform> floatingImages = new();        // 생성된 이미지들
    private List<string> selectedItemNames = new();             // 선택된 아이템 이름 추적

    // UI 텍스트들
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI actionButtonText;

    public GameObject closeButton;
    private void Awake()
    {
        Instance = this;
    }

    private void LateUpdate()
    {
        if (floatingItemContainer != null && floatingImageTarget != null)
        {
            floatingItemContainer.position = floatingImageTarget.position;
        }
    }

    public void ShowPopup()
    {
        UpdateSlots();
        panel.SetActive(true);
        closeButton.gameObject.SetActive(true); 
    }

    public bool IsPopupOpen()
    {
        return panel.activeSelf;
    }

    public void ShowPopupForCategory(string category, string title, string actionBtn)
    {
        Debug.Log($"[팝업] 카테고리 '{category}'에 맞는 아이템만 표시");

        foreach (var slot in slots)
            slot.ClearSlot();

        int i = 0;
        foreach (var pair in StorageInventory.Instance.GetAllItems())
        {
            if (!IngredientCategoryMap.categoryByItem.TryGetValue(pair.Key, out var itemCategory)) continue;
            if (itemCategory != category) continue;

            if (i >= slots.Count) break;

            Sprite sprite = Resources.Load<Sprite>("Sprites/" + pair.Key);
            if (sprite == null)
            {
                Debug.LogWarning($"[팝업] 스프라이트 로드 실패: {pair.Key}");
                continue;
            }

            slots[i].SetItem(sprite, pair.Value);
            i++;
        }

        // 텍스트 설정
        titleText.text = title;
        actionButtonText.text = actionBtn;

        panel.SetActive(true);
        closeButton.gameObject.SetActive(true);
    }

    public void HidePopup()
    {
        panel.SetActive(false);
        closeButton.gameObject.SetActive(false);

        // 생성된 아이콘 프리팹 전부 제거
        foreach (var image in floatingImages)
        {
            if (image != null)
                Destroy(image.gameObject);
        }

        floatingImages.Clear();
        selectedItemNames.Clear();
    }

    public void UpdateSlots()
    {
        foreach (var slot in slots)
            slot.ClearSlot();

        int i = 0;
        foreach (var pair in StorageInventory.Instance.GetAllItems())
        {
            if (i >= slots.Count) break;

            Sprite sprite = Resources.Load<Sprite>("Sprites/" + pair.Key);
            if (sprite == null)
            {
                Debug.LogWarning($"[팝업] 스프라이트 로드 실패: {pair.Key}");
                continue;
            }

            slots[i].SetItem(sprite, pair.Value);
            i++;
        }
    }


    public void OnItemSelected(string itemName, Sprite sprite)
    {
        if (selectedItemNames.Contains(itemName) || selectedItemNames.Count >= maxFloatingItems)
            return;

        selectedItemNames.Add(itemName);

        RectTransform newImage = Instantiate(floatingItemImagePrefab, floatingItemImagePrefab.parent);
        newImage.GetComponent<Image>().sprite = sprite;
        newImage.gameObject.SetActive(true);

        floatingImages.Add(newImage);
        Debug.Log($"[OnItemSelected] {itemName} 선택됨. floatingImages.Count = {floatingImages.Count}");
    }


    public void OnItemDeselected(string itemName)
    {
        int index = selectedItemNames.IndexOf(itemName);
        if (index < 0) return;

        Destroy(floatingImages[index].gameObject);
        floatingImages.RemoveAt(index);
        selectedItemNames.RemoveAt(index);
    }

}
