using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TimeManager : MonoBehaviour
{
    public int hour = 9;
    public int minute = 0;
    public float realSecondsPerGameMinute = 0.25f;
    private float timer = 0f;

    public TMP_Text dayText;               // "1����"
    public Image clockProgressImage;       // ���� �̹���

    public int currentDay = 1;             // ����
    private int totalGameMinutes = (26 - 9) * 60; // �Ϸ� �� ��(9�� ~ 26�� �� 1020��)

    public static TimeManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        LoadDay();
        UpdateDayUI();
        UpdateClockProgressUI();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= realSecondsPerGameMinute)
        {
            timer = 0f;
            minute += 1;
            if (minute >= 60)
            {
                minute = 0;
                hour += 1;

                if (hour >= 26)
                {
                    StartCoroutine(EndOfDayRoutine());
                }
            }

            UpdateClockProgressUI();
        }
    }

    void LoadDay()
    {
        string path = Path.Combine(Application.persistentDataPath, "dayData.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            DayData data = JsonUtility.FromJson<DayData>(json);
            currentDay = data.day;
            hour = data.hour;
            minute = data.minute;
        }
        else
        {
            currentDay = 1;
            hour = 9;
            minute = 0;
        }
    }

    void UpdateClockProgressUI()
    {
        int minutesPassed = (hour - 9) * 60 + minute;
        float progress = Mathf.Clamp01((float)minutesPassed / totalGameMinutes);
        clockProgressImage.fillAmount = progress;
    }

    

    void UpdateDayUI()
    {
        dayText.text = $"{currentDay}����";
    }

    IEnumerator EndOfDayRoutine()
    {
        SaveDayData(); // �� ��ȯ ���� �ݵ�� ����

        yield return new WaitForSeconds(1f);

        currentDay++; // �Ϸ� ���� �� ���� ��

        if (FadeManager.Instance != null)
            FadeManager.Instance.FadeToScene("ReceiptScene");
        else
            SceneManager.LoadScene("ReceiptScene");
    }

    public void SaveDayData()
    {
        string path = Path.Combine(Application.persistentDataPath, "dayData.json");
        DayData data = new DayData
        {
            day = currentDay,
            hour = hour,
            minute = minute
        };
        File.WriteAllText(path, JsonUtility.ToJson(data));
    }
}

[System.Serializable]
public class DayData
{
    public int day;
    public int hour;
    public int minute;
}
