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
        // 플레이어가 근처에 있고, 물뿌리개를 들고 있고, 스페이스키를 눌렀다면
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.Space))
        {
            if (InventoryManager.Instance.IsHoldingWateringCan())
            {
                // 물뿌리개 내려놓기
                RestoreWateringCan();
                InventoryManager.Instance.RemoveHeldItem(); // 손에서 해제

                Debug.Log("물뿌리개를 다시 놓았습니다.");
            }
        }
    }

    public void RestoreWateringCan()
    {
        if (placedCan == null && wateringCanPrefab != null)
        {
            placedCan = Instantiate(wateringCanPrefab, transform.position, Quaternion.identity);
            // Z 위치 고정
            Vector3 pos = placedCan.transform.position;
            pos.z = 0f;
            placedCan.transform.position = pos;

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
