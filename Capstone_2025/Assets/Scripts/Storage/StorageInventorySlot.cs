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
        PlayerStoreBoxInventoryUIManager.Instance.OnItemSelected(itemName, itemSprite);
    }
}
