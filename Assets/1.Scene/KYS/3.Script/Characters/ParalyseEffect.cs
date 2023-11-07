using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ParalyseEffect : StatusEffect
{
    public ParalyseEffect(StatusEffectedCharacter target) : base(StatusEffectType.Paralysed, target) { }

    public override void ApplyEffect()
    {
        // CameraController 쪽에서 ParalyseEffect의 저장/삭제에 따른 효과 적용
    }

    public override void CustomStartEffect()
    {
        Animator animator = target.gameObject.GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Paralysed");
        }
    }

    public override void CustomEndEffect()
    {
        Animator animator = target.gameObject.GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Default");
        }
    }
}