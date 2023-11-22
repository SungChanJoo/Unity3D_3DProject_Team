using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;

public class StatusEffectedCharacter : MonoBehaviour
{
    private List<StatusEffect> statusEffects;

    // Player용. 일단 이거 씀
    // 몬스터 상태 이상을 구현할 경우 PlayerData를 에너미 관련 클래스로 바꾸면 됨
    [SerializeField] private PlayerData targetData;
    public PlayerData TargetData => targetData;

    private void Awake()
    {
        statusEffects = new List<StatusEffect>();

        GetComponentInChildren<ParticleSystem>()?.Stop();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            AddStatusEffect(StatusEffectType.Paralysed);
        if (Input.GetKeyDown(KeyCode.O))
            AddStatusEffect(StatusEffectType.Dizzy);
        if (Input.GetKeyDown(KeyCode.P))
            AddStatusEffect(StatusEffectType.Poisoned);
    }

    public bool CheckIfTargetDead() => TargetData.IsDead;

    public void AddStatusEffect(StatusEffectType type)
    {
        // 이미 해당 상태이상을 겪던 중이라면 새로 상태이상을 부여하는 게 아니라 리셋 타이머에 0만 할당하고 리턴
        for (int i = 0; i < statusEffects.Count; i++)
        {
            if (statusEffects[i].Type.Equals(type))
            {
                Debug.Log("Reset SE Timer: " + type);
                statusEffects[i].ResetTimer();
                return;
            }
        }

        StatusEffect se = type switch
        {
            StatusEffectType.Poisoned => new PoisonEffect(this),
            StatusEffectType.Paralysed => new ParalyseEffect(this),
            StatusEffectType.Dizzy => new DizzyEffect(this),
            _ => throw new NotImplementedException()
        };

        statusEffects.Add(se);

        StartCoroutine(se.StartEffectRoutine());
    }

    public void RemoveStatusEffect(StatusEffect se)
    {
        statusEffects.Remove(se);
    }
}
