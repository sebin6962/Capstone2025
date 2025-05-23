using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterButtonSlot : MonoBehaviour
{
    public Image background;
    public Color selectedColor = new(0.7f, 0.9f, 1f);
    public Color defaultColor = Color.white;

    private bool isSelected = false;
    public bool IsSelected() => isSelected; // 혹은 선택 상태를 저장하는 변수 이름

    public void Select()
    {
        isSelected = true;
        UpdateUI();
    }

    public void Deselect()
    {
        isSelected = false;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (background != null)
            background.color = isSelected ? selectedColor : defaultColor;
    }
}
