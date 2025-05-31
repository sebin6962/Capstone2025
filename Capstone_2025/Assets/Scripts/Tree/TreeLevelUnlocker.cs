using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class TreeLevelUnlocker : MonoBehaviour
{
    public Button[] levelButtons;                // 0: 첫 레벨
    public TMP_Text[] levelDescTexts;            // 0: 첫 레벨 설명 텍스트
    public int[] starlightNeededForLevel;        // 0: 첫 레벨에 필요한 별빛
    public string[] levelDescriptions;           // 0: 첫 레벨 해금시 보여줄 텍스트
    public Color unlockedColor;
    public Color lockedColor;

    // 툴팁 관련 필드 추가
    public GameObject tooltipPanel;
    public TMP_Text tooltipText;

    private int currentUnlockedLevel = 0; // 0=첫번째 레벨만 해금

    public GameObject notEnoughStarlightPanel; // 별빛 부족 알림 패널
    public CanvasGroup notEnoughStarlightGroup; // 알림 패널의 CanvasGroup

    private Coroutine notEnoughCoroutine = null;

    void Start()
    {
        UpdateLevelButtons();

        // 모든 버튼에 이벤트 리스너 추가
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int idx = i; // 지역 변수로 캡처
            EventTrigger trigger = levelButtons[i].gameObject.GetComponent<EventTrigger>();
            if (trigger == null) trigger = levelButtons[i].gameObject.AddComponent<EventTrigger>();

            // 마우스 오버
            EventTrigger.Entry entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            entryEnter.callback.AddListener((_) => ShowTooltip(idx));
            trigger.triggers.Add(entryEnter);

            // 마우스 아웃
            EventTrigger.Entry entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            entryExit.callback.AddListener((_) => HideTooltip());
            trigger.triggers.Add(entryExit);
        }
    }

    // 툴팁 표시/숨김 메서드
    public void ShowTooltip(int levelIdx)
    {
        tooltipPanel.SetActive(true);
        tooltipText.text = $"{starlightNeededForLevel[levelIdx]} 개의 별빛";

        // 위치 이동
        RectTransform buttonRect = levelButtons[levelIdx].GetComponent<RectTransform>();
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null, buttonRect.position);

        RectTransform tooltipRect = tooltipPanel.GetComponent<RectTransform>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            tooltipRect.parent as RectTransform,
            screenPos,
            null,
            out localPoint
        );
        tooltipRect.anchoredPosition = localPoint + new Vector2(0, 60f); // 버튼 위로 40 픽셀 이동
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }

    public void TryUnlockLevel(int levelIdx)
    {
        // 1. 이미 해금됐거나, 선행레벨이 해금 안됐으면 리턴
        if (levelIdx > currentUnlockedLevel) return;

        int needStarlight = starlightNeededForLevel[levelIdx];

        // 2. 별빛 충분한지 체크 (DataManager 또는 StarDataManager 사용)
        int currentStarlight = StarDataManager.Instance.playerData.starlight;

        if (currentStarlight < needStarlight)
        {
            ShowNotEnoughStarlight();
            return;
        }

        // 3. 별빛 차감
        StarDataManager.Instance.SpendStarlight(needStarlight);

        // 4. 해금 표시(버튼 색상 변경)
        currentUnlockedLevel = Mathf.Max(currentUnlockedLevel, levelIdx + 1); // 다음 레벨 해금 가능하게

        UpdateLevelButtons();
    }

    void UpdateLevelButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            bool unlocked = i < currentUnlockedLevel;
            bool canUnlock = i == currentUnlockedLevel;
            var colors = levelButtons[i].colors;
            levelButtons[i].interactable = canUnlock;
            levelButtons[i].GetComponent<Image>().color = unlocked ? unlockedColor : lockedColor;

            // 레벨 별 텍스트 해금 시 나타나도록 설정
            if (unlocked)
                levelDescTexts[i].text = levelDescriptions[i];
            else
                levelDescTexts[i].text = "???";
        }
    }

    public void ShowNotEnoughStarlight()
    {
        if (notEnoughCoroutine != null)
            StopCoroutine(notEnoughCoroutine);

        notEnoughCoroutine = StartCoroutine(NotEnoughRoutine());
    }

    private IEnumerator NotEnoughRoutine()
    {
        notEnoughStarlightPanel.SetActive(true);

        // 페이드 인
        float duration = 0.5f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            notEnoughStarlightGroup.alpha = Mathf.Lerp(0, 1, elapsed / duration);
            yield return null;
        }
        notEnoughStarlightGroup.alpha = 1f;

        // 1초간 유지
        yield return new WaitForSeconds(1f);

        // 페이드 아웃
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            notEnoughStarlightGroup.alpha = Mathf.Lerp(1, 0, elapsed / duration);
            yield return null;
        }
        notEnoughStarlightGroup.alpha = 0f;

        notEnoughStarlightPanel.SetActive(false);
        notEnoughCoroutine = null;
    }
}
