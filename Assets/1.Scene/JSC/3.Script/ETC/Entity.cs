using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IDamageable
{
    public bool IsDead = false;

    public void TakeDamage(float damage, float knockBack, Vector3 hitposition, Vector3 hitNomal)
    {
        Debug.Log(damage + "������ �Դ� ��...");
    }
}
