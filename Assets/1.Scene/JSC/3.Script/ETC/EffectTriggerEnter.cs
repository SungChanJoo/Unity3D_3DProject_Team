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
            Debug.Log("�÷��̾� ������ �Ծ���..");
            
            if(other.TryGetComponent(out PlayerData player))
            {
                player.TakeDamage(damage, 10, Vector3.zero, Vector3.zero);

            }
            else
            {
                Debug.Log("��ȿ �� �ʾ�? �÷��̾�");
            }
        }
    }
}
