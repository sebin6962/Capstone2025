using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;
    public PopupInventoryUIManager popupUIManager;

    private bool isNearMaker = false;
    private string currentMakerTag;

    private void Update()
    {
        if (isNearMaker && Input.GetKeyDown(interactKey))
        {
            if (MakerPopupData.popupMap.TryGetValue(currentMakerTag, out var info))
            {
                Debug.Log($"[��ȣ�ۿ�] {currentMakerTag} ���۱� �˾� ����");
                popupUIManager.ShowPopupForCategory(info.category, info.title, info.actionLabel);
            }
            else
            {
                Debug.LogWarning($"[��ȣ�ۿ�] {currentMakerTag} ���۱⿡ ���� ���� ����");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (MakerPopupData.popupMap.ContainsKey(other.tag))
        {
            currentMakerTag = other.tag;
            isNearMaker = true;
            Debug.Log($"[��ȣ�ۿ�] {currentMakerTag} ���۱� ����");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (MakerPopupData.popupMap.ContainsKey(other.tag))
        {
            isNearMaker = false;
            Debug.Log($"[��ȣ�ۿ�] {currentMakerTag} ���۱� ��Ż");
        }
    }
}
