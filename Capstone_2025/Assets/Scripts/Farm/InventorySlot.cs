using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image itemImage;
    public Text countText;

    private string itemName;
    private int count;

    public void OnClick()
    {
        bool slotHasItem = HasItem();
        bool isHolding = BoxInventoryManager.Instance.IsHoldingItem();
        string heldName = BoxInventoryManager.Instance.GetHeldItemName();

        // [1] 손에 없음 → 슬롯에서 아이템 집기
        if (!isHolding && slotHasItem)
        {
            // 수량이 2 이상이면 분리 → 손에 1개만, 슬롯에 -1
            if (GetItemCount() > 1)
            {
                // 손에 1개 전달
                BoxInventoryManager.Instance.HoldItemFromSlot(GetSprite(), GetItemName());

                // 슬롯 수량 -1
                SetItem(GetSprite(), GetItemName(), GetItemCount() - 1);

                BoxInventoryManager.Instance.SaveInventory();
            }
            else
            {
                // 수량이 1이면 기존처럼 전부 들기
                BoxInventoryManager.Instance.PickUpFromSlot(this);
            }

            return;
        }

        // [2] 손에 아이템 있음
        if (isHolding)
        {
            // [2-1] 같은 아이템 → 수량 증가
            if (slotHasItem && heldName == GetItemName())
            {
                int newCount = GetItemCount() + 1;
                SetItem(GetSprite(), heldName, newCount);
                BoxInventoryManager.Instance.RemoveHeldItem();
                BoxInventoryManager.Instance.SaveInventory();
                return;
            }

            // [2-2] 빈 슬롯
            if (!slotHasItem)
            {
                // 인벤토리에 같은 아이템이 이미 있는지 확인
                bool existsInOtherSlot = false;

                foreach (var other in BoxInventoryManager.Instance.slots)
                {
                    if (other != this && other.HasItem() && other.GetItemName() == heldName)
                    {
                        existsInOtherSlot = true;
                        break;
                    }
                }

                // [2-2-1] 인벤토리에 없음 → 새로 저장
                if (!existsInOtherSlot)
                {
                    SetItem(BoxInventoryManager.Instance.GetHeldSprite(), heldName, 1);
                    BoxInventoryManager.Instance.RemoveHeldItem();
                    BoxInventoryManager.Instance.SaveInventory();
                }
                else
                {
                    Debug.Log("같은 아이템이 다른 슬롯에 있어 무시됨");
                }

                return;
            }

            // [2-3] 다른 아이템이 있는 슬롯 → 무시
            Debug.Log("다른 아이템이 있어 무시됨");
        }
    }

    public void SetItem(Sprite sprite, string name = "", int count = 1)
    {
        if (sprite == null)
        {
            Debug.LogWarning("SetItem: null 스프라이트 전달됨!");
            return;
        }

        itemImage.sprite = sprite;
        itemImage.enabled = true;
        itemName = string.IsNullOrEmpty(name) ? sprite.name : name;
        this.count = count;

        UpdateUI();

        Debug.Log($"슬롯에 아이템 설정됨: {itemName} x{count}");
    }

    public void ClearSlot()
    {
        itemImage.sprite = null;
        itemImage.enabled = false;
        itemName = "";
        count = 0;

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (countText == null) return;

        if (HasItem())
        {
            countText.enabled = true;
            countText.text = (count > 1) ? count.ToString() : "";
        }
        else
        {
            countText.text = "";
            countText.enabled = false; // 슬롯이 비었을 땐 숨김
        }
        //if (countText != null)
        //{
        //    countText.text = (count > 1) ? count.ToString() : "";
        //}
    }

    public bool HasItem()
    {
        return itemImage != null && itemImage.sprite != null;
    }

    public string GetItemName() => itemName;
    public int GetItemCount() => count;
    public Sprite GetSprite() => itemImage.sprite;
}
