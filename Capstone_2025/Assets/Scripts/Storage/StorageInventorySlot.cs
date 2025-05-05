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
            Debug.LogWarning("SetItem: ��������Ʈ�� null�Դϴ�!");
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
