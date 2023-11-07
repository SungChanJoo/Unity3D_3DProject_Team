using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusEffectType
{
    Poisoned,
    Paralysed,
    BlurryVision
}

public abstract class StatusEffect : IDisposable
{
    public StatusEffectType Type;

    protected bool IsOnGoing = false;

    // PlayerData 사용이 좋은 방법이 아니긴 한데 일단 적용
    protected StatusEffectedCharacter target;

    protected ParticleSystem particleSystem;

    // UI 쪽에서 icon 고르고 할당해주기
    private Sprite icon;

    private string animationTriggerName;

    private float removeTimer = 5;
    private float remainingTime = 0;
    private float checkTime = 1;

    public StatusEffect(StatusEffectType type, StatusEffectedCharacter target, string animationTriggerName)
    {
        Type = type;
        this.target = target;
        this.animationTriggerName = animationTriggerName;
    }

    public IEnumerator StartEffectRoutine()
    {
        StartEffect();

        ApplyEffect();

        while (remainingTime < removeTimer)
        {
            remainingTime += checkTime;

            // 1초마다 타이머 다 끝났나 체크
            yield return new WaitForSeconds(checkTime);
        }

        EndEffect();
    }

    private void StartEffect()
    {
        IsOnGoing = true;
        particleSystem?.Play();

        Animator animator = target.gameObject.GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.SetTrigger(animationTriggerName);
        }
    }

    private void EndEffect()
    {
        RemoveEffect();
        particleSystem?.Stop();

        //Animator animator = target.gameObject.GetComponentInChildren<Animator>();
        //if (animator != null)
        //{
        //    animator.Play("Walk Tree");
        //}
    }

    public abstract void ApplyEffect();

    public void RemoveEffect()
    {
        IsOnGoing = false;
        target.RemoveStatusEffect(this);
    }

    public void ResetTimer()
    {
        remainingTime = 0;
    }

    public void Dispose()
    {
        // Suppress finalization.
        GC.SuppressFinalize(this);
    }
}
