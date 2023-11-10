using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    protected abstract float AttackDamage { get; }
    protected abstract float ChargeAttackDamage { get; }
    protected abstract float Skill1Damage { get; }
    protected abstract float Skill2Damage { get; }

    private float currentDamage;

    private bool canDamageEnemy;
    public bool CanDamageEnemy
    {
        get => canDamageEnemy;
        private set => canDamageEnemy = value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CanDamageEnemy && other.TryGetComponent(out Enemy target))
        {
            target.TakeDamage(currentDamage, 10, Vector3.zero, Vector3.zero);
            DisableDamaging();
        }
    }

    // 어떤 공격이든, 공격 키 누를 경우 사용
    public void EnableDamaging()
    {
        CanDamageEnemy = true;
    }

    // 1. 공격 애니메이션이 다 끝났을 때 부름
    // 2. 공격 키를 누른 뒤 대미지를 에너미에게 입혔는데, 새로운 공격 키 입력이 없었음에도 에너미에게 다시 대미지를 입힐 수 있는 경우 부름
        // ex) 칼이 에너미를 한 번 치고(대미지 입힘) 통과했다가 플레이어가 칼을 바로 잡는 동안 우연히 에너미에게 칼이 한 번 더 닿음 (이 경우 대미지 안 입게 함)
    // 해당 메소드를 구현 안 할 경우 2번의 예시와 같은 경우가 발생할 수도 있고, 플레이어가 걷고 있을 때 우연히 칼이 에너미에게 닿기만 해도 에너미는 대미지를 입을 수 있다.
    public void DisableDamaging()
    {
        CanDamageEnemy = false;
    }

    public virtual void Attack()
    {
        currentDamage = AttackDamage;
        EnableDamaging();
    }

    public virtual void ChargeAttack()
    {
        currentDamage = ChargeAttackDamage;
        EnableDamaging();
    }

    public virtual void Parry()
    {
        // 패링은 아래 두 줄을 쓸라나 안 쓸라나 모르겠다.
        // EnableDamaging();
        // currentDamage = attackDamage;
    }

    public virtual void Skill1()
    {
        currentDamage = Skill1Damage;
        EnableDamaging();
    }

    // Option 1
    public void AdditionalAttack(float damage)
    {
        currentDamage = damage;
        EnableDamaging();
    }

    // Option 2
    public void AdditionalSkill2()
    {
        // 이 메소드를 사용할 경우 원하는 대미지 값으로 수정 요함
        currentDamage = 40;
        EnableDamaging();
    }

    public virtual void Skill2()
    {
        currentDamage = Skill2Damage;
        EnableDamaging();
    }
}
