using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    //�Ű����� ���ط�, �и��� ��, ���� ��ġ, ���� ����
    public void TakeDamage(float damage, float knockBack, Vector3 hitposition, Vector3 hitNomal);
}
