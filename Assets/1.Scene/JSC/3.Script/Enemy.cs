using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("AI Ȱ��ȭ")]
    public bool isAI = true;
    [Header("������ ��� ���̾�")]
    public LayerMask TargetLayer;
    protected Entity targetEntity;

    [SerializeField] private Transform player;
    //��θ� ����� AI Agent 
    protected NavMeshAgent agent;

    [Header("�� ����")]
    public float MaxHeath = 100f;
    public float Health { get; protected set; }
    public bool IsDead { get; protected set; }

    [SerializeField] protected float damage = 20f;
    [SerializeField] protected float force = 0f; // �̴���
    [SerializeField] protected float attackDistane = 2f; //���ݹ���
    [SerializeField] protected float timebetAttack = 2.267f; // ���ݼӵ�
    [SerializeField] protected float detectPlayerRange = 5f; // �÷��̾� Ž�� ����
    protected float lastAttackTimebet;

    [Header("ETC")]
    [SerializeField] protected Slider hpSlider;
    [SerializeField] protected GameObject weapon;
    [SerializeField] protected GameObject[] wayPoint;
    protected Animator enemyAni;
    protected Rigidbody enemyRigid;
    protected bool isAttack = false;
    protected bool isAttackTime = false; //Į�� ����ĥ �� �������� �ް� ����
    public event Action OnDead;


    protected virtual void OnEnable()
    {
        IsDead = false;
        Health = MaxHeath;
    }

    protected virtual void Awake()
    {
        TryGetComponent(out agent);
        TryGetComponent(out enemyAni);
        TryGetComponent(out enemyRigid);
        hpSlider.value = MaxHeath;
    }



    public virtual void TakeDamage(float damage, float knockBack, Vector3 hitposition, Vector3 hitNomal)
    {

        Health -= damage;
        hpSlider.value = Health;
        Debug.Log("�� ������..");
        if (Health <= 0 && !IsDead)
        {
            Die();
        }
    }
    
    public virtual void Die()
    {
        Debug.Log("������");

        if (OnDead != null)
        {
            OnDead();
        }
        IsDead = true;

        //enemy�� ������ �ִ� collider ����
        Collider[] colls = GetComponents<Collider>();
    
        foreach(Collider c in colls)
        {
            c.enabled = false;
        }

        agent.isStopped = true;
        agent.enabled = false;
        enemyAni.SetTrigger("Death");
    }

}
