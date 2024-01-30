using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTriggerEnter : MonoBehaviour
{
    public float damage = 10f;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("플레이어 데미지 입었음..");
            
            if(other.TryGetComponent(out PlayerData player))
            {
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 hitNormal = transform.position - other.transform.position;
                player.TakeDamage(damage, 10, hitPoint, hitNormal);

            }
            else
            {
                Debug.Log("에효 또 너야? 플레이어");
            }
        }
    }
}
