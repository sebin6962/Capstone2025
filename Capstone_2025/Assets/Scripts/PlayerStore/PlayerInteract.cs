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
                Debug.Log($"[상호작용] {currentMaker.makerId} 제작기 팝업 열기");
                popupUIManager.ShowPopupForCategory(info.category, info.title, info.actionLabel);
            }
            else
            {
                Debug.LogWarning($"등록되지 않은 제작기: {currentMaker.makerId}");
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
            Debug.Log($"접근: {currentMaker.makerId}");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var maker = other.GetComponent<MakerInfo>();
        if (maker != null && currentMaker == maker)
        {
            isNearMaker = false;
            Debug.Log($"이탈: {currentMaker.makerId}");
            currentMaker = null;
        }
    }
}
