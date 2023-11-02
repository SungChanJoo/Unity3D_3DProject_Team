using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyControll : MonoBehaviour, IDamageable
{
    [Header("AI 활성화")]
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
    [SerializeField] private float attackDistane = 2f; //공격범위
    [SerializeField] private float timebetAttack = 2.267f; // 공격속도
    [SerializeField] private float startAttackTime = 0.3f; // 공격속도
    [SerializeField] private float endAttackTime = 1.5f; // 공격속도
    private float lastAttackTimebet;
    [SerializeField] private Slider hpSlider;
    private Animator enemyAni;
    private Rigidbody enemyRigid;
    private bool isAttack = false;
    private bool isAttackTime = false; //칼이 내리칠 때 데미지를 받게 만듬
    [SerializeField] private GameObject weapon;

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
        TryGetComponent(out enemyRigid);
        weapon.GetComponent<BoxCollider>().enabled = false;
        hpSlider.value = MaxHeath;
    }



    public virtual void TakeDamage(float damage, float knockBack, Vector3 hitposition, Vector3 hitNomal)
    {
        enemyAni.SetTrigger("TakeDamage");
        transform.LookAt(targetEntity.transform.position);
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
        enemyAni.SetTrigger("Death");
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (!IsDead && Time.time >= lastAttackTimebet + timebetAttack)
        {
            lastAttackTimebet = Time.time;

            if (other.TryGetComponent(out Entity e))

            {
                if (targetEntity.Equals(e))
                {
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
        float aniTime = timebetAttack;
        yield return new WaitForSeconds(startAttackTime); //공격 시작시 무기 콜라이더 활성화
        aniTime -= startAttackTime;
        weapon.GetComponent<BoxCollider>().enabled = true;

        yield return new WaitForSeconds(timebetAttack - endAttackTime); //공격 끝나면 무기 콜라이더 비활성화
        aniTime -= endAttackTime;
        weapon.GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(aniTime); //남은 애니메이션 재생

        isAttack = false;
        enemyAni.SetBool("isAttack", false);

    }

    private IEnumerator UpdataTargetPosition()
    {
        RaycastHit raycastHit;
        Ray ray;
        while (!IsDead)
        {
            if(isTarget)
            {
                ray = new Ray(transform.position + new Vector3(0, 1f, 0), transform.forward );

                Debug.DrawRay(transform.position + new Vector3(0, 1f, 0), transform.forward * attackDistane, Color.red);
                //float distance = Vector3.Distance(targetEntity.transform.position, transform.position);
                //거리가 공격범위보다 가깝고 공격중이 아닐때 //, 10f, LayerMask.NameToLayer("Player")
                if (Physics.Raycast(ray, out raycastHit, attackDistane, TargetLayer) && !isAttack )
                {

                    //Debug.Log(raycastHit.transform.gameObject);

                    isAttack = true;
                    agent.isStopped = true;

                    enemyAni.SetTrigger("Attack");
                    enemyAni.SetBool("isAttack", true);

                    StartCoroutine(DelayAttack_co());
                }
                else if(!isAttack)
                {

                    agent.isStopped = false;
                    agent.SetDestination(targetEntity.transform.position);
                }
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
        }


    }
}
