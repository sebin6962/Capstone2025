using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StorageInventorySlot : MonoBehaviour
{
    public Image itemImage;
    public TextMeshProUGUI countText;

    //추가: 아이템 정보 저장용
    public string itemName;
    public Sprite itemSprite;

    public void SetItem(string itemKey, Sprite sprite, int count)
    {
        if (sprite == null)
        {
            ClearSlot(); // 스프라이트가 null이면 슬롯 초기화
            Debug.LogWarning("SetItem: 스프라이트가 null입니다. 슬롯 비움.");
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

        // 바구니에 아이템 추가 시도
        if (HeldItemManager.Instance.GetHeldItemName() == "basket" &&
            BasketInventoryUIManager.Instance.IsOpen &&
            BasketInventoryUIManager.Instance.TryAddItemToBasket(itemName, itemSprite))
        {
            StorageInventory.Instance.AddItem(itemName, -1); // 상자에서 제거

            // 수량이 0이 되었는지 다시 검사
            if (!StorageInventory.Instance.HasItem(itemName))
            {
                ClearSlot(); // 슬롯도 비움
            }

            Debug.Log($"[이동] {itemName} 1개 → 바구니로 이동");

            PlayerStoreBoxInventoryUIManager.Instance.UpdateSlots(); // UI 갱신
        }
    }
}
