using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpwaner : MonoBehaviour
{
    void Start()
    {
        string entrance = SceneTransitionInfo.Instance.entranceID;
        GameObject spawnPoint = GameObject.Find(entrance);

        if (spawnPoint != null)
        {
            transform.position = spawnPoint.transform.position;

            // 방향 예시 (선택적)
            //var animator = GetComponent<PlayerAnimator>();
            //if (entrance == "FromVillage") animator?.LookDown();
            //else if (entrance == "FromStore") animator?.LookUp();
        }
    }
}
