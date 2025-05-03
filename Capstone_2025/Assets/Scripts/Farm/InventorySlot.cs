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
            // 빈 슬롯 + 아이템 들고 있음 → 아이템을 슬롯에 넣는다
            BoxInventoryManager.Instance.PlaceHeldItemInSlot(this);

        }
        else if (slotHasItem && !isHolding)
        {
            // 슬롯에 아이템 있고 + 아무것도 안 들고 있음 → 아이템을 슬롯에서 다시 든다
            BoxInventoryManager.Instance.PickUpFromSlot(this);
        }
        else
        {
            Debug.Log($"슬롯 클릭됨: slotHasItem = {slotHasItem}, isHolding = {isHolding}");

        }
    }

    public void SetItem(Sprite sprite, string name = "")
    {
        if (sprite == null)
        {
            Debug.LogWarning("SetItem: null 스프라이트 전달됨!");
            return;
        }

        itemImage.sprite = sprite;
        itemImage.enabled = true;
        itemName = name != "" ? name : sprite.name;

        Debug.Log("슬롯에 아이템 설정됨: " + itemName);
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
