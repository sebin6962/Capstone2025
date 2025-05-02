using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public float moveSpeed = 10f;
    Rigidbody2D rb;
    Animator animator;

    private Vector2 movement;
    private GameObject currentItem;

    //밭에 물 주는 변수
    public FarmManager farmManager;
    public bool isHoldingWateringCan = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // 인벤토리 열려 있으면 움직임 차단
        if (GameManager.Instance.inventoryPanel.activeSelf)
        {
            movement = Vector2.zero;
            return;
        }

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");//입력감지

        if (currentItem != null && Input.GetKeyDown(KeyCode.Space))
        {
            if (InventoryManager.Instance.IsHoldingItem())
            {
                Debug.Log("이미 아이템이나 도구를 들고 있어서 새로운 것을 들 수 없습니다.");
                return;
            }

            // 구별 로그
            if (ToolData.Instance.IsTool(currentItem.name))
                Debug.Log("도구 들기: " + currentItem.name);
            else
                Debug.Log("아이템 들기: " + currentItem.name);

            InventoryManager.Instance.HoldItem(currentItem);
        }

        //밭에 물주기
        if (Input.GetMouseButtonDown(0) && InventoryManager.Instance.IsHoldingWateringCan())
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;

            Vector3Int clickedCell = farmManager.fieldTilemap.WorldToCell(mouseWorldPos);
            Vector3Int playerCell = farmManager.fieldTilemap.WorldToCell(transform.position);

            int dx = Mathf.Abs(clickedCell.x - playerCell.x);
            int dy = Mathf.Abs(clickedCell.y - playerCell.y);

            if (dx <= 1 && dy <= 1) // 주변 한 칸 이내
            {
                if (farmManager.IsFarmTile(mouseWorldPos))
                {
                    farmManager.WaterSoil(mouseWorldPos);
                    Debug.Log("물 뿌리기 완료 (거리 허용됨)");
                }
                else
                {
                    Debug.Log("여기는 밭이 아닙니다.");
                }
            }
            else
            {
                Debug.Log("너무 멀어서 물을 줄 수 없습니다.");
            }
        }

    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
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
