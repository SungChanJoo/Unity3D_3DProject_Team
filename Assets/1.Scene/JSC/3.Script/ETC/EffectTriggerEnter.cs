using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTriggerEnter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("플레이어 데미지 입었음..");
            
            if(other.TryGetComponent(out PlayerData player))
            {
                player.TakeDamage(10f, 10, Vector3.zero, Vector3.zero);

            }
            else
            {
                Debug.Log("에효 또 너야? 플레이어");
            }
        }
    }
}
