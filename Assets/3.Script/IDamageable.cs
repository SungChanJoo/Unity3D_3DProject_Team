using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(float damage, float knockBack, Vector3 hitposition, Vector3 hitNomal);
}
