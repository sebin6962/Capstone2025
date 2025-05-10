using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;
    public PopupInventoryUIManager popupUIManager;

    private bool isNearMaker = false;
    private string currentMakerTag;

    private MakerInfo currentMaker;

    private void Update()
    {
        if (isNearMaker && Input.GetKeyDown(interactKey))
        {
            if (MakerPopupData.popupMap.TryGetValue(currentMaker.makerId, out var info))
            {
                Debug.Log($"[��ȣ�ۿ�] {currentMaker.makerId} ���۱� �˾� ����");
                popupUIManager.ShowPopupForCategory(info.category, info.title, info.actionLabel);
            }
            else
            {
                Debug.LogWarning($"��ϵ��� ���� ���۱�: {currentMaker.makerId}");
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
