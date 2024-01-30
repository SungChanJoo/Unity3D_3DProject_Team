using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBoss : MonoBehaviour
{
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject bossScene;
    private void OnTriggerEnter(Collider other)
    {
        bossScene.SetActive(true);
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
