using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.VFX;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("AI Ȱ��ȭ")]
    public bool isAI = true;
    [Header("������ ��� ���̾�")]
    public LayerMask TargetLayer;
    protected PlayerData player;

    //��θ� ����� AI Agent 
    protected NavMeshAgent agent;

    [Header("�� ����")]
    public float MaxHealth = 100f;
    public float Health { get; protected set; }
    public bool IsDead { get; protected set; }

    protected float damage = 20f;
    protected float force = 0f; // �̴���
    protected float speed = 2f;
    protected float attackDistance = 2f; //���ݹ���
    protected float timebetAttack = 2.267f; // ���ݼӵ�
    protected float detectRange = 5f; // �÷��̾� Ž�� ����
    protected float lastAttackTimebet;

    [Header("ETC")]
    [SerializeField] protected Slider hpSlider;
    [SerializeField] protected GameObject weapon;
    [SerializeField] protected VisualEffect hitEffect;
    protected Animator enemyAni;
    protected Rigidbody enemyRigid;
    protected bool isAttack = false;
    protected bool isAttackTime = false; //Į�� ����ĥ �� �������� �ް� ����
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
        hitEffect.Stop();

    }



    public virtual void TakeDamage(float damage, float knockBack, Vector3 hitposition, Vector3 hitNomal)
    {

        Health -= damage;
        hpSlider.value = Health;

        if(hitEffect == null)
        {
            Debug.Log("�� ~ �����~");
        }
        hitEffect.transform.position = hitposition;
        hitEffect.transform.rotation = Quaternion.LookRotation(hitNomal);
        hitEffect.Play();

        Debug.Log(damage+"���� �� ������..");
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
            Debug.Log("���� �̺�Ʈ ȣ��!");
        }
        IsDead = true;

        //enemy�� ������ �ִ� collider ����
        Collider[] colls = GetComponents<Collider>();
    
        foreach(Collider c in colls)
        {
            c.enabled = false;
        }

        agent.enabled = false;
        enemyAni.SetTrigger("Death");
    }

}
