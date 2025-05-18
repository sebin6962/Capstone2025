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
                Debug.LogWarning($"[����UI] ��������Ʈ �ε� ����: {pair.Key}");
                continue;
            }

            slots[i].SetItem(pair.Key, sprite, pair.Value);
            i++;
        }
    }

    public void OnItemSelected(string itemName, Sprite sprite)
    {
        selectedItemName = itemName;

        // �ٱ��ϰ� ���� �ְ�, ������ �߰��� �����ߴٸ�
        if (BasketInventoryUIManager.Instance.IsOpen &&
            HeldItemManager.Instance.GetHeldItemName() == "basket" &&
            BasketInventoryUIManager.Instance.TryAddItemToBasket(itemName, sprite))
        {
            // ���� �κ��丮���� ������ 1�� ����
            currentInventory.AddItem(itemName, -1);
            Debug.Log($"[�̵�] {itemName} 1�� �� �ٱ��Ϸ� �̵�");

            UpdateSlots(); // ���� UI ����
            BasketInventoryUIManager.Instance.UpdateUI(); // �ٱ��� UI ����
        }
        else
        {
            Debug.LogWarning($"[�̵� ����] {itemName} �� �ٱ��Ϸ� �̵��� �� �����ϴ�.");
        }
    }

    //public void OnItemDeselected(string itemName)
    //{
    //    if (selectedItemName == itemName)
    //    {
    //        selectedItemName = null;
    //        Debug.Log($"[����UI] ���� ����: {itemName}");
    //    }
    //}
}
