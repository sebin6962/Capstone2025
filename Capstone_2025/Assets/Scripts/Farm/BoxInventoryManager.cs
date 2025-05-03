using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BoxInventoryManager : MonoBehaviour
{
    public static BoxInventoryManager Instance;

    public GameObject inventoryPanel;
    public List<InventorySlot> slots;

    private GameObject heldItem;
    private Sprite heldSprite;
    private string heldItemName;

    private string savePath;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        savePath = Path.Combine(Application.persistentDataPath, "boxInventory.json");
        LoadInventory();
    }

    public bool IsHoldingTool(string toolName)
    {
        return heldItemName != null &&
               ToolData.Instance != null &&
               ToolData.Instance.IsTool(heldItemName) &&
               heldItemName == toolName;
    }

    public bool IsHoldingWateringCan()
    {
        return IsHoldingTool("wateringCan");
    }

    public bool IsHoldingItem()
    {
        return heldSprite != null;
    }

    public string GetHeldItemName()
    {
        return heldItemName;
    }

    public void RemoveHeldItem()
    {
        heldItem = null;
        heldSprite = null;
        heldItemName = null;
        HeldItemManager.Instance.HideHeldItem();
    }

    public void HoldItem(GameObject item)
    {
        heldItem = item;

        var spriteRenderer = item.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            heldSprite = spriteRenderer.sprite;
            heldItemName = heldSprite.name;

            HeldItemManager.Instance.ShowHeldItem(heldSprite);

            Debug.Log($"[DEBUG] 들고 있는 아이템 이름: {heldItemName}");
            Debug.Log($"[DEBUG] IsTool: {ToolData.Instance.IsTool(heldItemName)}");
            Debug.Log($"[DEBUG] IsHoldingWateringCan: {IsHoldingWateringCan()}");
        }

        Destroy(item);
    }

    public void PlaceHeldItemInSlot(InventorySlot clickedSlot = null)
    {
        Debug.Log("슬롯 클릭 - 아이템 넣기 시도");

        if (heldSprite == null)
        {
            Debug.LogWarning("들고 있는 스프라이트가 없음! 아이템이 null임");
            return;
        }

        // 도구는 저장 불가
        if (ToolData.Instance.IsTool(heldItemName))
        {
            Debug.Log("도구는 상자에 저장할 수 없습니다: " + heldItemName);
            return;
        }

        // 1. 동일 아이템 있는 슬롯 찾기
        foreach (var slot in slots)
        {
            if (slot.HasItem() && slot.GetItemName() == heldItemName)
            {
                int newCount = slot.GetItemCount() + 1;
                slot.SetItem(slot.GetSprite(), heldItemName, newCount);
                RemoveHeldItem();
                SaveInventory();
                return;
            }
        }

        // 2. 빈 슬롯 찾기
        foreach (var slot in slots)
        {
            if (!slot.HasItem())
            {
                slot.SetItem(heldSprite, heldItemName, 1);
                RemoveHeldItem();
                SaveInventory();
                return;
            }
        }

        Debug.Log("인벤토리에 빈 슬롯이 없습니다.");
    }

    public void PickUpFromSlot(InventorySlot slot)
    {
        if (!slot.HasItem()) return;

        heldSprite = slot.GetSprite();
        heldItemName = slot.GetItemName();
        heldItem = null;

        slot.ClearSlot();
        HeldItemManager.Instance.ShowHeldItem(heldSprite);

        Debug.Log("인벤토리에서 아이템 다시 듦: " + heldItemName);
        SaveInventory();
    }

    public void SaveInventory()
    {
        var data = new InventorySaveData();
        foreach (var slot in slots)
        {
            data.items.Add(new InventorySlotData
            {
                itemName = slot.HasItem() ? slot.GetItemName() : "",
                count = slot.HasItem() ? slot.GetItemCount() : 0
            });
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"[저장 완료] {savePath}");
    }

    public void LoadInventory()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("인벤토리 저장 파일 없음");
            return;
        }

        string json = File.ReadAllText(savePath);
        var data = JsonUtility.FromJson<InventorySaveData>(json);

        for (int i = 0; i < slots.Count && i < data.items.Count; i++)
        {
            var item = data.items[i];
            if (!string.IsNullOrEmpty(item.itemName) && item.count > 0)
            {
                Sprite sprite = Resources.Load<Sprite>("Sprites/" + item.itemName);
                slots[i].SetItem(sprite, item.itemName, item.count);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }

        Debug.Log("인벤토리 불러오기 완료");
    }

    public Sprite GetHeldSprite()
    {
        return heldSprite;
    }

    //수량 분리해서 들기 함수(한 번 클릭 시 아이템 하나만 들기)
    public void HoldItemFromSlot(Sprite sprite, string itemName)
    {
        heldSprite = sprite;
        heldItemName = itemName;
        heldItem = null;

        HeldItemManager.Instance.ShowHeldItem(heldSprite);

        Debug.Log($"[분리] 슬롯에서 {itemName} 1개만 손에 듬");
    }

    [System.Serializable]
    public class InventorySlotData
    {
        public string itemName;
        public int count;
    }

    [System.Serializable]
    public class InventorySaveData
    {
        public List<InventorySlotData> items = new();
    }
}
