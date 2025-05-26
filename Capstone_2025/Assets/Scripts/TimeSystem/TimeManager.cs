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

    public TMP_Text dayText;               // "1일차"
    public Image clockProgressImage;       // 원형 이미지

    public int currentDay = 1;             // 일차
    private int totalGameMinutes = (26 - 9) * 60; // 하루 총 분(9시 ~ 26시 → 1020분)

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

        // VillageScene에서만 동작 (혹시 모르니 Scene 체크)
        if (SceneManager.GetActiveScene().name == "VillageScene")
        {
            // StatementScene에서만 플래그 ON
            int isNextDay = PlayerPrefs.GetInt("NextDayFlag", 0);
            if (isNextDay == 1)
            {
                currentDay++;
                hour = 9;
                minute = 0;
                SaveDayData();
                // 플래그 리셋
                PlayerPrefs.SetInt("NextDayFlag", 0);
            }
            // else : 플래그가 없으면 날짜/시간 그대로 유지
        }
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
        dayText.text = $"{currentDay}일차";
    }

    IEnumerator EndOfDayRoutine()
    {
        SaveDayData(); // 씬 전환 전에 반드시 저장

        yield return new WaitForSeconds(1f);

        currentDay++; // 하루 종료 후 다음 날

        if (FadeManager.Instance != null)
            FadeManager.Instance.FadeToScene("StatementScene");
        else
            SceneManager.LoadScene("StatementScene");
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
    void OnApplicationQuit()
    {
        SaveDayData();
    }

    void OnDisable()
    {
        SaveDayData();
    }
}

[System.Serializable]
public class DayData
{
    public int day;
    public int hour;
    public int minute;
}
