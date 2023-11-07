using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

/*public enum State
{
    Idle = 0,
    Chase,
    Attack,
    Patroll,
    Die
}*/

public class AnyMonster : Enemy
{
    public List<GameObject> wayPoint;
    [SerializeField] private EnemyData enemyData;
    private bool isPatroll = true;
    //private bool isMiss = false;
    //protected State state;
    private bool isTarget
    {
        get
        {
            if (targetEntity != null && !targetEntity.IsDead && 
                Vector3.SqrMagnitude(targetEntity.transform.position - transform.position) < 150f) // 플레이어 거리가 탐지 범위안에 있을 때  
            {
                isPatroll = false;
                return true;
            }
            targetEntity = null;
            isPatroll = true;
            return false;

        }
    }

    private void SetUp()
    {
        MaxHealth = enemyData.MaxHealth;
        damage = enemyData.Damage;
        force = enemyData.Force;
        speed = enemyData.Speed;
        attackDistance = enemyData.AttackDistance;
        timebetAttack = enemyData.TimegetAttack;
        detectRange = enemyData.DetectRange;
    }
    protected override void Awake()
    {
        SetUp();
        base.Awake();
        agent.avoidancePriority = UnityEngine.Random.Range(0, 100);
        weapon.GetComponent<BoxCollider>().enabled = false;
    }



    public override void TakeDamage(float damage, float knockBack, Vector3 hitposition, Vector3 hitNomal)
    {
        enemyAni.SetTrigger("TakeDamage");
        transform.LookAt(targetEntity.transform.position);
        base.TakeDamage(damage, knockBack, hitposition, hitNomal);
    }
    
    public override void Die()
    {
        base.Die();
        //나중에 해줘..!
    }

    protected void OnTriggerEnter(Collider other)
    {
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
    
    void OnStartAttack()
    {
        weapon.GetComponent<BoxCollider>().enabled = true;
    }
    void OnEndAttack()
    {
        weapon.GetComponent<BoxCollider>().enabled = false;
    }
    void OnEndAni()
    {
        StartCoroutine(DelayAttack_co());
    }
    IEnumerator DelayAttack_co()
    {
        yield return new WaitForSeconds(timebetAttack);
        isAttack = false;
        agent.isStopped = false;
        enemyAni.SetBool("isMove", !isAttack);
        Debug.Log("move는 " + !isAttack);
    }
    private IEnumerator UpdataTargetPosition()
    {
        RaycastHit raycastHit;
        Ray ray;
        while (!IsDead)
        {
            hpSlider.value = Health; // 왜 슬라이더 값이 변하는 지 모르겠네...

            if (isTarget)
            {           
                ray = new Ray(transform.position + new Vector3(0, 1f, 0), transform.forward );

                Debug.DrawRay(transform.position + new Vector3(0, 1f, 0), transform.forward * attackDistance, Color.red);
                //float distance = Vector3.Distance(targetEntity.transform.position, transform.position);
                //거리가 공격범위보다 가깝고 공격중이 아닐때 //, 10f, LayerMask.NameToLayer("Player")
                if (Physics.Raycast(ray, out raycastHit, attackDistance, TargetLayer))
                {
                    if (!IsDead && Time.time >= lastAttackTimebet && !isAttack)
                    {
                        lastAttackTimebet = Time.time;
                        lastAttackTimebet += timebetAttack;
                        agent.isStopped = true;

                        isAttack = true;
                        enemyAni.SetBool("isMove", !isAttack);
                        enemyAni.SetTrigger("Attack");
                    }
                    //Debug.Log(raycastHit.transform.gameObject);
                }
                else if(!isAttack )
                {
                    agent.SetDestination(targetEntity.transform.position);

                }
                //isMiss = true;
            }
            else
            {

                //agent.isStopped = true;
                //현재 위치에서 20 반지름으로 가상의 원을 만들어 TargetLayer를 가진 콜라이더 추출
                Collider[] coll = Physics.OverlapSphere(transform.position + transform.forward * (detectRange-1f), detectRange, TargetLayer);

                for (int i = 0; i < coll.Length; i++)
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

    void Patroll()
    {
        enemyAni.SetBool("isPatrolling", isPatroll);

        //플레이어를 놓치면 바로 순찰 아니면 플레이어의 마지막 위치까지 이동한 뒤에 순찰..
/*        if (isMiss)
        {
            agent.SetDestination(wayPoint[UnityEngine.Random.Range(0, wayPoint.Length)].transform.position);
            isMiss = false;
        }*/
            
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(PatrollDelay_co());
            //StartCoroutine(PatrollDelay_co());

        }

    }
    IEnumerator PatrollDelay_co()
    {
        enemyAni.SetBool("isPatrolling", false);

        yield return new WaitForSeconds(3f);
        agent.SetDestination(wayPoint[UnityEngine.Random.Range(0, wayPoint.Count)].transform.position);


    }
    /*    void Chase()
        {

        }
        void ActionState(State state)
        {

        }*/
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + transform.forward * (detectRange - 1f), detectRange);
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

            if (isPatroll)
            {
                Patroll();
            }

        }


    }
}
