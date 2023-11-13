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

public abstract class StatusEffect
{
    public StatusEffectType Type;

    protected bool IsOnGoing = false;

    // PlayerData 사용이 좋은 방법이 아니긴 한데 일단 적용
    protected StatusEffectedCharacter target;

    private float removeTimer = 5;
    private float remainingTime = 0;
    private float checkTime = 1;

    public StatusEffect(StatusEffectType type, StatusEffectedCharacter target)
    {
        Type = type;
        this.target = target;
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
        CustomStartEffect();
    }

    private void EndEffect()
    {
        IsOnGoing = false;
        target.RemoveStatusEffect(this);
        CustomEndEffect();
    }

    public virtual void CustomStartEffect() { }
    public virtual void CustomEndEffect()  { }

    public abstract void ApplyEffect();

    public void ResetTimer()
    {
        remainingTime = 0;
    }
}
