using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagePlayerManager : MonoBehaviour
{
    private GameObject currentItem;

    public FarmManager farmManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (BoxInventoryManager.Instance.IsHoldingItem())
            {
                BoxInventoryManager.Instance.TryAutoStoreHeldItem();
            }
        }

        if (currentItem != null && Input.GetKeyDown(KeyCode.Space))
        {
            if (currentItem.name == "wateringCan")
                return;

            if (BoxInventoryManager.Instance.IsHoldingItem())
            {
                Debug.Log("�̹� �������̳� ������ ��� �־ ���ο� ���� �� �� �����ϴ�.");
                return;
            }

            if (ToolData.Instance.IsTool(currentItem.name))
                Debug.Log("���� ���: " + currentItem.name);
            else
                Debug.Log("������ ���: " + currentItem.name);

            BoxInventoryManager.Instance.HoldItem(currentItem);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (BoxInventoryManager.Instance.IsHoldingWateringCan() &&
                TryGetClickedFarmTile(out var pos1, out _))
            {
                farmManager.WaterSoil(pos1);
                Debug.Log("�� �Ѹ��� �Ϸ�");
            }
            else if (BoxInventoryManager.Instance.IsHoldingItem() &&
                     BoxInventoryManager.Instance.GetHeldItemName() == "seedBag" &&
                     TryGetClickedFarmTile(out var pos2, out _))
            {
                farmManager.PlantSeed(pos2);
                Debug.Log("���� �Ѹ��� �Ϸ�");
            }
            else
            {
                Debug.Log("�ʹ� �ְų� ���� �ƴմϴ�. ���̳� ������ ����� �� �����ϴ�.");
            }
        }
    }

    private bool TryGetClickedFarmTile(out Vector3 worldPos, out Vector3Int clickedCell)
    {
        worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;

        clickedCell = farmManager.fieldTilemap.WorldToCell(worldPos);
        Vector3Int playerCell = farmManager.fieldTilemap.WorldToCell(transform.position);

        int dx = Mathf.Abs(clickedCell.x - playerCell.x);
        int dy = Mathf.Abs(clickedCell.y - playerCell.y);

        return dx <= 1 && dy <= 1 && farmManager.IsFarmTile(worldPos);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            Debug.Log("������ ������: " + other.name);
            currentItem = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == currentItem)
        {
            currentItem = null;
        }
    }
}
