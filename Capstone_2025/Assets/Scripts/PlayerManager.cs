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

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");//입력감지

        if (currentItem != null && Input.GetKeyDown(KeyCode.Space))
        {
            if (InventoryManager.Instance.IsHoldingItem())
            {
                Debug.Log("이미 아이템을 들고 있어서 새로운 아이템을 들 수 없습니다.");
                return;
            }

            // 들기 로직 (임시로 이름 출력)
            Debug.Log("아이템 들기: " + currentItem.name);
            InventoryManager.Instance.HoldItem(currentItem);
            //currentItem.SetActive(false); // 화면에서 제거
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
