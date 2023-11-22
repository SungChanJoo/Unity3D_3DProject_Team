using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

class PoisonEffect : StatusEffect
{
    private VisualEffect particleSystem = null;

    private float damage = 5;
    private float effectTimer = 1;

    public PoisonEffect(StatusEffectedCharacter target) : base(StatusEffectType.Poisoned, target)
    {
        Transform container = target.gameObject.GetComponentsInChildren<Transform>(true).Where(x => x.CompareTag("StatusEffect")).FirstOrDefault();

        if (container == null)
            throw new System.Exception("Poison Particle System을 찾을 수 없음");

        container.gameObject.SetActive(true);
        particleSystem = container.gameObject.GetComponentInChildren<VisualEffect>();
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

            yield return new WaitForSeconds(effectTimer); // 1초마다 타이머 다 끝났나 체크
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