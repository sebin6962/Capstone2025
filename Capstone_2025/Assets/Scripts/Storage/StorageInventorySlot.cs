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

        // �ٱ��Ͽ� ������ �߰� �õ�
        if (HeldItemManager.Instance.GetHeldItemName() == "basket" &&
            BasketInventoryUIManager.Instance.IsOpen &&
            BasketInventoryUIManager.Instance.TryAddItemToBasket(itemName, itemSprite))
        {
            StorageInventory.Instance.AddItem(itemName, -1); // ���ڿ��� ����

            // ������ 0�� �Ǿ����� �ٽ� �˻�
            if (!StorageInventory.Instance.HasItem(itemName))
            {
                ClearSlot(); // ���Ե� ���
            }

            Debug.Log($"[�̵�] {itemName} 1�� �� �ٱ��Ϸ� �̵�");

            PlayerStoreBoxInventoryUIManager.Instance.UpdateSlots(); // UI ����
        }
    }
}
