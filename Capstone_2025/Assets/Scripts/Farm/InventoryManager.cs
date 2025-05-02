using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    //public List<Item> items = new List<Item>();
    public GameObject inventoryPanel;
    public List<InventorySlot> slots;
    private GameObject heldItem;

    private Sprite heldSprite;
    private string heldItemName;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void HoldItem(GameObject item)
    {
        heldItem = item;

        var spriteRenderer = item.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            heldSprite = spriteRenderer.sprite;
            heldItemName = spriteRenderer.sprite.name;
            HeldItemManager.Instance.ShowHeldItem(heldSprite); // UI 표시
            Debug.Log("들고 있는 아이템: " + heldItemName);
        }

        item.SetActive(false);
    }

    public void PlaceHeldItemInSlot(InventorySlot slot)
    {
        Debug.Log("슬롯 클릭 - 아이템 넣기 시도");

        if (heldSprite == null)
        {

            Debug.LogWarning("들고 있는 스프라이트가 없음! 아이템이 null임");
            return;
        }

        slot.SetItem(heldSprite, heldItemName);

        heldItem = null;
        heldSprite = null;
        heldItemName = null;

        HeldItemManager.Instance.HideHeldItem(); // UI 숨기기
        SaveInventory();
    }

    public void PickUpFromSlot(InventorySlot slot)
    {
        if (!slot.HasItem()) return;

        heldSprite = slot.GetSprite();
        heldItemName = slot.GetItemName();
        heldItem = null; // 슬롯에서 온 것이므로 GameObject는 null

        slot.ClearSlot();
        HeldItemManager.Instance.ShowHeldItem(heldSprite); // UI 표시

        Debug.Log("인벤토리에서 아이템 다시 듦: " + heldItemName);
    }

    public void SaveInventory()
    {
        List<string> spriteNames = new List<string>();
        foreach (var slot in slots)
        {
            spriteNames.Add(slot.HasItem() ? slot.GetItemName() : "");
        }

        string json = JsonUtility.ToJson(new InventorySaveData { items = spriteNames });
        PlayerPrefs.SetString("Inventory", json);
    }

    public void LoadInventory()
    {
        string json = PlayerPrefs.GetString("Inventory", "");
        if (!string.IsNullOrEmpty(json))
        {
            InventorySaveData data = JsonUtility.FromJson<InventorySaveData>(json);
            for (int i = 0; i < slots.Count; i++)
            {
                if (!string.IsNullOrEmpty(data.items[i]))
                {
                    Sprite itemSprite = Resources.Load<Sprite>("Sprites/" + data.items[i]);
                    slots[i].SetItem(itemSprite, data.items[i]);
                }
            }
        }
    }

    public bool IsHoldingItem()
    {
        return heldSprite != null;
    }

    [System.Serializable]
    public class InventorySaveData
    {
        public List<string> items;
    }
}
