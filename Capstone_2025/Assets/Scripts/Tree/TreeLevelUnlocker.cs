using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class TreeLevelUnlocker : MonoBehaviour
{
    public Button[] levelButtons;                // 0: ù ����
    public TMP_Text[] levelDescTexts;            // 0: ù ���� ���� �ؽ�Ʈ
    public int[] starlightNeededForLevel;        // 0: ù ������ �ʿ��� ����
    public string[] levelDescriptions;           // 0: ù ���� �رݽ� ������ �ؽ�Ʈ
    public Color unlockedColor;
    public Color lockedColor;

    // ���� ���� �ʵ� �߰�
    public GameObject tooltipPanel;
    public TMP_Text tooltipText;

    private int currentUnlockedLevel = 0; // 0=ù��° ������ �ر�

    public GameObject notEnoughStarlightPanel; // ���� ���� �˸� �г�
    public CanvasGroup notEnoughStarlightGroup; // �˸� �г��� CanvasGroup

    private Coroutine notEnoughCoroutine = null;

    void Start()
    {
        UpdateLevelButtons();

        // ��� ��ư�� �̺�Ʈ ������ �߰�
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int idx = i; // ���� ������ ĸó
            EventTrigger trigger = levelButtons[i].gameObject.GetComponent<EventTrigger>();
            if (trigger == null) trigger = levelButtons[i].gameObject.AddComponent<EventTrigger>();

            // ���콺 ����
            EventTrigger.Entry entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            entryEnter.callback.AddListener((_) => ShowTooltip(idx));
            trigger.triggers.Add(entryEnter);

            // ���콺 �ƿ�
            EventTrigger.Entry entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            entryExit.callback.AddListener((_) => HideTooltip());
            trigger.triggers.Add(entryExit);
        }
    }

    // ���� ǥ��/���� �޼���
    public void ShowTooltip(int levelIdx)
    {
        tooltipPanel.SetActive(true);
        tooltipText.text = $"{starlightNeededForLevel[levelIdx]} ���� ����";

        // ��ġ �̵�
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
        tooltipRect.anchoredPosition = localPoint + new Vector2(0, 60f); // ��ư ���� 40 �ȼ� �̵�
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }

    public void TryUnlockLevel(int levelIdx)
    {
        // 1. �̹� �رݵưų�, ���෹���� �ر� �ȵ����� ����
        if (levelIdx > currentUnlockedLevel) return;

        int needStarlight = starlightNeededForLevel[levelIdx];

        // 2. ���� ������� üũ (DataManager �Ǵ� StarDataManager ���)
        int currentStarlight = StarDataManager.Instance.playerData.starlight;

        if (currentStarlight < needStarlight)
        {
            ShowNotEnoughStarlight();
            return;
        }

        // 3. ���� ����
        StarDataManager.Instance.SpendStarlight(needStarlight);

        // 4. �ر� ǥ��(��ư ���� ����)
        currentUnlockedLevel = Mathf.Max(currentUnlockedLevel, levelIdx + 1); // ���� ���� �ر� �����ϰ�

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

            // ���� �� �ؽ�Ʈ �ر� �� ��Ÿ������ ����
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

        // ���̵� ��
        float duration = 0.5f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            notEnoughStarlightGroup.alpha = Mathf.Lerp(0, 1, elapsed / duration);
            yield return null;
        }
        notEnoughStarlightGroup.alpha = 1f;

        // 1�ʰ� ����
        yield return new WaitForSeconds(1f);

        // ���̵� �ƿ�
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
