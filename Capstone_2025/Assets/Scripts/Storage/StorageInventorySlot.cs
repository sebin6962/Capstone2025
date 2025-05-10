using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StorageInventorySlot : MonoBehaviour
{
    public Image itemImage;
    public TextMeshProUGUI countText;

    // 추가: 배경색 바꿀 Image
    public Image background;
    public Color selectedColor = new Color(1f, 0.9f, 0.6f); // 연한 노랑
    public Color defaultColor = Color.white;

    //추가: 아이템 정보 저장용
    public string itemName;
    public Sprite itemSprite;

    private bool isSelected = false;

    public void SetItem(Sprite sprite, int count)
    {
        if (sprite == null)
        {
            ClearSlot(); // 스프라이트가 null이면 슬롯 초기화
            Debug.LogWarning("SetItem: 스프라이트가 null입니다. 슬롯 비움.");
            return;
        }

        itemSprite = sprite;
        itemName = sprite.name;
        itemImage.sprite = sprite;
        itemImage.enabled = true;

        countText.text = (count > 1) ? count.ToString() : "";
        countText.enabled = true;

        isSelected = false;
        UpdateBackground();
    }



    public void ClearSlot()
    {
        itemSprite = null;
        itemImage.sprite = null;
        itemImage.enabled = false;

        countText.text = "";
        countText.enabled = false;

        isSelected = false;
        UpdateBackground();
    }

    public void OnClick()
    {
        // 아이템이 없으면 클릭 무시
        if (itemSprite == null || string.IsNullOrEmpty(itemName))
        {
            Debug.LogWarning("[슬롯 클릭] 아이템이 없습니다. 무시됨.");
            return;
        }

        isSelected = !isSelected;
        UpdateBackground();

        if (isSelected)
        {
            PopupInventoryUIManager.Instance.OnItemSelected(itemName, itemSprite);
        }
        else
        {
            PopupInventoryUIManager.Instance.OnItemDeselected(itemName);
        }
    }

    private void UpdateBackground()
    {
        if (background != null)
            background.color = isSelected ? selectedColor : defaultColor;
    }
}
