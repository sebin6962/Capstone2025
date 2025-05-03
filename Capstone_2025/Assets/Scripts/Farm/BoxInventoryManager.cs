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

            Debug.Log($"[DEBUG] ��� �ִ� ������ �̸�: {heldItemName}");
            Debug.Log($"[DEBUG] IsTool: {ToolData.Instance.IsTool(heldItemName)}");
            Debug.Log($"[DEBUG] IsHoldingWateringCan: {IsHoldingWateringCan()}");
        }

        Destroy(item);
    }

    public void PlaceHeldItemInSlot(InventorySlot clickedSlot = null)
    {
        Debug.Log("���� Ŭ�� - ������ �ֱ� �õ�");

        if (heldSprite == null)
        {
            Debug.LogWarning("��� �ִ� ��������Ʈ�� ����! �������� null��");
            return;
        }

        // ������ ���� �Ұ�
        if (ToolData.Instance.IsTool(heldItemName))
        {
            Debug.Log("������ ���ڿ� ������ �� �����ϴ�: " + heldItemName);
            return;
        }

        // 1. ���� ������ �ִ� ���� ã��
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

        // 2. �� ���� ã��
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

        Debug.Log("�κ��丮�� �� ������ �����ϴ�.");
    }

    public void PickUpFromSlot(InventorySlot slot)
    {
        if (!slot.HasItem()) return;

        heldSprite = slot.GetSprite();
        heldItemName = slot.GetItemName();
        heldItem = null;

        slot.ClearSlot();
        HeldItemManager.Instance.ShowHeldItem(heldSprite);

        Debug.Log("�κ��丮���� ������ �ٽ� ��: " + heldItemName);
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
        Debug.Log($"[���� �Ϸ�] {savePath}");
    }

    public void LoadInventory()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("�κ��丮 ���� ���� ����");
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

        Debug.Log("�κ��丮 �ҷ����� �Ϸ�");
    }

    public Sprite GetHeldSprite()
    {
        return heldSprite;
    }

    //���� �и��ؼ� ��� �Լ�(�� �� Ŭ�� �� ������ �ϳ��� ���)
    public void HoldItemFromSlot(Sprite sprite, string itemName)
    {
        heldSprite = sprite;
        heldItemName = itemName;
        heldItem = null;

        HeldItemManager.Instance.ShowHeldItem(heldSprite);

        Debug.Log($"[�и�] ���Կ��� {itemName} 1���� �տ� ��");
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
