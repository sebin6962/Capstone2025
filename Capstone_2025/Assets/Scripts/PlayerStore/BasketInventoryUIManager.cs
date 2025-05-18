using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BasketInventoryUIManager : MonoBehaviour
{
    public static BasketInventoryUIManager Instance;

    public GameObject basketPanel;
    public RectTransform panelTransform;
    public List<BasketInventorySlot> slots;
    public Vector3 offset = new(0, 2.5f, 0);

    private BasketInventory currentBasket;
    private Transform player;

    public bool IsOpen { get; private set; } = false;

    private HashSet<string> selectedItemNames = new();

    private List<int> selectedSlotIndices = new();

    [Header("���� ����")]
    public RectTransform progressBarPrefab;
    public GameObject resultItemPrefab;
    public Transform worldCanvasParent;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void OpenBasketUI(BasketInventory basket, Transform playerTransform)
    {
        currentBasket = basket;
        player = playerTransform;

        // �ε�
        currentBasket.Load();

        // UI ����
        UpdateUI();

        basketPanel.SetActive(true);
        IsOpen = true;
    }

    public void CloseBasketUI()
    {
        basketPanel.SetActive(false);
        IsOpen = false;
    }

    private void LateUpdate()
    {
        if (IsOpen && player != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(player.position + offset);
            panelTransform.position = screenPos;
        }
    }

    public void UpdateUI()
    {
        Dictionary<string, int> itemCounts = new();
        Dictionary<string, Sprite> itemSprites = new();

        for (int i = 0; i < currentBasket.itemNames.Count; i++)
        {
            string name = currentBasket.itemNames[i];
            Sprite sprite = currentBasket.itemSprites[i];

            if (!itemCounts.ContainsKey(name))
            {
                itemCounts[name] = 1;
                itemSprites[name] = sprite;
            }
            else
            {
                itemCounts[name]++;
            }
        }

        int index = 0;
        foreach (var pair in itemCounts)
        {
            if (index >= slots.Count) break;
            slots[index].SetItem(pair.Key, itemSprites[pair.Key], pair.Value);
            slots[index].slotIndex = index;
            index++;
        }

        for (int i = index; i < slots.Count; i++)
        {
            slots[i].Clear();
        }
    }

    public void OnSlotClicked(int index)
    {
        if (index >= slots.Count) return;

        //string itemName = slots[index].itemName;
        var slot = slots[index];

        if (string.IsNullOrEmpty(slot.itemName))
        {
            Debug.LogWarning("[���� ���� ����] ��ȿ���� ���� �� ������ ���õǾ����ϴ�.");
            return;
        }
        string itemName = slot.itemName;

        if (PlayerStoreBoxInventoryUIManager.Instance.IsOpen())
        {
            int removeIndex = currentBasket.itemNames.IndexOf(itemName);
            if (removeIndex != -1)
            {
                currentBasket.itemNames.RemoveAt(removeIndex);
                currentBasket.itemSprites.RemoveAt(removeIndex);
                StorageInventory.Instance.AddItem(itemName, 1);
                currentBasket.Save();
                UpdateUI();
                PlayerStoreBoxInventoryUIManager.Instance.UpdateSlots();
            }
            return;
        }

        if (PlayerInteract.Instance.IsNearMaker())
        {
            if (selectedSlotIndices.Contains(index))
            {
                selectedSlotIndices.Remove(index);
                slot.Deselect();
                Debug.Log($"[���� ����] {itemName}");
            }
            else
            {
                selectedSlotIndices.Add(index);
                slot.Select();
                Debug.Log($"[����] {itemName}");
            }
        }

    }

    public void StartCrafting(MakerInfo maker)
    {
        // ���õ� ������ �̸� ����Ʈ ����
        List<string> selectedItemNames = new();
        foreach (int idx in selectedSlotIndices)
        {
            selectedItemNames.Add(slots[idx].itemName);

            if (!string.IsNullOrEmpty(name))
                selectedItemNames.Add(name);
        }

        if (selectedSlotIndices.Count == 0)
        {
            Debug.LogWarning("[���� ����] ���õ� ������ ����");
            return;
        }

        // ���Կ��� �̸� �� ��������Ʈ ����
        int selectedSlotIndex = selectedSlotIndices[0];
        string resultItemName = slots[selectedSlotIndex].itemName;
        Sprite resultSprite = slots[selectedSlotIndex].itemSprite;

        // ��ȿ�� ���������� �˻�
        if (string.IsNullOrEmpty(resultItemName))
        {
            Debug.LogWarning("[���� ����] ��ȿ���� ���� �������Դϴ�");
            return;
        }

        // ��� ����
        foreach (int idx in selectedSlotIndices)
        {
            string name = slots[idx].itemName;
            int removeIndex = currentBasket.itemNames.IndexOf(name);
            if (removeIndex != -1)
            {
                currentBasket.itemNames.RemoveAt(removeIndex);
                currentBasket.itemSprites.RemoveAt(removeIndex);
            }
            currentBasket.Save();
            selectedSlotIndices.Clear();
            UpdateUI();
            CloseBasketUI();

            StartCoroutine(ShowProgressAndSpawnItem(maker, selectedItemNames));
        }
    }

    private IEnumerator ShowProgressAndSpawnItem(MakerInfo maker, List<string> selectedItemNames)
    {
        Transform makerTransform = maker.transform;

        RectTransform progressBar = Instantiate(progressBarPrefab, worldCanvasParent);
        Vector3 worldPos = makerTransform.position + new Vector3(0f, 1.2f, 0f);
        progressBar.position = worldPos;

        if (Camera.main != null)
            progressBar.position += Camera.main.transform.forward * 0.01f;

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

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fillImage.fillAmount = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        Vector3 progressBarPos = progressBar.position;
        Destroy(progressBar.gameObject);

        Sprite resultSprite = CraftingRecipeManager.Instance.GetResultSprite(maker.makerId, selectedItemNames);

        if (resultSprite == null)
        {
            Debug.LogWarning("������ ��ġ ���� �Ǵ� ��������Ʈ �ε� ���� �� �⺻��� ���");
            resultSprite = Resources.Load<Sprite>("Sprites/�⺻���");
        }

        GameObject result = Instantiate(resultItemPrefab, progressBarPos, Quaternion.identity);
        SpriteRenderer sr = result.GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            sr.sprite = resultSprite;
        }
        else
        {
            Debug.LogError("[���� ����] SpriteRenderer�� �����տ� �����ϴ�!");
        }

        maker.currentResultObject = result;

        ResultItem ri = result.AddComponent<ResultItem>();
        ri.itemName = resultSprite.name;

        Debug.Log($"[���� �Ϸ�] {resultSprite.name} ������");
    }

    public bool TryAddItemToBasket(string itemName, Sprite sprite)
    {
        if (currentBasket == null) return false;
        bool added = currentBasket.AddItem(itemName, sprite);
        if (added) UpdateUI(); // �߰� ���� �� UI ����
        return added;
    }

    public void SetPlayer(Transform playerTransform)
    {
        this.player = playerTransform;
    }

    public bool CanSelectSlot()
    {
        return PlayerInteract.Instance.IsNearMaker(); // ���۱�� ���� ���¸�
    }

    public void OnItemSelected(string itemName, int slotIndex)
    {
        if (!selectedSlotIndices.Contains(slotIndex))
            selectedSlotIndices.Add(slotIndex);
    }

    public void OnItemDeselected(string itemName, int slotIndex)
    {
        selectedSlotIndices.Remove(slotIndex);
    }
}
