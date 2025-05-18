using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStoreBoxInventoryUIManager : MonoBehaviour
{
    public static PlayerStoreBoxInventoryUIManager Instance;

    public GameObject panel;
    public List<StorageInventorySlot> slots;
    public Button closeButton;

    private StorageInventory currentInventory;

    private string selectedItemName = null;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseUI);
    }

    public void OpenUI(StorageInventory storage)
    {
        currentInventory = storage;
        panel.SetActive(true);
        selectedItemName = null;
        UpdateSlots();
    }

    public void CloseUI()
    {
        panel.SetActive(false);
        currentInventory = null;
        selectedItemName = null;
    }

    public bool IsOpen()
    {
        return panel.activeSelf;
    }

    public void UpdateSlots()
    {
        foreach (var slot in slots)
            slot.ClearSlot();

        int i = 0;
        foreach (var pair in currentInventory.GetAllItems())
        {
            if (i >= slots.Count) break;

            Sprite sprite = Resources.Load<Sprite>("Sprites/Ingredients/" + pair.Key);
            if (sprite == null)
            {
                Debug.LogWarning($"[상자UI] 스프라이트 로드 실패: {pair.Key}");
                continue;
            }

            slots[i].SetItem(pair.Key, sprite, pair.Value);
            i++;
        }
    }

    public void OnItemSelected(string itemName, Sprite sprite)
    {
        selectedItemName = itemName;

        // 바구니가 열려 있고, 아이템 추가에 성공했다면
        if (BasketInventoryUIManager.Instance.IsOpen &&
            HeldItemManager.Instance.GetHeldItemName() == "basket" &&
            BasketInventoryUIManager.Instance.TryAddItemToBasket(itemName, sprite))
        {
            // 상자 인벤토리에서 아이템 1개 차감
            currentInventory.AddItem(itemName, -1);
            Debug.Log($"[이동] {itemName} 1개 → 바구니로 이동");

            UpdateSlots(); // 상자 UI 갱신
            BasketInventoryUIManager.Instance.UpdateUI(); // 바구니 UI 갱신
        }
        else
        {
            Debug.LogWarning($"[이동 실패] {itemName} 을 바구니로 이동할 수 없습니다.");
        }
    }

    //public void OnItemDeselected(string itemName)
    //{
    //    if (selectedItemName == itemName)
    //    {
    //        selectedItemName = null;
    //        Debug.Log($"[상자UI] 선택 해제: {itemName}");
    //    }
    //}
}
