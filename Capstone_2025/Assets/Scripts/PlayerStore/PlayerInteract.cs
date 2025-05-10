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
                Debug.Log($"[상호작용] {currentMakerTag} 제작기 팝업 열기");
                popupUIManager.ShowPopupForCategory(info.category, info.title, info.actionLabel);
            }
            else
            {
                Debug.LogWarning($"[상호작용] {currentMakerTag} 제작기에 대한 정보 없음");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (MakerPopupData.popupMap.ContainsKey(other.tag))
        {
            currentMakerTag = other.tag;
            isNearMaker = true;
            Debug.Log($"[상호작용] {currentMakerTag} 제작기 접근");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (MakerPopupData.popupMap.ContainsKey(other.tag))
        {
            isNearMaker = false;
            Debug.Log($"[상호작용] {currentMakerTag} 제작기 이탈");
        }
    }
}
