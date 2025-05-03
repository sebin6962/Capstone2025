using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeldItemManager : MonoBehaviour
{
    public static HeldItemManager Instance;

    public Image heldItemImage; // UI Image ������Ʈ (�÷��̾� �Ӹ� ��)
    public Transform player;    // �÷��̾� Transform

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void LateUpdate()
    {// ��� �ִ� �������� ������ �̹��� ����
        if (!BoxInventoryManager.Instance.IsHoldingItem())
        {
            if (heldItemImage.enabled)
            {
                heldItemImage.enabled = false;
            }
            return;
        }
        // �÷��̾� ���� ����ٴϰ� ��ġ ������Ʈ
        if (heldItemImage.enabled)
        {
            Vector3 offset = new Vector3(0, 1.5f, 0); // �Ӹ� �� ��ġ
            Vector3 screenPos = Camera.main.WorldToScreenPoint(player.position + offset); // ����� �ʱ�ȭ ���ÿ�
            heldItemImage.transform.position = screenPos;

            //Debug.Log("HeldItemImage ��ġ: " + screenPos);
        }
    }

    public void ShowHeldItem(Sprite sprite)
    {
        if (sprite == null)
        {
            Debug.LogWarning("ShowHeldItem: ��������Ʈ�� null�Դϴ�!");
            return;
        }
        if (heldItemImage == null)
        {
            Debug.LogError("heldItemImage�� ������� �ʾҽ��ϴ�! Inspector���� Image�� �巡���� �����ϼ���.");
            return;
        }
        heldItemImage.sprite = sprite;
        heldItemImage.enabled = true;
        Debug.Log("ShowHeldItem: ������ ǥ�õ� - " + sprite.name);
    }

    public void HideHeldItem()
    {
        heldItemImage.enabled = false;
    }
}
