using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyControll : MonoBehaviour, IDamageable
{
    [Header("������ ��� ���̾�")]
    public LayerMask TargetLayer;
    private Entity targetEntity;

    //��θ� ����� AI Agent 
    private NavMeshAgent agent;

    [Header("�� ����")]
    public float MaxHeath = 100f;
    public float Health { get; protected set; }
    public bool IsDead { get; protected set; }

    [SerializeField] private float damage = 20f;
    [SerializeField] private float force = 0f; // �̴���
    [SerializeField] private float timebetAttack = 0.5f; // ���ݼӵ�
    private float lastAttackTimebet;
    
    //���� �÷��̾� ������ �Ÿ�
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
        //�´� �ִϸ��̼� �߰����� ������ todo 1031
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

        //enemy�� ������ �ִ� collider ����
        Collider[] colls = GetComponents<Collider>();
    
        foreach(Collider c in colls)
        {
            c.enabled = false;
        }
        agent.isStopped = true;
        agent.enabled = false;
        //�״� �ִϸ��̼� �߰����� ������ todo 1031
    }

    //stay -> ��� ���� ��
    protected void OnTriggerStay(Collider other)
    {
        if(!IsDead && Time.time >= lastAttackTimebet + timebetAttack)
        {
            Attack(other);
        }
    }
    
    //����
    protected virtual void Attack(Collider other)
    {
        if (other.TryGetComponent(out Entity e))
        {
            if (targetEntity.Equals(e))
            {
                //���� �ִϸ��̼� �߰����� todo 1031
                lastAttackTimebet = Time.time;
                //ClosestPoint -> ��� ��ġ
                //���� �ǰ� ��ġ�� �ǰ� ���� �ٻ簪�� ���
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
                //���� ��ġ���� 20 ���������� ������ ���� ����� TargetLayer�� ���� �ݶ��̴� ����
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
