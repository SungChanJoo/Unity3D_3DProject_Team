using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class PoisonEffect : StatusEffect
{
    private ParticleSystem particleSystem;

    private float damage = 5;
    private float effectTimer = 1;

    public PoisonEffect(StatusEffectedCharacter target) : base(StatusEffectType.Poisoned, target)
    {
        particleSystem = target.gameObject.GetComponentInChildren<ParticleSystem>();
        particleSystem?.Stop();
    }

    public override void ApplyEffect()
    {
        target.StartCoroutine(DamageRoutine());
    }

    private IEnumerator DamageRoutine()
    {
        while (IsOnGoing)
        {
            target.TargetData.TakeDamage(damage);

            // 1초마다 타이머 다 끝났나 체크
            yield return new WaitForSeconds(effectTimer);
        }
    }

    public override void CustomStartEffect()
    {
        particleSystem?.Play();
    }

    public override void CustomEndEffect()
    {
        particleSystem?.Stop();
    }
}