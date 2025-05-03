using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WateringCanAnchor : MonoBehaviour
{
    public GameObject wateringCanPrefab; // Prefab ����
    private GameObject placedCan;
    private bool isPlayerNearby = false;

    void Start()
    {
        if (wateringCanPrefab != null)
        {
            placedCan = Instantiate(wateringCanPrefab, transform.position, Quaternion.identity);
            Vector3 fixedPos = placedCan.transform.position;
            fixedPos.z = 0f;
            placedCan.transform.position = fixedPos;

            
        }
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.Space))
        {
            if (!BoxInventoryManager.Instance.IsHoldingItem())
            {
                BoxInventoryManager.Instance.HoldItem(wateringCanPrefab);
                Destroy(placedCan);
                placedCan = null;
                Debug.Log("���Ѹ����� ������ϴ�.");
            }
        }

        if (isPlayerNearby && Input.GetKeyDown(KeyCode.R))
        {
            if (BoxInventoryManager.Instance.IsHoldingWateringCan())
            {
                RestoreWateringCan();
                BoxInventoryManager.Instance.RemoveHeldItem();
                Debug.Log("���Ѹ����� �ٽ� ���ҽ��ϴ�.");
            }
        }
    }

    public void RestoreWateringCan()
    {
        if (placedCan == null && wateringCanPrefab != null)
        {
            placedCan = Instantiate(wateringCanPrefab, transform.position, Quaternion.identity);
            // Z ��ġ ����
            Vector3 fixedPos = placedCan.transform.position;
            fixedPos.z = 0f;
            placedCan.transform.position = fixedPos;

            Debug.Log("���Ѹ��� ���� ��ġ: " + transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerNearby = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerNearby = false;
    }
}
