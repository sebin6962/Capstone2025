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
            // 팝업 열려 있으면 닫기
            if (PopupInventoryUIManager.Instance.IsPopupOpen())
            {
                PopupInventoryUIManager.Instance.HidePopup();
                return;
            }

            //    // 팝업이 닫혀 있다면 제작기 근처인지 확인
            //    if (isNearMaker && currentMaker != null)
            //    {
            //        if (MakerPopupData.popupMap.TryGetValue(currentMaker.makerId, out var info))
            //        {
            //            Debug.Log($"[상호작용] {currentMaker.makerId} 제작기 팝업 열기");
            //            popupUIManager.ShowPopupForCategory(info.category, info.title, info.actionLabel);
            //        }
            //    }
            //}

            // 제작기 결과가 존재한다면 수거 우선
            if (isNearMaker && currentMaker != null && currentMaker.currentResultObject != null)
            {
                var resultSpriteRenderer = currentMaker.currentResultObject.GetComponent<SpriteRenderer>();
                if (resultSpriteRenderer != null)
                {
                    Sprite resultSprite = resultSpriteRenderer.sprite;

                    // 플레이어 머리 위에 표시
                    HeldItemManager.Instance.ShowHeldItem(resultSprite, resultSprite.name);

                    // 결과 오브젝트 삭제
                    Destroy(currentMaker.currentResultObject);
                    currentMaker.currentResultObject = null;

                    Debug.Log($"[제작 결과 수거] {resultSprite.name} 을 플레이어가 들고 있음");
                    return; // 수거했으면 팝업 열지 않음
                }
            }

            // 팝업 열기
            if (isNearMaker && currentMaker != null)
            {
                if (MakerPopupData.popupMap.TryGetValue(currentMaker.makerId, out var info))
                {
                    Debug.Log($"[상호작용] {currentMaker.makerId} 제작기 팝업 열기");
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
