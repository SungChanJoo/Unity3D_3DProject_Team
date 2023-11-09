using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBoss : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        CinemachineManager.Instance.LoadBossCam();
        GetComponent<BoxCollider>().enabled = false;
    }
}
