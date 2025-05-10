using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PopupInventoryUIManager : MonoBehaviour
{
    public GameObject panel;
    public List<StorageInventorySlot> slots;

    // UI 텍스트들
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI actionButtonText;

    public GameObject closeButton;

    public void ShowPopup()
    {
        UpdateSlots();
        panel.SetActive(true);
        closeButton.gameObject.SetActive(true); 
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
}
