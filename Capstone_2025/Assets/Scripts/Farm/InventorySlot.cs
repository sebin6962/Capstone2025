using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image itemImage;
    public Text countText;

    private string itemName;
    private int count;

    public void OnClick()
    {
        bool slotHasItem = HasItem();
        bool isHolding = BoxInventoryManager.Instance.IsHoldingItem();
        string heldName = BoxInventoryManager.Instance.GetHeldItemName();

        // [1] �տ� ���� �� ���Կ��� ������ ����
        if (!isHolding && slotHasItem)
        {
            // ������ 2 �̻��̸� �и� �� �տ� 1����, ���Կ� -1
            if (GetItemCount() > 1)
            {
                // �տ� 1�� ����
                BoxInventoryManager.Instance.HoldItemFromSlot(GetSprite(), GetItemName());

                // ���� ���� -1
                SetItem(GetSprite(), GetItemName(), GetItemCount() - 1);

                BoxInventoryManager.Instance.SaveInventory();
            }
            else
            {
                // ������ 1�̸� ����ó�� ���� ���
                BoxInventoryManager.Instance.PickUpFromSlot(this);
            }

            return;
        }

        // [2] �տ� ������ ����
        if (isHolding)
        {
            // [2-1] ���� ������ �� ���� ����
            if (slotHasItem && heldName == GetItemName())
            {
                int newCount = GetItemCount() + 1;
                SetItem(GetSprite(), heldName, newCount);
                BoxInventoryManager.Instance.RemoveHeldItem();
                BoxInventoryManager.Instance.SaveInventory();
                return;
            }

            // [2-2] �� ����
            if (!slotHasItem)
            {
                // �κ��丮�� ���� �������� �̹� �ִ��� Ȯ��
                bool existsInOtherSlot = false;

                foreach (var other in BoxInventoryManager.Instance.slots)
                {
                    if (other != this && other.HasItem() && other.GetItemName() == heldName)
                    {
                        existsInOtherSlot = true;
                        break;
                    }
                }

                // [2-2-1] �κ��丮�� ���� �� ���� ����
                if (!existsInOtherSlot)
                {
                    SetItem(BoxInventoryManager.Instance.GetHeldSprite(), heldName, 1);
                    BoxInventoryManager.Instance.RemoveHeldItem();
                    BoxInventoryManager.Instance.SaveInventory();
                }
                else
                {
                    Debug.Log("���� �������� �ٸ� ���Կ� �־� ���õ�");
                }

                return;
            }

            // [2-3] �ٸ� �������� �ִ� ���� �� ����
            Debug.Log("�ٸ� �������� �־� ���õ�");
        }
    }

    public void SetItem(Sprite sprite, string name = "", int count = 1)
    {
        if (sprite == null)
        {
            Debug.LogWarning("SetItem: null ��������Ʈ ���޵�!");
            return;
        }

        itemImage.sprite = sprite;
        itemImage.enabled = true;
        itemName = string.IsNullOrEmpty(name) ? sprite.name : name;
        this.count = count;

        UpdateUI();

        Debug.Log($"���Կ� ������ ������: {itemName} x{count}");
    }

    public void ClearSlot()
    {
        itemImage.sprite = null;
        itemImage.enabled = false;
        itemName = "";
        count = 0;

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (countText == null) return;

        if (HasItem())
        {
            countText.enabled = true;
            countText.text = (count > 1) ? count.ToString() : "";
        }
        else
        {
            countText.text = "";
            countText.enabled = false; // ������ ����� �� ����
        }
        //if (countText != null)
        //{
        //    countText.text = (count > 1) ? count.ToString() : "";
        //}
    }

    public bool HasItem()
    {
        return itemImage != null && itemImage.sprite != null;
    }

    public string GetItemName() => itemName;
    public int GetItemCount() => count;
    public Sprite GetSprite() => itemImage.sprite;
}
