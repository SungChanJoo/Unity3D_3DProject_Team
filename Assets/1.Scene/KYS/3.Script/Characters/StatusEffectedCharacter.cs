using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;

public class StatusEffectedCharacter : MonoBehaviour
{
    private ObservableCollection<StatusEffect> statusEffects;

    // Player용. 일단 이거 씀
    // 몬스터 상태 이상을 구현할 경우 PlayerData를 에너미 관련 클래스로 바꾸면 됨
    [SerializeField] private PlayerData targetData;
    public PlayerData TargetData => targetData;

    // UI에서 Subscribe 해야 함. 아이콘 보여주는 용
    public Action<List<StatusEffect>> StatusEffectChangedEvent;

    private void Awake()
    {
        statusEffects = new ObservableCollection<StatusEffect>();
        statusEffects.CollectionChanged += StatusEffects_CollectionChanged;

        ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
        if (ps != null)
            ps.Stop();
    }

    public void AddStatusEffect(StatusEffectType type)
    {
        // 이미 해당 상태이상을 겪던 중이라면 새로 상태이상을 부여하는 게 아니라 리셋 타이머에 0만 할당하고 리턴
        for (int i = 0; i < statusEffects.Count; i++)
        {
            if (statusEffects[i].Type.Equals(type))
            {
                Debug.Log("Rest SE Timer: " + type);
                statusEffects[i].ResetTimer();
                return;
            }
        }

        StatusEffect se;

        switch (type)
        {
            case StatusEffectType.Poisoned:
                se = new PoisonEffect(this);
                break;
            case StatusEffectType.Paralysed:
                se = new ParalyseEffect(this);
                break;
            case StatusEffectType.Dizzy:
                se = new DizzyEffect(this);
                break;
            default:
                throw new NotImplementedException();
        }

        statusEffects.Add(se);

        StartCoroutine(se.StartEffectRoutine());
    }

    public void RemoveStatusEffect(StatusEffect se)
    {
        statusEffects.Remove(se);
    }

    private void StatusEffects_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        StatusEffectChangedEvent?.Invoke(new List<StatusEffect>(statusEffects)); // 복사본만 넘긴다.
    }
}
