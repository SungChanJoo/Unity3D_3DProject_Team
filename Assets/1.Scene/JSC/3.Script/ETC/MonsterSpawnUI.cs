using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnUI : MonoBehaviour
{
    [SerializeField] private Transform spawnPos;
    [SerializeField] private GameObject enemy;
    public void SpanwEnemy()
    {
        Instantiate(enemy, spawnPos.position, Quaternion.identity);
    }
}
