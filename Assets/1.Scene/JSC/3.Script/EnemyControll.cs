using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyControll : MonoBehaviour, IDamageable
{
    public bool isAI = true;
    [Header("추적할 대상 레이어")]
    public LayerMask TargetLayer;
    private Entity targetEntity;

    [SerializeField] private Transform player;
    //경로를 계산할 AI Agent 
    private NavMeshAgent agent;

    [Header("적 세팅")]
    public float MaxHeath = 100f;
    public float Health { get; protected set; }
    public bool IsDead { get; protected set; }

    [SerializeField] private float damage = 20f;
    [SerializeField] private float force = 0f; // 미는힘
    [SerializeField] private float attackDistane = 3f; //공격범위
                                                       //  private float lastAttackTimebet;
                                                       //    [SerializeField] private float timebetAttack = 2.15f; // 공격속도
    [SerializeField] private Slider hpSlider;
    private Animator enemyAni;
    private Rigidbody enemyRigid;
    public event Action OnDead;
    private bool isAttack = false;
    private bool isAttackTime = false; //칼이 내리칠 때 데미지를 받게 만듬
    [SerializeField] private GameObject sword;
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
        TryGetComponent(out enemyRigid);
        sword.GetComponent<BoxCollider>().enabled = false;
        hpSlider.value = MaxHeath;
    }



    public virtual void TakeDamage(float damage, float knockBack, Vector3 hitposition, Vector3 hitNomal)
    {
        //맞는 애니메이션 추가해줘 성찬아 todo 1031
        Health -= damage;
        hpSlider.value = Health;
        Debug.Log("나 아프다..");
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

    protected void OnTriggerEnter(Collider other)
    {
        if (!IsDead)
        {
            if (other.TryGetComponent(out Entity e))
            {
                if (targetEntity.Equals(e))
                {
                    //공격 애니메이션 추가해줘 todo 1031
                    //ClosestPoint -> 닿는 위치
                    //상대방 피격 위치와 피격 방향 근사값을 계산
                    Vector3 hitPoint = other.ClosestPoint(transform.position);
                    Vector3 hitNormal = transform.position - other.transform.position;
                    e.TakeDamage(damage, force, hitPoint, hitNormal);
                }
            }
        }

    }
    
    IEnumerator DelayAttack_co()
    {
        if (isAttack)
        {
            agent.SetDestination(transform.position);
            for(int i = 0; i < 10; i++)
            {

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(player.transform.position), 0.2f);
                yield return null;
            }
            yield return new WaitForSeconds(0.4f);
            sword.GetComponent<BoxCollider>().enabled = true;

        }
        yield return new WaitForSeconds(1.10f-0.4f);
        sword.GetComponent<BoxCollider>().enabled = false;

        yield return new WaitForSeconds(0.9f);
        isAttack = false;
        agent.isStopped = false;
        enemyAni.SetBool("isMove", true);
    }

    private IEnumerator UpdataTargetPosition()
    {

        while (!IsDead)
        {
            if(isTarget )
            {
                if(isAttack)
                {
                    agent.isStopped = true;

                }
                else
                {
                    agent.isStopped = false;

                }

                agent.SetDestination(targetEntity.transform.position + Vector3.forward * attackDistane);

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
/*    void FreezeVelocity()
    {
        enemyRigid.velocity = Vector3.zero;
        enemyRigid.angularVelocity = Vector3.zero;


    }*/
    private void Start()
    {
        if(isAI)
            StartCoroutine(UpdataTargetPosition());
    }
    private void Update()
    {
        if(isAI)
        {
            enemyAni.SetBool("HasTarget", isTarget);
            if (isTarget)
            {
                float distance = Vector3.Distance(targetEntity.transform.position, transform.position);
                if (distance <= attackDistane + 0.2f && !isAttack)
                {

                    isAttack = true;
                    enemyAni.SetTrigger("Attack");

                    StartCoroutine(DelayAttack_co());
                }
                else
                {
                    enemyAni.SetBool("isMove", false);
                }
            }
        }


    }
}
