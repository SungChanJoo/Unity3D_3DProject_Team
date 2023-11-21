using UnityEngine;

class ParalyseEffect : StatusEffect
{
    public ParalyseEffect(StatusEffectedCharacter target) : base(StatusEffectType.Paralysed, target) { }

    public override void CustomStartEffect()
    {
        Animator animator = target.gameObject.GetComponentInChildren<Animator>();
        if (animator != null)
            animator.SetTrigger("Paralysed");

        if (target.TryGetComponent(out CameraController player))
            player.isParalysed = true;
    }

    public override void CustomEndEffect()
    {
        Animator animator = target.gameObject.GetComponentInChildren<Animator>();
        if (animator != null)
            animator.SetTrigger("Default");

        if (target.TryGetComponent(out CameraController player))
            player.isParalysed = false;
    }
}