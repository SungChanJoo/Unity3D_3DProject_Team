using System.Collections;
using UnityEngine;

public enum StatusEffectType
{
    Poisoned,
    Paralysed,
    Dizzy
}

public abstract class StatusEffect
{
    public StatusEffectType Type;

    protected StatusEffectedCharacter target;

    protected bool IsOnGoing = false;

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

            if (target.CheckIfTargetDead())
            {
                EndEffect();
                yield break;
            }

            yield return new WaitForSeconds(checkTime); // 1초마다 타이머 다 끝났나 체크
        }

        EndEffect();
    }

    private void StartEffect()
    {
        target.TargetData.playerStateUI.ViewStateUI(Type, removeTimer);
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
    public virtual void ApplyEffect() { }

    // 새로 공격 받을 시 갱신
    public void ResetTimer()
    {
        target.TargetData.playerStateUI.ViewStateUI(Type, removeTimer);
        remainingTime = 0;
    }
}
