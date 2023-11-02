using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemy : MonoBehaviour, IDamageable
{
    public void TakeDamage(float damage, float knockBack, Vector3 hitposition, Vector3 hitNomal)
    {
        Debug.Log("Dummy Enemy Took Damage");
    }
}
