using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WateringCanAnchor : MonoBehaviour
{
    public GameObject wateringCanPrefab; // Prefab 연결
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
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (!BoxInventoryManager.Instance.IsHoldingItem())
            {
                GameObject canInstance = Instantiate(wateringCanPrefab); // 복제본 생성
                BoxInventoryManager.Instance.HoldItem(canInstance);
                Destroy(placedCan);
                placedCan = null;
                Debug.Log("물뿌리개를 들었습니다.");
            }
        }

        if (isPlayerNearby && Input.GetKeyDown(KeyCode.Space))
        {
            if (BoxInventoryManager.Instance.IsHoldingItem() &&
        BoxInventoryManager.Instance.IsHoldingWateringCan())
            {
                RestoreWateringCan();
                BoxInventoryManager.Instance.RemoveHeldItem();
                Debug.Log("물뿌리개를 다시 놓았습니다.");
            }
            else
            {
                Debug.Log("물뿌리개를 들고 있지 않음");
            }

            //수정 필요
            //if (BoxInventoryManager.Instance.IsHoldingWateringCan())
            //{
            //    RestoreWateringCan();
            //    BoxInventoryManager.Instance.RemoveHeldItem();
            //    Debug.Log("물뿌리개를 다시 놓았습니다.");
            //}
        }
    }

    public void RestoreWateringCan()
    {
        if (placedCan == null && wateringCanPrefab != null)
        {
            placedCan = Instantiate(wateringCanPrefab, transform.position, Quaternion.identity);
            // Z 위치 고정
            Vector3 fixedPos = placedCan.transform.position;
            fixedPos.z = 0f;
            placedCan.transform.position = fixedPos;

            Debug.Log("물뿌리개 생성 위치: " + transform.position);
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
