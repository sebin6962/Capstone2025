using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StorageInventoryUIManager : MonoBehaviour
{
    public GameObject panel;                     // â�� �г�
    public List<StorageInventorySlot> slots;     // �̸� ��ġ�� ���Ե�

    public void ToggleStorageUI()
    {
        // UI ��ư �ܿ��� �� �� ���� ���ǹ� �߰�
        if (!EventSystem.current.currentSelectedGameObject ||
            EventSystem.current.currentSelectedGameObject.GetComponent<Button>() == null)
            return;

        if (panel.activeSelf)
        {
            panel.SetActive(false);
        }
        else
        {
            UpdateSlots();
            panel.SetActive(true);
        }

        // â�� �� �� Ȯ�� ó��
        StorageAlertManager.Instance.OnStorageOpened();
    }

    public void UpdateSlots()
    {
        // ��� ���� �ʱ�ȭ
        foreach (var slot in slots)
            slot.ClearSlot();

        // â�� ������ ä���
        int i = 0;
        foreach (var pair in StorageInventory.Instance.GetAllItems())
        {
            if (i >= slots.Count) break;

            Sprite sprite = Resources.Load<Sprite>("Sprites/Ingredients/" + pair.Key);
            if (sprite == null)
            {
                Debug.LogWarning($"��������Ʈ �ε� ����: {pair.Key}");
                continue;
            }

            slots[i].SetItem(pair.Key, sprite, pair.Value);
            i++;
        }
    }
}
