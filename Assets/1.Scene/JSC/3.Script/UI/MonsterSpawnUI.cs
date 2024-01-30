using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnUI : MonoBehaviour
{
    //[SerializeField] private Transform spawnButtonPos;
    [SerializeField] private Transform spawnPos;
    [SerializeField] private GameObject enemy;
    //[SerializeField] private GameObject Spanwbutton;

    public void SpanwEnemy()
    {
        Instantiate(enemy, spawnPos.position, Quaternion.Euler(0,180f,0));
       // Instantiate(Spanwbutton, spawnButtonPos.position, Quaternion.identity);
    }
}
