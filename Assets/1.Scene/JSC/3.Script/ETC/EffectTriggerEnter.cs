using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTriggerEnter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("�÷��̾� ������ �Ծ���..");
            
            if(other.TryGetComponent(out PlayerData player))
            {
                player.TakeDamage(10f, 10, Vector3.zero, Vector3.zero);

            }
            else
            {
                Debug.Log("��ȿ �� �ʾ�? �÷��̾�");
            }
        }
    }
}
