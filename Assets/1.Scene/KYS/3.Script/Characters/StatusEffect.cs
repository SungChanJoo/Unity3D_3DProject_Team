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

    // PlayerData ����� ���� ����� �ƴϱ� �ѵ� �ϴ� ����
    protected StatusEffectedCharacter target;

    // UI �ʿ��� icon ���� �Ҵ����ֱ�
    private Sprite icon;
    
    // effect

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
        IsOnGoing = true;

        ApplyEffect();

        while (remainingTime < removeTimer)
        {
            remainingTime += checkTime;

            // 1�ʸ��� Ÿ�̸� �� ������ üũ
            yield return new WaitForSeconds(checkTime);
        }

        RemoveEffect();
        
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
