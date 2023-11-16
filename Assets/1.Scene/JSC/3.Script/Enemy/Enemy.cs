using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("AI 활성화")]
    public bool isAI = true;
    [Header("추적할 대상 레이어")]
    public LayerMask TargetLayer;
    protected PlayerData player;

    //경로를 계산할 AI Agent 
    protected NavMeshAgent agent;

    [Header("적 세팅")]
    public float MaxHealth = 100f;
    public float Health { get; protected set; }
    public bool IsDead { get; protected set; }

    [SerializeField] protected float damage = 20f;
    [SerializeField] protected float force = 0f; // 미는힘
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected float attackDistance = 2f; //공격범위
    [SerializeField] protected float timebetAttack = 2.267f; // 공격속도
    [SerializeField] protected float detectRange = 5f; // 플레이어 탐지 범위
    protected float lastAttackTimebet;

    [Header("ETC")]
    [SerializeField] protected Slider hpSlider;
    [SerializeField] protected GameObject weapon;
    protected Animator enemyAni;
    protected Rigidbody enemyRigid;
    protected bool isAttack = false;
    protected bool isAttackTime = false; //칼이 내리칠 때 데미지를 받게 만듬
    public event Action OnDead;


    protected virtual void OnEnable()
    {
        IsDead = false;
        Health = MaxHealth;
        hpSlider.maxValue = MaxHealth;
        hpSlider.value = Health;
    }

    protected virtual void Awake()
    {
        TryGetComponent(out agent);
        TryGetComponent(out enemyAni);
        TryGetComponent(out enemyRigid);
    }



    public virtual void TakeDamage(float damage, float knockBack, Vector3 hitposition, Vector3 hitNomal)
    {

        Health -= damage;
        hpSlider.value = Health;
        Debug.Log(damage+"받음 나 아프다..");
        if (Health <= 0 && !IsDead)
        {
            Die();
        }
    }
    
    public virtual void Die()
    {
        Debug.Log("깨꼬닭");

        if (OnDead != null)
        {
            OnDead();
            Debug.Log("죽음 이벤트 호출!");
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
        enemyAni.SetTrigger("Death");

        Destroy(gameObject, 2f);
    }

}
