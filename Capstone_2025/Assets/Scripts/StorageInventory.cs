using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageInventory : MonoBehaviour
{
    public static StorageInventory Instance;

    private Dictionary<string, int> storage = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void AddItem(string itemName, int amount)
    {
        if (storage.ContainsKey(itemName))
            storage[itemName] += amount;
        else
            storage[itemName] = amount;

        Debug.Log($"[â��] {itemName} x{amount} �߰���. ����: {storage[itemName]}");
    }

    public int GetItemCount(string itemName)
    {
        return storage.TryGetValue(itemName, out var count) ? count : 0;
    }
}
