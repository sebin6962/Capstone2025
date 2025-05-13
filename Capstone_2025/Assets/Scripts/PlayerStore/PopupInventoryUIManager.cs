using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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

    public RectTransform progressBarPrefab;
    public GameObject resultItemPrefab;
    public Transform worldCanvasParent; // ���൵ �� ��ġ�� �θ� (���� �����̽� Canvas)

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

            Sprite sprite = Resources.Load<Sprite>("Sprites/Ingredients/" + pair.Key);
            if (sprite == null)
            {
                Debug.LogWarning($"[�˾�] ��������Ʈ �ε� ����: {pair.Key}");
                continue;
            }

            slots[i].SetItem(pair.Key, sprite, pair.Value);
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
        
    }

    public void UpdateSlots()
    {
        foreach (var slot in slots)
            slot.ClearSlot();

        int i = 0;
        foreach (var pair in StorageInventory.Instance.GetAllItems())
        {
            if (i >= slots.Count) break;

            Sprite sprite = Resources.Load<Sprite>("Sprites/Ingredients/" + pair.Key);
            if (sprite == null)
            {
                Debug.LogWarning($"[�˾�] ��������Ʈ �ε� ����: {pair.Key}");
                continue;
            }

            slots[i].SetItem(pair.Key, sprite, pair.Value);
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

    public void OnActionButtonPressed()
    {
        if (selectedItemNames.Count == 0)
        {
            Debug.LogWarning("��Ḧ �ϳ� �̻� �����ؾ� �մϴ�.");
            return;
        }

        HidePopup();

        if (PlayerInteract.Instance.currentMaker == null)
        {
            Debug.LogWarning("currentMaker�� null�Դϴ�.");
            return;
        }

        // ���õ� ��� ���� ����
        foreach (var itemName in selectedItemNames)
        {
            StorageInventory.Instance.AddItem(itemName, -1);  // ���� -1
        }

        StartCoroutine(ShowProgressAndSpawnItem(PlayerInteract.Instance.currentMaker));
    }

    private IEnumerator ShowProgressAndSpawnItem(MakerInfo maker)
    {
        Transform makerTransform = maker.transform;

        // 1. ���� ������ ����
        RectTransform progressBar = Instantiate(progressBarPrefab, worldCanvasParent);
        Vector3 worldPos = makerTransform.position + new Vector3(0f, 1.2f, 0f);
        progressBar.position = worldPos;

        // Z ��ġ ���� (ī�޶� ������)
        if (Camera.main != null)
            progressBar.position += Camera.main.transform.forward * 0.01f;

        //'Fill' ������Ʈ ã�� (���� �̹���)
        Transform fill = progressBar.transform.Find("Fill");
        if (fill == null)
        {
            Debug.LogError("������ �ȿ� 'Fill' ������Ʈ�� �����ϴ�!");
            yield break;
        }

        Image fillImage = fill.GetComponent<Image>();
        fillImage.fillAmount = 0f;

        float duration = 3f;
        float elapsed = 0f;

        // 2. �ð��� ���� fillAmount ����
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fillImage.fillAmount = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        // 3. ���൵ �� ���� �� �� ��� ��ġ ����
        Vector3 progressBarPos = progressBar.position;

        // 3. �� ����
        Destroy(progressBar.gameObject);

        // 4. ���õ� ��� �̸� ����Ʈ ����
        List<string> selectedIngredients = new(selectedItemNames);

        // 5. ��� ��������Ʈ ��������
        Sprite resultSprite = CraftingRecipeManager.Instance.GetResultSprite(
        maker.makerId, selectedIngredients
    );

        // 6. ��������Ʈ ���� ��� �⺻ ó��
        if (resultSprite == null)
        {
            Debug.LogWarning("������ ��ġ ���� �Ǵ� ��������Ʈ �ε� ���� �� �⺻��� ���");
            resultSprite = Resources.Load<Sprite>("Sprites/�⺻���");
        }

        // 7. ��� ������ ����
        GameObject result = Instantiate(resultItemPrefab, progressBarPos, Quaternion.identity);
        SpriteRenderer sr = result.GetComponent<SpriteRenderer>();
        sr.sprite = resultSprite;

        //selectedItemNames.Clear();
        //foreach (var image in floatingImages)
        //    Destroy(image.gameObject);
        //floatingImages.Clear();

        // ���۱⸶�� �ϳ��� �����ǵ��� maker�� ���
        maker.currentResultObject = result;

        // ��������Ʈ �̸� ���� (��� ���� �� ��� �ٴ� ������ �̸�)
        ResultItem ri = result.AddComponent<ResultItem>();
        ri.itemName = resultSprite.name;
    }
}