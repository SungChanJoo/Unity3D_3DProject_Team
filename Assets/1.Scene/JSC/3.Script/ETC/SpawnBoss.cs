using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBoss : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        CinemachineManager.Instance.LoadBossCam();
        Debug.Log("왜 오류가 날까용");
        GetComponent<BoxCollider>().enabled = false;
        Destroy(gameObject);
    }
}
