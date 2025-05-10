using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StorageInventorySlot : MonoBehaviour
{
    public Image itemImage;
    public TextMeshProUGUI countText;

    // �߰�: ���� �ٲ� Image
    public Image background;
    public Color selectedColor = new Color(1f, 0.9f, 0.6f); // ���� ���
    public Color defaultColor = Color.white;

    //�߰�: ������ ���� �����
    public string itemName;
    public Sprite itemSprite;

    private bool isSelected = false;

    public void SetItem(Sprite sprite, int count)
    {
        if (sprite == null)
        {
            ClearSlot(); // ��������Ʈ�� null�̸� ���� �ʱ�ȭ
            Debug.LogWarning("SetItem: ��������Ʈ�� null�Դϴ�. ���� ���.");
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
        // �������� ������ Ŭ�� ����
        if (itemSprite == null || string.IsNullOrEmpty(itemName))
        {
            Debug.LogWarning("[���� Ŭ��] �������� �����ϴ�. ���õ�.");
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
