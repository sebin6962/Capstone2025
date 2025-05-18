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

        isSelected = false;
        UpdateBackground();
    }

    private void OnClick()
    {// ���콺 Ŭ���� �ƴ� �ٸ� �Է��� ����
        if (!Input.GetMouseButtonUp(0)) return;
        if (itemSprite == null || string.IsNullOrEmpty(itemName)) return;
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

    public bool IsSelected()
    {
        return isSelected;
    }
}
