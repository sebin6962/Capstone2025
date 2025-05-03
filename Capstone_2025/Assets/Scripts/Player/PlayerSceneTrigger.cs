using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerSceneTrigger : MonoBehaviour
{
    private string targetScene = "";
    private bool isInTrigger = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "PlayerStore")
        {
            targetScene = "PlayerStoreScene"; // �̵��� �� �̸�
            isInTrigger = true;
        }

        if (other.name == "TreeEntry")
        {
            targetScene = "TreeScene"; // �̵��� �� �̸�
            isInTrigger = true;
        }

        if (other.name == "Market")
        {
            targetScene = "MarketScene"; // �̵��� �� �̸�
            isInTrigger = true;
        }

        if (other.name == "MillStore")
        {
            targetScene = "MillScene"; // �̵��� �� �̸�
            isInTrigger = true;
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.name == "PlayerStore")
        {
            targetScene = "";
            isInTrigger = false;
        }

        if (other.name == "TreeEntry")
        {
            targetScene = "";
            isInTrigger = false;
        }

        if (other.name == "Market")
        {
            targetScene = "";
            isInTrigger = false;
        }

        if (other.name == "MillStore")
        {
            targetScene = "";
            isInTrigger = false;
        }
    }

    private void Update()
    {
        if (isInTrigger && Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("��ȣ�ۿ�: WŰ ���� �� �� ��ȯ ��");
            SceneManager.LoadScene(targetScene);
        }
    }
}
