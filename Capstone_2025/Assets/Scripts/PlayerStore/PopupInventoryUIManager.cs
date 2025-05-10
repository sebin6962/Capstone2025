using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupInventoryUIManager : MonoBehaviour
{
    public GameObject panel;
    public List<StorageInventorySlot> slots;

    public static PopupInventoryUIManager Instance;

    public Transform floatingImageTarget; // �÷��̾� �Ӹ� ���� ���� ��� (��: Player�� empty �ڽ� ������Ʈ)
    public RectTransform floatingItemContainer; // �׸��� �����̳�
    public RectTransform floatingItemImagePrefab; // ������ ������ (Image + RectTransform)
    public int maxFloatingItems = 4;

    private List<RectTransform> floatingImages = new();        // ������ �̹�����
    private List<string> selectedItemNames = new();             // ���õ� ������ �̸� ����

    // UI �ؽ�Ʈ��
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI actionButtonText;

    public GameObject closeButton;
    private void Awake()
    {
        Instance = this;
    }

    private void LateUpdate()
    {
        if (floatingItemContainer != null && floatingImageTarget != null)
        {
            floatingItemContainer.position = floatingImageTarget.position;
        }
    }

    public void ShowPopup()
    {
        UpdateSlots();
        panel.SetActive(true);
        closeButton.gameObject.SetActive(true); 
    }

    public bool IsPopupOpen()
    {
        return panel.activeSelf;
    }

    public void ShowPopupForCategory(string category, string title, string actionBtn)
    {
        Debug.Log($"[�˾�] ī�װ� '{category}'�� �´� �����۸� ǥ��");

        foreach (var slot in slots)
            slot.ClearSlot();

        int i = 0;
        foreach (var pair in StorageInventory.Instance.GetAllItems())
        {
            if (!IngredientCategoryMap.categoryByItem.TryGetValue(pair.Key, out var itemCategory)) continue;
            if (itemCategory != category) continue;

            if (i >= slots.Count) break;

            Sprite sprite = Resources.Load<Sprite>("Sprites/" + pair.Key);
            if (sprite == null)
            {
                Debug.LogWarning($"[�˾�] ��������Ʈ �ε� ����: {pair.Key}");
                continue;
            }

            slots[i].SetItem(sprite, pair.Value);
            i++;
        }

        // �ؽ�Ʈ ����
        titleText.text = title;
        actionButtonText.text = actionBtn;

        panel.SetActive(true);
        closeButton.gameObject.SetActive(true);
    }

    public void HidePopup()
    {
        panel.SetActive(false);
        closeButton.gameObject.SetActive(false);

        // ������ ������ ������ ���� ����
        foreach (var image in floatingImages)
        {
            if (image != null)
                Destroy(image.gameObject);
        }

        floatingImages.Clear();
        selectedItemNames.Clear();
    }

    public void UpdateSlots()
    {
        foreach (var slot in slots)
            slot.ClearSlot();

        int i = 0;
        foreach (var pair in StorageInventory.Instance.GetAllItems())
        {
            if (i >= slots.Count) break;

            Sprite sprite = Resources.Load<Sprite>("Sprites/" + pair.Key);
            if (sprite == null)
            {
                Debug.LogWarning($"[�˾�] ��������Ʈ �ε� ����: {pair.Key}");
                continue;
            }

            slots[i].SetItem(sprite, pair.Value);
            i++;
        }
    }


    public void OnItemSelected(string itemName, Sprite sprite)
    {
        if (selectedItemNames.Contains(itemName) || selectedItemNames.Count >= maxFloatingItems)
            return;

        selectedItemNames.Add(itemName);

        RectTransform newImage = Instantiate(floatingItemImagePrefab, floatingItemImagePrefab.parent);
        newImage.GetComponent<Image>().sprite = sprite;
        newImage.gameObject.SetActive(true);

        floatingImages.Add(newImage);
        Debug.Log($"[OnItemSelected] {itemName} ���õ�. floatingImages.Count = {floatingImages.Count}");
    }


    public void OnItemDeselected(string itemName)
    {
        int index = selectedItemNames.IndexOf(itemName);
        if (index < 0) return;

        Destroy(floatingImages[index].gameObject);
        floatingImages.RemoveAt(index);
        selectedItemNames.RemoveAt(index);
    }

}
