using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageInventoryUIManager : MonoBehaviour
{
    public GameObject panel;                     // 창고 패널
    public List<StorageInventorySlot> slots;     // 미리 배치된 슬롯들

    public void ToggleStorageUI()
    {
        if (panel.activeSelf)
        {
            panel.SetActive(false);
        }
        else
        {
            UpdateSlots();
            panel.SetActive(true);
        }

        // 창고 열 때 확인 처리
        StorageAlertManager.Instance.OnStorageOpened();
    }

    public void UpdateSlots()
    {
        // 모든 슬롯 초기화
        foreach (var slot in slots)
            slot.ClearSlot();

        // 창고 데이터 채우기
        int i = 0;
        foreach (var pair in StorageInventory.Instance.GetAllItems())
        {
            if (i >= slots.Count) break;

            Sprite sprite = Resources.Load<Sprite>("Sprites/" + pair.Key);
            if (sprite == null)
            {
                Debug.LogWarning($"스프라이트 로드 실패: {pair.Key}");
                continue;
            }

            slots[i].SetItem(sprite, pair.Value);
            i++;
        }
    }
}
