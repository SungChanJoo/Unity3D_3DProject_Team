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

    // PlayerData ����� ���� ����� �ƴϱ� �ѵ� �ϴ� ����
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

            // 1�ʸ��� Ÿ�̸� �� ������ üũ
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
