using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class BasketInventorySlot : MonoBehaviour
{
    public Image itemImage;
    public TextMeshProUGUI countText;

    [HideInInspector] public string itemName;
    [HideInInspector] public int slotIndex;
    [HideInInspector] public Sprite itemSprite;

    // 추가: 배경색 바꿀 Image
    public Image background;
    public Color selectedColor = new(1f, 0.9f, 0.6f); // 연한 노랑
    public Color defaultColor = Color.white;
    private bool isSelected = false;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void SetItem(string itemKey, Sprite sprite, int count)
    {
        itemName = itemKey;
        itemSprite = sprite;

        itemImage.sprite = sprite;
        itemImage.enabled = true;
        countText.text = count > 1 ? count.ToString() : "";
        countText.enabled = true;
    }

    public void Clear()
    {
        itemSprite = null;
        itemName = "";
        itemImage.sprite = null;
        itemImage.enabled = false;

        countText.text = "";
        countText.enabled = false;

        isSelected = false;
        UpdateBackground();
    }

    private void OnClick()
    {// 마우스 클릭이 아닌 다른 입력은 무시
        if (!Input.GetMouseButtonUp(0)) return;
        if (itemSprite == null || string.IsNullOrEmpty(itemName)) return;
        // 항상 호출: 제작기든 상자든 상황에 따라 처리
        if (BasketInventoryUIManager.Instance != null)
        {
            BasketInventoryUIManager.Instance.OnSlotClicked(slotIndex);
        }
    }

    private void UpdateBackground()
    {
        if (background != null)
            background.color = isSelected ? selectedColor : defaultColor;
    }
    public void Select()
    {
        isSelected = true;
        UpdateBackground();
    }

    public void Deselect()
    {
        isSelected = false;
        UpdateBackground();
    }

    public bool IsSelected()
    {
        return isSelected;
    }
}
