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
            targetScene = "PlayerStoreScene"; // 이동할 씬 이름
            isInTrigger = true;
        }

        if (other.name == "TreeEntry")
        {
            targetScene = "TreeScene"; // 이동할 씬 이름
            isInTrigger = true;
        }

        if (other.name == "Market")
        {
            targetScene = "MarketScene"; // 이동할 씬 이름
            isInTrigger = true;
        }

        if (other.name == "MillStore")
        {
            targetScene = "MillScene"; // 이동할 씬 이름
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
            Debug.Log("상호작용: W키 누름 → 씬 전환 중");
            SceneManager.LoadScene(targetScene);
        }
    }
}
