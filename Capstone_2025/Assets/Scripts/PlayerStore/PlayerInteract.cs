using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public static PlayerInteract Instance;
    public KeyCode interactKey = KeyCode.E;
    public PopupInventoryUIManager popupUIManager;

    private bool isNearMaker = false;
    private string currentMakerTag;

    public MakerInfo currentMaker;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {

        if (Input.GetKeyDown(interactKey))
        {
            // �˾� ���� ������ �ݱ�
            if (PopupInventoryUIManager.Instance.IsPopupOpen())
            {
                PopupInventoryUIManager.Instance.HidePopup();
                return;
            }

            //    // �˾��� ���� �ִٸ� ���۱� ��ó���� Ȯ��
            //    if (isNearMaker && currentMaker != null)
            //    {
            //        if (MakerPopupData.popupMap.TryGetValue(currentMaker.makerId, out var info))
            //        {
            //            Debug.Log($"[��ȣ�ۿ�] {currentMaker.makerId} ���۱� �˾� ����");
            //            popupUIManager.ShowPopupForCategory(info.category, info.title, info.actionLabel);
            //        }
            //    }
            //}

            // ���۱� ����� �����Ѵٸ� ���� �켱
            if (isNearMaker && currentMaker != null && currentMaker.currentResultObject != null)
            {
                var resultSpriteRenderer = currentMaker.currentResultObject.GetComponent<SpriteRenderer>();
                if (resultSpriteRenderer != null)
                {
                    Sprite resultSprite = resultSpriteRenderer.sprite;

                    // �÷��̾� �Ӹ� ���� ǥ��
                    HeldItemManager.Instance.ShowHeldItem(resultSprite, resultSprite.name);

                    // ��� ������Ʈ ����
                    Destroy(currentMaker.currentResultObject);
                    currentMaker.currentResultObject = null;

                    Debug.Log($"[���� ��� ����] {resultSprite.name} �� �÷��̾ ��� ����");
                    return; // ���������� �˾� ���� ����
                }
            }

            // �˾� ����
            if (isNearMaker && currentMaker != null)
            {
                if (MakerPopupData.popupMap.TryGetValue(currentMaker.makerId, out var info))
                {
                    Debug.Log($"[��ȣ�ۿ�] {currentMaker.makerId} ���۱� �˾� ����");
                    popupUIManager.ShowPopupForCategory(info.category, info.title, info.actionLabel);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var maker = other.GetComponent<MakerInfo>();
        if (maker != null && MakerPopupData.popupMap.ContainsKey(maker.makerId))
        {
            currentMaker = maker;
            isNearMaker = true;
            Debug.Log($"����: {currentMaker.makerId}");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var maker = other.GetComponent<MakerInfo>();
        if (maker != null && currentMaker == maker)
        {
            isNearMaker = false;
            Debug.Log($"��Ż: {currentMaker.makerId}");
            currentMaker = null;
        }
    }
}
