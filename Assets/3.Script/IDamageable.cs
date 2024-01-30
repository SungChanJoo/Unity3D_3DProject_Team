using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    //매개변수 피해량, 밀리는 힘, 맞은 위치, 맞은 각도
    public void TakeDamage(float damage, float knockBack, Vector3 hitposition, Vector3 hitNomal);
}
