using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBoss : MonoBehaviour
{
    [SerializeField] private GameObject boss;

    private void OnTriggerEnter(Collider other)
    {
        LoadBossScene();
        Destroy(gameObject);
    }
    public void LoadBossScene()
    {
        CinemachineManager.Instance.LoadBossCam();
        GetComponent<BoxCollider>().enabled = false;
        boss.SetActive(true);
    }
}
