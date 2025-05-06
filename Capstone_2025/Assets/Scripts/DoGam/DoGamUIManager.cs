using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;

public class DoGamUIManager : MonoBehaviour
{
    public static DoGamUIManager Instance;

    public GameObject panel;
    public Button openButton;
    public Button closeButton;

    public Button nextButton;
    public Button prevButton;

    private int currentIndex = 0;
    private List<DoGamEntry> entryList = new();

    public Image itemImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI recipeText;

    private Dictionary<string, DoGamEntry> doGamDict;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        openButton.onClick.AddListener(() => OpenDoGam("백설기"));
        closeButton.onClick.AddListener(CloseDoGam);

        panel.SetActive(false);
        prevButton.gameObject.SetActive(false); // 버튼 숨기기
        nextButton.gameObject.SetActive(false); // 버튼 숨기기
        LoadDoGamDataFromJSON();
    }

    private void Start()
    {
        nextButton.onClick.AddListener(() => NextEntry());
        prevButton.onClick.AddListener(() => PrevEntry());
    }

    private void Update()
    {
        if (!panel.activeSelf) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            PrevEntry();
        }
        else if (scroll < 0f)
        {
            NextEntry();
        }

        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData data = new PointerEventData(EventSystem.current);
            data.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(data, results);

            foreach (var r in results)
            {
                Debug.Log("Raycast hit: " + r.gameObject.name);
            }
        }
    }

    void LoadDoGamDataFromJSON()
    {
        TextAsset json = Resources.Load<TextAsset>("DoGamData");
        var data = JsonUtility.FromJson<DoGamEntryList>(json.text);

        doGamDict = new Dictionary<string, DoGamEntry>();
        entryList = new List<DoGamEntry>(data.entries);

        foreach (var entry in data.entries)
            doGamDict[entry.name] = entry;

        Debug.Log($"[도감 로딩] 항목 수: {entryList.Count}");

        for (int i = 0; i < entryList.Count; i++)
            Debug.Log($"[{i}] {entryList[i].name}");
    }

    public void OpenDoGam(string itemName)
    {
        if (!doGamDict.ContainsKey(itemName))
        {
            Debug.LogWarning($"도감 항목 '{itemName}'을 찾을 수 없습니다.");
            return;
        }

        // 버튼을 가장 위로 올림 (레이캐스트 우선순위 확보)
        prevButton.transform.SetAsLastSibling();
        nextButton.transform.SetAsLastSibling();

        var entry = doGamDict[itemName];
        panel.SetActive(true);
        prevButton.gameObject.SetActive(true); 
        nextButton.gameObject.SetActive(true); 

        nameText.text = entry.name;
        descriptionText.text = entry.description;
        recipeText.text = string.Join("\n", entry.recipe);

        itemImage.sprite = Resources.Load<Sprite>("Sprites/" + entry.image);
    }

    public void ShowEntry(int index)
    {
        if (index < 0 || index >= entryList.Count) return;

        var entry = entryList[index];

        nameText.text = entry.name;
        descriptionText.text = entry.description;
        recipeText.text = string.Join("\n", entry.recipe);
        itemImage.sprite = Resources.Load<Sprite>("Sprites/" + entry.image);
    }

    public void CloseDoGam()
    {
        panel.SetActive(false);
        prevButton.gameObject.SetActive(false); // 버튼 숨기기
        nextButton.gameObject.SetActive(false); // 버튼 숨기기
    }

    public void NextEntry()
    {
        if (currentIndex < entryList.Count - 1)
        {
            currentIndex++;
            ShowEntry(currentIndex);
            Debug.Log("다음 버튼 클릭");
        }
    }

    public void PrevEntry()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ShowEntry(currentIndex);
            Debug.Log("이전 버튼 클릭");
        }
    }
}
