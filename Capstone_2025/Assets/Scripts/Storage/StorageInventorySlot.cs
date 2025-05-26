using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StorageInventorySlot : MonoBehaviour
{
    public Image itemImage;
    public TextMeshProUGUI countText;

    //�߰�: ������ ���� �����
    public string itemName;
    public Sprite itemSprite;

    public void SetItem(string itemKey, Sprite sprite, int count)
    {
        if (sprite == null)
        {
            ClearSlot(); // ��������Ʈ�� null�̸� ���� �ʱ�ȭ
            Debug.LogWarning("SetItem: ��������Ʈ�� null�Դϴ�. ���� ���.");
            return;
        }

        itemSprite = sprite;
        itemName = itemKey;
        itemImage.sprite = sprite;
        itemImage.enabled = true;

        countText.text = (count > 1) ? count.ToString() : "";
        countText.enabled = true;
    }



    public void ClearSlot()
    {
        itemImage.sprite = null;
        itemImage.enabled = false;
        countText.text = "";
        countText.enabled = false;
        itemName = "";
        itemSprite = null;
    }

    public void OnClick()
    {
        if (itemSprite == null || string.IsNullOrEmpty(itemName)) return;
        PlayerStoreBoxInventoryUIManager.Instance.OnItemSelected(itemName, itemSprite);
    }
}
