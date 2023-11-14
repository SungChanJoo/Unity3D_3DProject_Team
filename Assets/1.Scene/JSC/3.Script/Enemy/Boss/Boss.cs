using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum State
{
    Idle = 0,
    Short,
    Middle,
    Long,

}

public class Boss : Enemy
{

    [SerializeField] protected string name = string.Empty;
    [SerializeField] protected Text nameText;
    [SerializeField] protected Text damageText;

    [Header("Boss 세팅")]
    [SerializeField] protected EnemyData enemyData;
    [SerializeField] protected float longDetectRange = 20f;
    [SerializeField] protected float middleDetectRange = 10f;
    [SerializeField] protected float shortDetectRange = 5f;
    [SerializeField] protected float jumpPower = 5f;
    [SerializeField] protected float nextBehaviorTimebet = 10f;
    protected float lastBehaviorTime;
    protected Rigidbody enemyR;
    protected float totalDamage = 0;

    [SerializeField] protected GameObject enemyStrongEffect;

    public bool canFight = false;



    protected RaycastHit raycastHit;
    protected Ray centerRay;
    protected Ray rightRay;
    protected Ray leftRay;

    protected State bossState;
    public bool isStrong = false;

    //protected bool isBehavior = false;


    protected bool isTarget
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
    protected void SetUp()
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
        bossState = State.Idle;
    }



    public override void TakeDamage(float damage, float knockBack, Vector3 hitposition, Vector3 hitNomal)
    {
        base.TakeDamage(damage, knockBack, hitposition, hitNomal);
        StartCoroutine(UITakeDamage_co(damage));
    }
    protected IEnumerator UITakeDamage_co(float damage)
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
    protected virtual void OnStartAttack()
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
    protected virtual void OnEndAni()
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

    protected bool PlayerDetectRange(float distance)
    {
        if (Physics.Raycast(centerRay, out raycastHit, distance, TargetLayer) ||
            Physics.Raycast(rightRay, out raycastHit, distance, TargetLayer) ||
            Physics.Raycast(leftRay, out raycastHit, distance, TargetLayer) && !isAttack)
        {
            return true;
        }
        return false;
    }

    protected void SetRangeAni(State bossState)
    {
        switch (bossState)
        {
            case State.Short:
                enemyAni.SetBool("isShort", true);
                enemyAni.SetBool("isMiddle", false);
                enemyAni.SetBool("isLong", false);
                break;
            case State.Middle:
                enemyAni.SetBool("isShort", false);
                enemyAni.SetBool("isMiddle", true);
                enemyAni.SetBool("isLong", false);
                break;
            case State.Long:
                enemyAni.SetBool("isShort", false);
                enemyAni.SetBool("isMiddle", false);
                enemyAni.SetBool("isLong", true);
                break;
            default:
                
                break;
        }
    }


}
