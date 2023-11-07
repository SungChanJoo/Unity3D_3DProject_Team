using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private float longDetectRange = 20f;
    [SerializeField] private float middleDetectRange = 10f;
    [SerializeField] private float shortDetectRange = 5f;
    [SerializeField] private float jumpPower = 5f;
    private Rigidbody enemyR;

    RaycastHit raycastHit;
    Ray centerRay;
    Ray rightRay;
    Ray leftRay;

    private bool isBehavior = false;
    private bool isLong = false;
    private bool isMiddle = false;
    private bool isShort = false;

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
        yield return new WaitForSeconds(timebetAttack);
        isAttack = false;
        agent.enabled = true;

        agent.isStopped = false;

        enemyAni.SetBool("isMove", !isAttack);
        isBehavior = false;
        agent.speed = enemyData.Speed;
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

                    if (Physics.Raycast(rightRay, out raycastHit, attackDistance*1.2f, TargetLayer) || Physics.Raycast(leftRay, out raycastHit, attackDistance*1.2f, TargetLayer))
                    {
                        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetEntity.transform.forward), 0.5f);
                        transform.LookAt(targetEntity.transform);
                    }
                    if (DetectPlayer(shortDetectRange) && !isBehavior) //가까이 있을 때 근접 공격
                    {
                        enemyAni.SetBool("isShort", true);
                        enemyAni.SetBool("isMiddle", false);
                        isLong = false;
                        isMiddle = false;
                        isShort = true;
                    }
                    else if (DetectPlayer(middleDetectRange) && !isBehavior) // 대쉬 공격
                    {
                        enemyAni.SetBool("isShort", false);
                        enemyAni.SetBool("isMiddle", true);
                        isLong = false;
                        isMiddle = true;
                        isShort = false;
                    }
                    else if (DetectPlayer(longDetectRange) && !isBehavior) // 점프 공격
                    {
                        isLong = true;
                        isMiddle = false;
                        isShort = false;
                    }
                    if(isShort)
                    {
                        BasicAttack();

                    }
                    if (isMiddle)
                    {
                        DashAttack();
                    }
                    if (isLong)
                    {
                        JumpAttack();
                    }
                }else if(Time.time >= lastAttackTimebet)
                {
                    lastAttackTimebet = Time.time;
                    lastAttackTimebet += timebetAttack;
                    isLong = false;
                    isMiddle = false;
                    isShort = false;
                    isBehavior = false;
                    agent.speed = enemyData.Speed;
                }
                if(agent.enabled)
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
        isBehavior = true;
        if (Physics.Raycast(centerRay, out raycastHit, attackDistance, TargetLayer))
        {
            agent.isStopped = true;
            isAttack = true;
            enemyAni.SetBool("isMove", !isAttack);
            enemyAni.SetTrigger("Attack");
        }
    }
    private void DashAttack()
    {
        isBehavior = true;
        agent.speed *= 2;

        if (Physics.Raycast(centerRay, out raycastHit, attackDistance*2f, TargetLayer))
        {
            Debug.DrawRay(transform.position + new Vector3(0, 1f, 0), transform.forward * attackDistance*2f, Color.red);

            agent.isStopped = true;
            isAttack = true;
            enemyAni.SetBool("isMove", !isAttack);
            enemyAni.SetTrigger("DashAttack");

        }
    }
    private void JumpAttack()
    {
        isBehavior = true;
        agent.speed *= 2;

        if (Physics.Raycast(centerRay, out raycastHit, attackDistance * 10f, TargetLayer))
        {
            Debug.DrawRay(transform.position + new Vector3(0, 1f, 0), transform.forward * attackDistance * 10f, Color.black);
            agent.enabled = false;

            Debug.Log("JumpAttack 했어용");

            isAttack = true;
            enemyAni.SetBool("isMove", !isAttack);
            enemyAni.SetTrigger("JumpAttack");
            enemyR.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);

        }
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
