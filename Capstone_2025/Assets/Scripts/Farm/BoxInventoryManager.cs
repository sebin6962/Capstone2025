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

    public bool IsHoldingWateringCan() //플레이어가 든 도구가 물뿌리개임을 확인
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
            HeldItemManager.Instance.ShowHeldItem(heldSprite); // UI 표시
            //Debug.Log("들고 있는 아이템: " + heldItemName);

            Debug.Log($"[DEBUG] 들고 있는 아이템 이름: {heldItemName}");
            Debug.Log($"[DEBUG] IsTool: {ToolData.Instance.IsTool(heldItemName)}");
            Debug.Log($"[DEBUG] IsHoldingWateringCan: {IsHoldingWateringCan()}");
        }

        //item.SetActive(false);
        Destroy(item);
    }

    public void PlaceHeldItemInSlot(InventorySlot slot)
    {
        Debug.Log("슬롯 클릭 - 아이템 넣기 시도");

        if (heldSprite == null)
        {

            Debug.LogWarning("들고 있는 스프라이트가 없음! 아이템이 null임");
            return;
        }

        // 도구(물뿌리개)는 슬롯에 넣을 수 없음
        if (ToolData.Instance.IsTool(heldItemName))
        {
            Debug.Log("도구는 상자에 저장할 수 없습니다: " + heldItemName);
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
