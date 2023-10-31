using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyControll : MonoBehaviour, IDamageable
{
    [Header("추적할 대상 레이어")]
    public LayerMask TargetLayer;
    private Entity targetEntity;

    //경로를 계산할 AI Agent 
    private NavMeshAgent agent;

    [Header("적 세팅")]
    public float MaxHeath = 100f;
    public float Health { get; protected set; }
    public bool IsDead { get; protected set; }

    [SerializeField] private float damage = 20f;
    [SerializeField] private float force = 0f; // 미는힘
    [SerializeField] private float timebetAttack = 0.5f; // 공격속도
    private float lastAttackTimebet;
    
    //적과 플레이어 사이의 거리
    [SerializeField] private float enemybetPlayer = 1f;
    private Animator enemyAni;

    public event Action OnDead;
    private bool isTarget
    {
        get
        {
            if (targetEntity != null && !targetEntity.IsDead)
            {
                return true;
            }
            return false;
        }
    }

    protected virtual void OnEnable()
    {
        IsDead = false;
        Health = MaxHeath;
    }

    protected virtual void Awake()
    {
        TryGetComponent(out agent);
        TryGetComponent(out enemyAni);
    }



    public virtual void TakeDamage(float damage, float knockBack, Vector3 hitposition, Vector3 hitNomal)
    {
        //맞는 애니메이션 추가해줘 성찬아 todo 1031
        Health -= damage;

        if(Health <= 0 && !IsDead)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        if(OnDead != null)
        {
            OnDead();
        }
        IsDead = true;

        //enemy가 가지고 있는 collider 전부
        Collider[] colls = GetComponents<Collider>();
    
        foreach(Collider c in colls)
        {
            c.enabled = false;
        }
        agent.isStopped = true;
        agent.enabled = false;
        //죽는 애니메이션 추가해줘 성찬아 todo 1031
    }

    //stay -> 닿고 있을 때
    protected void OnTriggerStay(Collider other)
    {
        if(!IsDead && Time.time >= lastAttackTimebet + timebetAttack)
        {
            Attack(other);
        }
    }
    
    //공격
    protected virtual void Attack(Collider other)
    {
        if (other.TryGetComponent(out Entity e))
        {
            if (targetEntity.Equals(e))
            {
                //공격 애니메이션 추가해줘 todo 1031
                lastAttackTimebet = Time.time;
                //ClosestPoint -> 닿는 위치
                //상대방 피격 위치와 피격 방향 근사값을 계산
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 hitNormal = transform.position - other.transform.position;
                e.TakeDamage(damage, force, hitPoint, hitNormal);
            }
        }
    }
    protected IEnumerator UpdataTargetPosition()
    {
        while(!IsDead)
        {
            
            if(isTarget)
            {
                agent.isStopped = false;
                agent.SetDestination(targetEntity.transform.position - transform.forward * enemybetPlayer);
            }
            else
            {
                agent.isStopped = true;
                //현재 위치에서 20 반지름으로 가상의 원을 만들어 TargetLayer를 가진 콜라이더 추출
                Collider[] coll = Physics.OverlapSphere(transform.position, 20f, TargetLayer);
                for(int i = 0; i < coll.Length; i++)
                {
                    if(coll[i].TryGetComponent(out Entity e))
                    {
                        if(!e.IsDead)
                        {
                            targetEntity = e;
                            break;
                        }
                    }
                }
            }
            yield return null;
        }
    }
    private void Start()
    {
        StartCoroutine(UpdataTargetPosition());
    }
    private void Update()
    {
        enemyAni.SetBool("HasTarget", isTarget);
    }
}
