using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class StorageEntry
{
    public string name;
    public int amount;
}

[System.Serializable]
public class StorageData
{
    public List<StorageEntry> items = new();
}

public class StorageInventory : MonoBehaviour
{
    public static StorageInventory Instance;

    private Dictionary<string, int> storage = new();
    private string savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            savePath = Path.Combine(Application.persistentDataPath, "storage.json");
            LoadStorage();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(string itemName, int amount)
    {
        if (storage.ContainsKey(itemName))
            storage[itemName] += amount;
        else
            storage[itemName] = amount;

        Debug.Log($"[창고] {itemName} x{amount} 추가됨. 총합: {storage[itemName]}");

        SaveStorage();
    }

    public int GetItemCount(string itemName)
    {
        return storage.TryGetValue(itemName, out var count) ? count : 0;
    }

    public void SaveStorage()
    {
        var data = new StorageData();

        foreach (var pair in storage)
            data.items.Add(new StorageEntry { name = pair.Key, amount = pair.Value });

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);

        Debug.Log("창고 저장 완료");
    }

    public void LoadStorage()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            var data = JsonUtility.FromJson<StorageData>(json);

            storage.Clear();
            foreach (var entry in data.items)
                storage[entry.name] = entry.amount;

            Debug.Log("창고 불러오기 완료");
        }
        else
        {
            Debug.Log("저장된 창고 없음. 새로 시작합니다.");
        }
    }

    public Dictionary<string, int> GetAllItems()
    {
        return new Dictionary<string, int>(storage);
    }
}

