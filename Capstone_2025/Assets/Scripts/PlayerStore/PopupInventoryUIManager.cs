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

    public Transform floatingImageTarget; // 플레이어 머리 위에 따라갈 대상 (예: Player의 empty 자식 오브젝트)
    public RectTransform floatingItemContainer; // 그리드 컨테이너
    public RectTransform floatingItemImagePrefab; // 아이콘 프리팹 (Image + RectTransform)
    public int maxFloatingItems = 4;

    private List<RectTransform> floatingImages = new();        // 생성된 이미지들
    private List<string> selectedItemNames = new();             // 선택된 아이템 이름 추적

    // UI 텍스트들
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI actionButtonText;

    public RectTransform progressBarPrefab;
    public GameObject resultItemPrefab;
    public Transform worldCanvasParent; // 진행도 바 배치할 부모 (월드 스페이스 Canvas)

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
        Debug.Log($"[팝업] 카테고리 '{category}'에 맞는 아이템만 표시");

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
                Debug.LogWarning($"[팝업] 스프라이트 로드 실패: {pair.Key}");
                continue;
            }

            slots[i].SetItem(pair.Key, sprite, pair.Value);
            i++;
        }

        // 텍스트 설정
        titleText.text = title;
        actionButtonText.text = actionBtn;

        panel.SetActive(true);
        closeButton.gameObject.SetActive(true);
    }

    public void HidePopup()
    {
        panel.SetActive(false);
        closeButton.gameObject.SetActive(false);

        // 생성된 아이콘 프리팹 전부 제거
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
                Debug.LogWarning($"[팝업] 스프라이트 로드 실패: {pair.Key}");
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
        Debug.Log($"[OnItemSelected] {itemName} 선택됨. floatingImages.Count = {floatingImages.Count}");
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
            Debug.LogWarning("재료를 하나 이상 선택해야 합니다.");
            return;
        }

        HidePopup();

        if (PlayerInteract.Instance.currentMaker == null)
        {
            Debug.LogWarning("currentMaker가 null입니다.");
            return;
        }

        // 선택된 재료 수량 차감
        foreach (var itemName in selectedItemNames)
        {
            StorageInventory.Instance.AddItem(itemName, -1);  // 수량 -1
        }

        StartCoroutine(ShowProgressAndSpawnItem(PlayerInteract.Instance.currentMaker));
    }

    private IEnumerator ShowProgressAndSpawnItem(MakerInfo maker)
    {
        Transform makerTransform = maker.transform;

        // 1. 원형 프리팹 생성
        RectTransform progressBar = Instantiate(progressBarPrefab, worldCanvasParent);
        Vector3 worldPos = makerTransform.position + new Vector3(0f, 1.2f, 0f);
        progressBar.position = worldPos;

        // Z 위치 조정 (카메라 앞으로)
        if (Camera.main != null)
            progressBar.position += Camera.main.transform.forward * 0.01f;

        //'Fill' 오브젝트 찾기 (원형 이미지)
        Transform fill = progressBar.transform.Find("Fill");
        if (fill == null)
        {
            Debug.LogError("프리팹 안에 'Fill' 오브젝트가 없습니다!");
            yield break;
        }

        Image fillImage = fill.GetComponent<Image>();
        fillImage.fillAmount = 0f;

        float duration = 3f;
        float elapsed = 0f;

        // 2. 시간에 따라 fillAmount 증가
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fillImage.fillAmount = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        // 3. 진행도 바 제거 전 → 결과 위치 저장
        Vector3 progressBarPos = progressBar.position;

        // 3. 바 제거
        Destroy(progressBar.gameObject);

        // 4. 선택된 재료 이름 리스트 복사
        List<string> selectedIngredients = new(selectedItemNames);

        // 5. 결과 스프라이트 가져오기
        Sprite resultSprite = CraftingRecipeManager.Instance.GetResultSprite(
        maker.makerId, selectedIngredients
    );

        // 6. 스프라이트 없을 경우 기본 처리
        if (resultSprite == null)
        {
            Debug.LogWarning("레시피 일치 실패 또는 스프라이트 로드 실패 → 기본결과 사용");
            resultSprite = Resources.Load<Sprite>("Sprites/기본결과");
        }

        // 7. 결과 아이템 생성
        GameObject result = Instantiate(resultItemPrefab, progressBarPos, Quaternion.identity);
        SpriteRenderer sr = result.GetComponent<SpriteRenderer>();
        sr.sprite = resultSprite;

        //selectedItemNames.Clear();
        //foreach (var image in floatingImages)
        //    Destroy(image.gameObject);
        //floatingImages.Clear();

        // 제작기마다 하나만 생성되도록 maker에 기록
        maker.currentResultObject = result;

        // 스프라이트 이름 저장 (결과 수거 시 들고 다닐 아이템 이름)
        ResultItem ri = result.AddComponent<ResultItem>();
        ri.itemName = resultSprite.name;
    }
}