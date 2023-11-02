using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour, IItem
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable target)
            && other.name.Contains("Enemy"))
        // 이름 비교는 임시. EnemyControl 가지고 있나 없나로 비교할 가능성 높음
        {
            target.TakeDamage(10, 10, Vector3.zero, Vector3.zero);
        }
    }

    public abstract void Attack();

    public abstract void ChargeAttack();

    public abstract void Parry();

    public abstract void Skill1();

    public abstract void Skill2();

    public void Drop()
    {
        throw new NotImplementedException();
    }

    public void Store()
    {
        throw new NotImplementedException();
    }

    public void Use(PlayerData player)
    {
        throw new NotImplementedException();
    }
}
