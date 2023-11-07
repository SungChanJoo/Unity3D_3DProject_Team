using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class StatusEffectedCharacter : MonoBehaviour
{
    private ObservableCollection<StatusEffect> statusEffects;

    // Player용. 일단 이거 씀
    [SerializeField] private PlayerData targetData;
    public PlayerData TargetData => targetData;

    // UI에서 Subscribe 해야 함
    public Action<List<StatusEffect>> StatusEffectChangedEvent;

    private void Awake()
    {
        statusEffects = new ObservableCollection<StatusEffect>();
        statusEffects.CollectionChanged += StatusEffects_CollectionChanged;

        ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
        if (ps != null)
            ps.Stop();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            AddStatusEffect(StatusEffectType.Poisoned);
        if (Input.GetKeyDown(KeyCode.O))
            AddStatusEffect(StatusEffectType.Paralysed);
        if (Input.GetKeyDown(KeyCode.P))
            AddStatusEffect(StatusEffectType.BlurryVision);
    }

    public void AddStatusEffect(StatusEffectType type)
    {
        for (int i = 0; i < statusEffects.Count; i++)
        {
            if (statusEffects[i].Type.Equals(type))
            {
                Debug.Log("Rest SE Timer: " + type);
                statusEffects[i].ResetTimer();
                return;
            }
        }

        StatusEffect se = null;

        switch (type)
        {
            case StatusEffectType.Poisoned:
                se = new PoisonEffect(this);
                break;
            case StatusEffectType.Paralysed:
                se = new ParalyseEffect(this);
                break;
            case StatusEffectType.BlurryVision:
                se = new BlurryVisionEffect(this);
                break;
            default:
                throw new NotImplementedException();
        }

        Debug.LogWarning("Add Status Effect: " + se.Type);
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
