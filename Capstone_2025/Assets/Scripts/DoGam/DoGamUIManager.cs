using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DoGamUIManager : MonoBehaviour
{
    public static DoGamUIManager Instance;

    public GameObject panel;
    public Button openButton;
    public Button closeButton;

    public Image itemImage;
    public Text nameText;
    public Text descriptionText;
    public Text recipeText;

    private Dictionary<string, DoGamEntry> doGamDict;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        openButton.onClick.AddListener(() => OpenDoGam("�鼳��"));
        closeButton.onClick.AddListener(CloseDoGam);

        panel.SetActive(false);
        LoadDoGamDataFromJSON();
    }

    void LoadDoGamDataFromJSON()
    {
        TextAsset json = Resources.Load<TextAsset>("DoGamData");
        var data = JsonUtility.FromJson<DoGamEntryList>(json.text);

        doGamDict = new Dictionary<string, DoGamEntry>();
        foreach (var entry in data.entries)
            doGamDict[entry.name] = entry;
    }

    public void OpenDoGam(string itemName)
    {
        if (!doGamDict.ContainsKey(itemName))
        {
            Debug.LogWarning($"���� �׸� '{itemName}'�� ã�� �� �����ϴ�.");
            return;
        }

        var entry = doGamDict[itemName];
        panel.SetActive(true);

        nameText.text = entry.name;
        descriptionText.text = entry.description;
        recipeText.text = string.Join("\n", entry.recipe);

        itemImage.sprite = Resources.Load<Sprite>("Sprites/" + entry.image);
    }

    public void CloseDoGam()
    {
        panel.SetActive(false);
    }
}
