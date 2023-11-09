using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// Scriptable Object로 만드는 것도 좋아보임
class Sword : WeaponBase
{
    protected override float AttackDamage => 10;

    protected override float ChargeAttackDamage => 20;

    protected override float Skill1Damage => 30;

    protected override float Skill2Damage => 5;
}

//class Sword : MonoBehaviour, IWeapon
//{
//    public void Attack()
//    {
//        Debug.Log("Attacked test");
//    }

//    public void ChargeAttack()
//    {
//        Debug.Log("Charge Attacked test");
//    }

//    // quick test
//    private void OnTriggerEnter(Collider other)
//    {
//        if(other.TryGetComponent(out IDamageable target)
//            && other.name.Contains("Enemy"))
//            // 이름 비교는 임시. EnemyControl 가지고 있나 없나로 비교할 가능성 높음
//        {
//            target.TakeDamage(10, 10, Vector3.zero, Vector3.zero);
//        }
//    }

//    public void Drop()
//    {
//        throw new NotImplementedException();
//    }

//    public void Parry()
//    {
//        throw new NotImplementedException();
//    }

//    public void Skill1()
//    {
//        throw new NotImplementedException();
//    }

//    public void Skill2()
//    {
//        throw new NotImplementedException();
//    }

//    public void Store()
//    {
//        throw new NotImplementedException();
//    }

//    public void Use(PlayerData player)
//    {
//        // ChangeCurrentWeapon용으로 사용할 수도 있고,
//        // IITem에서 Use를 아예 뺄 수도 있음.
//    }
//}