using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxInventoryManager : MonoBehaviour
{
    public static BoxInventoryManager Instance;
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


    public bool IsHoldingTool(string toolName)
    {
        return heldItemName != null &&
               ToolData.Instance != null &&
               ToolData.Instance.IsTool(heldItemName) &&
               heldItemName == toolName;
    }

    public bool IsHoldingWateringCan() //�÷��̾ �� ������ ���Ѹ������� Ȯ��
    {
        return IsHoldingTool("wateringCan");
    }



    public void HoldItem(GameObject item)
    {
        heldItem = item;

        var spriteRenderer = item.GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            heldSprite = spriteRenderer.sprite;
            heldItemName = spriteRenderer.sprite.name;
            HeldItemManager.Instance.ShowHeldItem(heldSprite); // UI ǥ��
            //Debug.Log("��� �ִ� ������: " + heldItemName);

            Debug.Log($"[DEBUG] ��� �ִ� ������ �̸�: {heldItemName}");
            Debug.Log($"[DEBUG] IsTool: {ToolData.Instance.IsTool(heldItemName)}");
            Debug.Log($"[DEBUG] IsHoldingWateringCan: {IsHoldingWateringCan()}");
        }

        //item.SetActive(false);
        Destroy(item);
    }

    public void PlaceHeldItemInSlot(InventorySlot slot)
    {
        Debug.Log("���� Ŭ�� - ������ �ֱ� �õ�");

        if (heldSprite == null)
        {

            Debug.LogWarning("��� �ִ� ��������Ʈ�� ����! �������� null��");
            return;
        }

        // ����(���Ѹ���)�� ���Կ� ���� �� ����
        if (ToolData.Instance.IsTool(heldItemName))
        {
            Debug.Log("������ ���ڿ� ������ �� �����ϴ�: " + heldItemName);
            return;
        }

        slot.SetItem(heldSprite, heldItemName);

        heldItem = null;
        heldSprite = null;
        heldItemName = null;

        HeldItemManager.Instance.HideHeldItem(); // UI �����
        SaveInventory();
    }

    public void PickUpFromSlot(InventorySlot slot)
    {
        if (!slot.HasItem()) return;

        heldSprite = slot.GetSprite();
        heldItemName = slot.GetItemName();
        heldItem = null; // ���Կ��� �� ���̹Ƿ� GameObject�� null

        slot.ClearSlot();
        HeldItemManager.Instance.ShowHeldItem(heldSprite); // UI ǥ��

        Debug.Log("�κ��丮���� ������ �ٽ� ��: " + heldItemName);
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

    [System.Serializable]
    public class InventorySaveData
    {
        public List<string> items;
    }
}
