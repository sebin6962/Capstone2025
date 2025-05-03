using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject inventoryPanel;
    public static GameManager Instance;


    // Update is called once per frame
    void Update()
    {
        if (BoxTrigger.isPlayerNearBox && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("EŰ ���� - �κ��丮 ��� �õ�");
            ToggleInventory();
        }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void ToggleInventory()
    {
        bool isActive = inventoryPanel.activeSelf;
        inventoryPanel.SetActive(!isActive);
    }

    public bool IsInventoryOpen()
    {
        return inventoryPanel.activeSelf;
    }
}
