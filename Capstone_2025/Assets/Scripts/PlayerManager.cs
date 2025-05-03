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
    //public bool isHoldingWateringCan = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsInventoryOpen())
        {
            movement = Vector2.zero;
            //return; // 움직임 및 입력 차단
        }
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");//입력감지

        if (currentItem != null && Input.GetKeyDown(KeyCode.Space))
        {
            // 물뿌리개는 WateringCanAnchor에서만 다루기
            if (currentItem.name == "wateringCan")
                return;

            if (BoxInventoryManager.Instance.IsHoldingItem())
            {
                Debug.Log("이미 아이템이나 도구를 들고 있어서 새로운 것을 들 수 없습니다.");
                return;
            }

            // 구별 로그
            if (ToolData.Instance.IsTool(currentItem.name))
                Debug.Log("도구 들기: " + currentItem.name);
            else
                Debug.Log("아이템 들기: " + currentItem.name);

            BoxInventoryManager.Instance.HoldItem(currentItem);
        }

        //밭에 물주기
        if (Input.GetMouseButtonDown(0))
        {
            if (BoxInventoryManager.Instance.IsHoldingWateringCan() &&
                TryGetClickedFarmTile(out var pos1, out _))
            {
                farmManager.WaterSoil(pos1);
                Debug.Log("물 뿌리기 완료");
            }
            else if (BoxInventoryManager.Instance.IsHoldingItem() &&
                     BoxInventoryManager.Instance.GetHeldItemName() == "seedBag" &&
                     TryGetClickedFarmTile(out var pos2, out _))
            {
                farmManager.PlantSeed(pos2);
                Debug.Log("씨앗 뿌리기 완료");
            }
            else
            {
                Debug.Log("너무 멀거나 밭이 아닙니다. 물이나 씨앗을 사용할 수 없습니다.");
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
    void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsInventoryOpen())
            return;

        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            Debug.Log("접촉한 아이템: " + other.name);
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
