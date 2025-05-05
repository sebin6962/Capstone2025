using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageInventorySlot : MonoBehaviour
{
    public Image itemImage;
    public Text countText;

    public void SetItem(Sprite sprite, int count)
    {
        if (sprite == null)
        {
            Debug.LogWarning("SetItem: 스프라이트가 null입니다!");
            return;
        }
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
    }
}
