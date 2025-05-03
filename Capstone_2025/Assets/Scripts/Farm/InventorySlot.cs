using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image itemImage;
    private string itemName;

    public void OnClick()
    {
        bool slotHasItem = HasItem();
        bool isHolding = BoxInventoryManager.Instance.IsHoldingItem();

        if (!slotHasItem && isHolding)
        {
            // �� ���� + ������ ��� ���� �� �������� ���Կ� �ִ´�
            BoxInventoryManager.Instance.PlaceHeldItemInSlot(this);

        }
        else if (slotHasItem && !isHolding)
        {
            // ���Կ� ������ �ְ� + �ƹ��͵� �� ��� ���� �� �������� ���Կ��� �ٽ� ���
            BoxInventoryManager.Instance.PickUpFromSlot(this);
        }
        else
        {
            Debug.Log($"���� Ŭ����: slotHasItem = {slotHasItem}, isHolding = {isHolding}");

        }
    }

    public void SetItem(Sprite sprite, string name = "")
    {
        if (sprite == null)
        {
            Debug.LogWarning("SetItem: null ��������Ʈ ���޵�!");
            return;
        }

        itemImage.sprite = sprite;
        itemImage.enabled = true;
        itemName = name != "" ? name : sprite.name;

        Debug.Log("���Կ� ������ ������: " + itemName);
    }

    public bool HasItem()
    {
        return itemImage != null && itemImage.sprite != null;
    }
    public string GetItemName() => itemName;

    public void ClearSlot()
    {
        itemImage.sprite = null;
        itemImage.enabled = false;
        itemName = "";
    }

    public Sprite GetSprite()
    {
        return itemImage.sprite;
    }
}
