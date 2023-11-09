using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


enum State
{
    Idle = 0,
    Short,
    Middle,
    Long,

}

public class Boss : Enemy
{

    [SerializeField] private string name = string.Empty;
    [SerializeField] private Text nameText;
    [SerializeField] private Text damageText;

    [Header("Boss 세팅")]
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private float longDetectRange = 20f;
    [SerializeField] private float middleDetectRange = 10f;
    [SerializeField] private float shortDetectRange = 5f;
    [SerializeField] private float jumpPower = 5f;
    [SerializeField] private float nextBehaviorTimebet = 10f;
    private float lastBehaviorTime;
    private Rigidbody enemyR;
    private float totalDamage = 0;

    [SerializeField] private GameObject enemyStrongEffect;





    RaycastHit raycastHit;
    Ray centerRay;
    Ray rightRay;
    Ray leftRay;

    State bossState =0;
    public bool isStrong = false;

    //private bool isBehavior = false;


    private bool isTarget
    {
        get
        {
            if (player != null && !player.IsDead) // 플레이어 거리가 탐지 범위안에 있을 때  
            {
                return true;
            }
            return false;

        }
    }
    private void SetUp()
    {
        nameText.text = name;
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
        enemyStrongEffect.SetActive(false);
    }



    public override void TakeDamage(float damage, float knockBack, Vector3 hitposition, Vector3 hitNomal)
    {
        base.TakeDamage(damage, knockBack, hitposition, hitNomal);
        StartCoroutine(UITakeDamage_co(damage));
    }
    private IEnumerator UITakeDamage_co(float damage)
    {
        totalDamage += damage;
        damageText.text = $"{totalDamage}";
        damageText.color = new Color(damageText.color.r, damageText.color.g, damageText.color.b, 1);
        while (damageText.color.a > 0.0f)
        {
            damageText.color = new Color(damageText.color.r, damageText.color.g, damageText.color.b, damageText.color.a - (0.1f * Time.deltaTime));
            yield return null;
        }
        totalDamage = 0;
    }

    public override void Die()
    {
        base.Die();
        //나중에 해줘..!
    }
    void OnStartAttack()
    {
        weapon.GetComponent<BoxCollider>().enabled = true;
        if (enemyStrongEffect.activeSelf == true)
        {
            enemyStrongEffect.SetActive(false);
        }
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
        if(!player.IsDead)
        {
            //AI 활성화
            agent.enabled = true;
            agent.isStopped = false;

            //상태 초기화
            bossState = State.Idle;
            enemyAni.SetBool("isMove", true);
            agent.speed = enemyData.Speed;

            isStrong = false;

            lastBehaviorTime = Time.time;
            lastBehaviorTime += nextBehaviorTimebet;
        }
        else
        {
            enemyAni.SetBool("HasTarget", isTarget);
        }

    }
    protected void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerData e))

        {
            if (player.Equals(e))
            {
                //ClosestPoint -> 닿는 위치
                //상대방 피격 위치와 피격 방향 근사값을 계산
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 hitNormal = transform.position - other.transform.position;
                if (isStrong)
                {
                    e.TakeDamage(damage*2f, force, hitPoint, hitNormal);
                }
                else
                {
                    e.TakeDamage(damage, force, hitPoint, hitNormal);
                }
            }
        }
    }

    public bool DetectPlayer(float detectRange)
    {
        Collider[] coll = Physics.OverlapSphere(transform.position, detectRange, TargetLayer);
        for (int i = 0; i < coll.Length; i++)
        {
            if (coll[i].TryGetComponent(out PlayerData e))
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
                    float rand = Random.Range(0, 100);
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
                        //transform.LookAt(player.transform);
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
                            if(rand > 30) // 70%확률
                            {
                                BasicAttack();
                            }
                            else
                            {
                                StrongAttack();
                            }
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
                    agent.SetDestination(player.transform.position);

                }
            }
            else
            {
                //현재 위치에서 20 반지름으로 가상의 원을 만들어 TargetLayer를 가진 콜라이더 추출
                Collider[] coll = Physics.OverlapSphere(transform.position, detectRange, TargetLayer);

                for (int i = 0; i < coll.Length; i++)
                {
                    if (coll[i].TryGetComponent(out PlayerData e))
                    {
                        if (!e.IsDead)
                        {
                            player = e;
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
    private void StrongAttack()
    {
        agent.isStopped = true;
        isAttack = true;
        isStrong = true;
        enemyStrongEffect.SetActive(true);
        enemyAni.SetBool("isMove", !isAttack);
        enemyAni.SetTrigger("StrongAttack");
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
            transform.LookAt(player.transform);
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, jumpY, transform.position.z), 1f * Time.deltaTime);
            yield return null;
           

        }
        enemyAni.SetTrigger("JumpIdle");
        GetComponent<BoxCollider>().enabled = true;

        Vector3 tempPos = player.transform.position;
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
            if(!player.IsDead)
            {
                enemyAni.SetBool("HasTarget", isTarget);

            }

        }
    }
}
