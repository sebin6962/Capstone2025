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

    // �߰�: ���� �ٲ� Image
    public Image background;
    public Color selectedColor = new(1f, 0.9f, 0.6f); // ���� ���
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
    }

    private void OnClick()
    {
        //if (BasketInventoryUIManager.Instance != null &&
        //BasketInventoryUIManager.Instance.CanSelectSlot())
        //{
        //    isSelected = !isSelected;
        //    UpdateBackground();

        //    if (isSelected)
        //        BasketInventoryUIManager.Instance.OnItemSelected(itemName, slotIndex);
        //    else
        //        BasketInventoryUIManager.Instance.OnItemDeselected(itemName, slotIndex);
        //}
        // �׻� ȣ��: ���۱�� ���ڵ� ��Ȳ�� ���� ó��
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
}
