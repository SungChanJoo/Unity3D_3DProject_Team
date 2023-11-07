using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class PoisonEffect : StatusEffect
{
    private float damage = 5;
    private float effectTimer = 1;

    public PoisonEffect(StatusEffectedCharacter target) : base(StatusEffectType.Poisoned, target, "Attack") { }

    public override void ApplyEffect()
    {
        target.StartCoroutine(DamageRoutine());
    }

    private IEnumerator DamageRoutine()
    {
        while (IsOnGoing)
        {
            // IDamageable의 TakeDamage 메소드가 범용성이 떨어진다.
            target.TargetData.TakeDamage(damage, 0, Vector3.zero, Vector3.zero);

            // 1초마다 타이머 다 끝났나 체크
            yield return new WaitForSeconds(effectTimer);
        }
    }
}