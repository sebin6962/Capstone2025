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

    //�翡 �� �ִ� ����
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
        // �κ��丮 ���� ������ ������ ����
        if (GameManager.Instance.inventoryPanel.activeSelf)
        {
            movement = Vector2.zero;
            return;
        }

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");//�Է°���

        if (currentItem != null && Input.GetKeyDown(KeyCode.Space))
        {
            if (InventoryManager.Instance.IsHoldingItem())
            {
                Debug.Log("�̹� �������̳� ������ ��� �־ ���ο� ���� �� �� �����ϴ�.");
                return;
            }

            // ���� �α�
            if (ToolData.Instance.IsTool(currentItem.name))
                Debug.Log("���� ���: " + currentItem.name);
            else
                Debug.Log("������ ���: " + currentItem.name);

            InventoryManager.Instance.HoldItem(currentItem);
        }

        //�翡 ���ֱ�
        if (Input.GetMouseButtonDown(0) && InventoryManager.Instance.IsHoldingWateringCan())
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;

            Vector3Int clickedCell = farmManager.fieldTilemap.WorldToCell(mouseWorldPos);
            Vector3Int playerCell = farmManager.fieldTilemap.WorldToCell(transform.position);

            int dx = Mathf.Abs(clickedCell.x - playerCell.x);
            int dy = Mathf.Abs(clickedCell.y - playerCell.y);

            if (dx <= 1 && dy <= 1) // �ֺ� �� ĭ �̳�
            {
                if (farmManager.IsFarmTile(mouseWorldPos))
                {
                    farmManager.WaterSoil(mouseWorldPos);
                    Debug.Log("�� �Ѹ��� �Ϸ� (�Ÿ� ����)");
                }
                else
                {
                    Debug.Log("����� ���� �ƴմϴ�.");
                }
            }
            else
            {
                Debug.Log("�ʹ� �־ ���� �� �� �����ϴ�.");
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
