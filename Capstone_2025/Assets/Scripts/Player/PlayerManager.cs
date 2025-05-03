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
            //return; // ������ �� �Է� ����
        }
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");//�Է°���

        if (currentItem != null && Input.GetKeyDown(KeyCode.Space))
        {
            // ���Ѹ����� WateringCanAnchor������ �ٷ��
            if (currentItem.name == "wateringCan")
                return;

            if (BoxInventoryManager.Instance.IsHoldingItem())
            {
                Debug.Log("�̹� �������̳� ������ ��� �־ ���ο� ���� �� �� �����ϴ�.");
                return;
            }

            // ���� �α�
            if (ToolData.Instance.IsTool(currentItem.name))
                Debug.Log("���� ���: " + currentItem.name);
            else
                Debug.Log("������ ���: " + currentItem.name);

            BoxInventoryManager.Instance.HoldItem(currentItem);
        }

        //�翡 ���ֱ�
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
