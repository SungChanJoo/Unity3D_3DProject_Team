using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum State
{
    Idle = 0,
    Short,
    Middle,
    Long
}

public class Boss : Enemy
{
    [Header("Boss 세팅")]
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private float longDetectRange = 20f;
    [SerializeField] private float middleDetectRange = 10f;
    [SerializeField] private float shortDetectRange = 5f;
    [SerializeField] private float jumpPower = 5f;
    [SerializeField] private float nextBehaviorTimebet = 10f;
    private float lastBehaviorTime;
    private Rigidbody enemyR;

    RaycastHit raycastHit;
    Ray centerRay;
    Ray rightRay;
    Ray leftRay;

    State bossState =0;

    //private bool isBehavior = false;


    private bool isTarget
    {
        get
        {
            if (targetEntity != null && !targetEntity.IsDead) // 플레이어 거리가 탐지 범위안에 있을 때  
            {
                return true;
            }
            targetEntity = null;
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
        enemyR = GetComponent<Rigidbody>();
        GetComponent<BoxCollider>().enabled = false;
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
        GetComponent<BoxCollider>().enabled = false;

        yield return new WaitForSeconds(timebetAttack);
        isAttack = false;
        
        //AI 활성화
        agent.enabled = true;
        agent.isStopped = false;

        //상태 초기화
        bossState = State.Idle;
        enemyAni.SetBool("isMove", !isAttack);
        agent.speed = enemyData.Speed;

        lastBehaviorTime = Time.time;
        lastBehaviorTime += nextBehaviorTimebet;
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

    public bool DetectPlayer(float detectRange)
    {
        Collider[] coll = Physics.OverlapSphere(transform.position, detectRange, TargetLayer);
        for (int i = 0; i < coll.Length; i++)
        {
            if (coll[i].TryGetComponent(out Entity e))
            {
                if (!e.IsDead)
                {
                    return true;
                }
            }
        }
        return false;
    }
    private IEnumerator UpdataTargetPosition()
    {

        while (!IsDead)
        {
            hpSlider.value = Health; // 왜 슬라이더 값이 변하는 지 모르겠네...

            if (isTarget)
            {
                centerRay = new Ray(transform.position + new Vector3(0, 1f, 0), transform.forward);
                rightRay = new Ray(transform.position + new Vector3(-1f, 1f, 0), transform.forward);
                leftRay = new Ray(transform.position + new Vector3(1f, 1f, 0), transform.forward);
                Debug.DrawRay(transform.position + new Vector3(0, 1f, 0), transform.forward * attackDistance, Color.red);
                Debug.DrawRay(transform.position + new Vector3(-1f, 1f, 0), transform.forward * attackDistance, Color.green);
                Debug.DrawRay(transform.position + new Vector3(1f, 1f, 0), transform.forward * attackDistance, Color.blue);

                if (!IsDead && Time.time >= lastAttackTimebet && !isAttack)
                {
                    if (Time.time >= lastBehaviorTime && bossState != State.Idle)
                    {
                        lastBehaviorTime = Time.time;
                        lastBehaviorTime += nextBehaviorTimebet;
                        bossState = State.Idle;
                        isAttack = false;
                        agent.speed = enemyData.Speed;
                    }


                    if (Physics.Raycast(rightRay, out raycastHit, longDetectRange, TargetLayer) || Physics.Raycast(leftRay, out raycastHit, longDetectRange, TargetLayer))
                    {
                        //transform.LookAt(targetEntity.transform);
                    }

                    if (DetectPlayer(shortDetectRange) && bossState == State.Idle) //가까이 있을 때 근접 공격
                    {
                        enemyAni.SetBool("isShort", true);
                        enemyAni.SetBool("isMiddle", false);
                        enemyAni.SetBool("isLong", false);

                        bossState = State.Short;
                    }
                    else if (DetectPlayer(middleDetectRange) && bossState == State.Idle) // 대쉬 공격
                    {
                        bossState = State.Middle;

                        enemyAni.SetBool("isShort", false);
                        enemyAni.SetBool("isMiddle", true);
                        enemyAni.SetBool("isLong", false);
                        agent.speed *= 3;
                    }
                    else if (DetectPlayer(longDetectRange) && bossState == State.Idle) // 점프 공격
                    {
                        bossState = State.Long;

                        enemyAni.SetBool("isShort", false);
                        enemyAni.SetBool("isMiddle", false);
                        enemyAni.SetBool("isLong", true);
                    }

                    if(bossState == State.Short)
                    {
                        if (Physics.Raycast(centerRay, out raycastHit, attackDistance, TargetLayer) ||
                            Physics.Raycast(rightRay, out raycastHit, attackDistance, TargetLayer) ||
                            Physics.Raycast(leftRay, out raycastHit, attackDistance, TargetLayer) && !isAttack)
                        {
                            BasicAttack();
                        }
                    }
                    else if (bossState == State.Middle)
                    {
                        Debug.DrawRay(transform.position + new Vector3(0, 1f, 0), transform.forward * middleDetectRange, Color.red);
                        float dashIdle = 3.5f;
                        if (Physics.Raycast(centerRay, out raycastHit, attackDistance * dashIdle, TargetLayer) ||
                            Physics.Raycast(rightRay, out raycastHit, attackDistance * dashIdle, TargetLayer) || 
                            Physics.Raycast(leftRay, out raycastHit, attackDistance * dashIdle, TargetLayer) && !isAttack)
                        {
                            DashAttack();
                        }
                    }
                    else if (bossState == State.Long)
                    {
                        Debug.DrawRay(transform.position + new Vector3(0, 1f, 0), transform.forward * longDetectRange, Color.black);

                        if (Physics.Raycast(centerRay, out raycastHit, longDetectRange, TargetLayer) ||
                            Physics.Raycast(rightRay, out raycastHit, longDetectRange, TargetLayer) ||
                            Physics.Raycast(leftRay, out raycastHit, longDetectRange, TargetLayer) && !isAttack)
                        {
                            StartCoroutine(JumpAttack_co());
                        }
                    }
                    lastAttackTimebet = Time.time;
                    lastAttackTimebet += timebetAttack;


                }
                else if (agent.enabled)
                {
                    agent.SetDestination(targetEntity.transform.position);

                }




                //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetEntity.transform.position), 1f);

                //transform.position = Vector3.MoveTowards(transform.position, targetEntity.transform.position, speed * Time.deltaTime);


            }
            else
            {
                //현재 위치에서 20 반지름으로 가상의 원을 만들어 TargetLayer를 가진 콜라이더 추출
                Collider[] coll = Physics.OverlapSphere(transform.position, detectRange, TargetLayer);

                for (int i = 0; i < coll.Length; i++)
                {
                    if (coll[i].TryGetComponent(out Entity e))
                    {
                        if (!e.IsDead)
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
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, longDetectRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, middleDetectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shortDetectRange);
    }

    private void BasicAttack()
    {
        agent.isStopped = true;
        isAttack = true;
        enemyAni.SetBool("isMove", !isAttack);
        enemyAni.SetTrigger("Attack");
    }
    private void DashAttack()
    {
        agent.isStopped = true;
        isAttack = true;
        enemyAni.SetBool("isMove", !isAttack);
        enemyAni.SetTrigger("DashAttack");
    }
    private IEnumerator JumpAttack_co()
    {
        isAttack = true;

        enemyR.useGravity = false;
        enemyR.isKinematic = true;
        agent.enabled = false;
        Debug.Log("JumpAttack 했어용");

        enemyAni.SetTrigger("JumpAttack");
        yield return new WaitForSeconds(0.3f);
        float jumpY = transform.position.y + jumpPower;
        while (transform.position.y < jumpY - 2f)
        {
            transform.LookAt(targetEntity.transform);
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, jumpY, transform.position.z), 1f * Time.deltaTime);
            yield return null;
           

        }
        enemyAni.SetTrigger("JumpIdle");
        GetComponent<BoxCollider>().enabled = true;

        Vector3 tempPos = targetEntity.transform.position;
        while ((transform.position.y - tempPos.y) > 1f)
        {

            transform.position = Vector3.Lerp(transform.position, tempPos, 5f * Time.deltaTime);
            yield return null;

        }

        enemyAni.SetTrigger("JumpEnd");
        enemyR.useGravity = true;
        enemyR.isKinematic = false;
        agent.enabled = true;
        agent.isStopped = true;
        enemyAni.SetBool("isMove", !isAttack);

    }

    private void Start()
    {
        if (isAI)
            StartCoroutine(UpdataTargetPosition());
    }
    private void Update()
    {
        if (isAI)
        {
            enemyAni.SetBool("HasTarget", isTarget);
        }
    }
}
