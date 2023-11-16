using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public enum MosterState
{
    Idle = 0,
    Chase,
    Attack,
    Patroll,
    Die
}

public class AnyMonster : Enemy
{
    public List<GameObject> wayPoint;
    [SerializeField] private EnemyData enemyData;
    private bool isPatroll = true;
    //private bool isMiss = false;
    protected MosterState state;
    protected RaycastHit raycastHit;
    protected Ray centerRay;
    protected Ray rightRay;
    protected Ray leftRay;

    [SerializeField] protected float nextBehaviorTimebet = 3f;
    protected float lastBehaviorTime;

    private bool isTarget
    {
        get
        {
            if (player != null && !player.IsDead && 
                Vector3.SqrMagnitude(player.transform.position - transform.position) < 150f) // �÷��̾� �Ÿ��� Ž�� �����ȿ� ���� ��  
            {
                isPatroll = false;
                return true;
            }
            player = null;
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
        state = MosterState.Idle;
    }



    public override void TakeDamage(float damage, float knockBack, Vector3 hitposition, Vector3 hitNomal)
    {
        agent.isStopped = true;
        enemyAni.SetTrigger("TakeDamage");
        enemyAni.SetBool("isMove", false);

        if (player != null)
        {
            transform.LookAt(player.transform.position);
        }
        else
        {
            transform.LookAt(transform.forward * -1f);
        }

        base.TakeDamage(damage, knockBack, hitposition, hitNomal);
    }
    
    public override void Die()
    {
        base.Die();
        //���߿� ����..!
    }

    protected void OnTriggerEnter(Collider other)
    {
            if (other.TryGetComponent(out PlayerData e))

            {
                if (player.Equals(e))
                {
                    //ClosestPoint -> ��� ��ġ
                    //���� �ǰ� ��ġ�� �ǰ� ���� �ٻ簪�� ���
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
        if (player != null && !player.IsDead)
        {
            if (PlayerDetectRange(attackDistance))
            {
                state = MosterState.Idle;
            }
            else
            {
                state = MosterState.Chase;
                //AI Ȱ��ȭ
                agent.enabled = true;
                agent.isStopped = false;

                //���� �ʱ�ȭ
                enemyAni.SetBool("isMove", true);
                agent.speed = enemyData.Speed;
            }
        }
        else
        {
            state = MosterState.Patroll;
            enemyAni.SetBool("HasTarget", isTarget);
        }
    }

    private IEnumerator UpdataTargetPosition()
    {
        while (!IsDead)
        {
            hpSlider.value = Health; // �� �����̴� ���� ���ϴ� �� �𸣰ڳ�...

            if (isTarget)
            {
                state = MosterState.Chase;
                agent.speed = enemyData.Speed*2f;

                centerRay = new Ray(transform.position + new Vector3(0, 1f, 0), transform.forward);
                rightRay = new Ray(transform.position + new Vector3(-1f, 1f, 0), transform.forward);
                leftRay = new Ray(transform.position + new Vector3(1f, 1f, 0), transform.forward);
                Debug.DrawRay(transform.position + new Vector3(0, 1f, 0), transform.forward * attackDistance, Color.red);
                Debug.DrawRay(transform.position + new Vector3(-1f, 1f, 0), transform.forward * attackDistance, Color.green);
                Debug.DrawRay(transform.position + new Vector3(1f, 1f, 0), transform.forward * attackDistance, Color.blue);
                //float distance = Vector3.Distance(player.transform.position, transform.position);
                //�Ÿ��� ���ݹ������� ������ �������� �ƴҶ� //, 10f, LayerMask.NameToLayer("Player")
                if (!IsDead && Time.time >= lastAttackTimebet && !isAttack && PlayerDetectRange(attackDistance))
                {
                    state = MosterState.Attack;
                    agent.isStopped = true;
                    isAttack = true;
                    enemyAni.SetBool("isMove", false);
                    enemyAni.SetTrigger("Attack");
                    lastAttackTimebet = Time.time;
                    lastAttackTimebet += timebetAttack;
                    
                }
                else if (state == MosterState.Chase)
                {
                    agent.SetDestination(player.transform.position);
                }
                if(Vector3.SqrMagnitude( player.transform.position - transform.position)>2000f && state == MosterState.Idle)
                {
                    state = MosterState.Patroll;

                    player = null;

                }
            }
            else
            {
                //AI Ȱ��ȭ
                agent.enabled = true;
                agent.isStopped = false;

                //���� �ʱ�ȭ
                enemyAni.SetBool("isMove", true);
                agent.speed = enemyData.Speed;
                state = MosterState.Patroll;

                //���� ��ġ���� 20 ���������� ������ ���� ����� TargetLayer�� ���� �ݶ��̴� ����
                Collider[] coll = Physics.OverlapSphere(transform.position + transform.forward * (detectRange-1f), detectRange, TargetLayer);

                for (int i = 0; i < coll.Length; i++)
                {
                    if(coll[i].TryGetComponent(out PlayerData e))
                    {
                        if(!e.IsDead)
                        {

                            player = e;
                            state = MosterState.Chase;
                            break;
                        }
                    }
                }
            }
            yield return null;
        }
    }
    protected bool PlayerDetectRange(float distance)
    {
        if (Physics.Raycast(centerRay, out raycastHit, distance, TargetLayer) ||
            Physics.Raycast(rightRay, out raycastHit, distance, TargetLayer) ||
            Physics.Raycast(leftRay, out raycastHit, distance, TargetLayer) && !isAttack)
        {
            transform.LookAt(player.transform);
            return true;
        }
        return false;
    }
    void Patroll()
    {
        enemyAni.SetBool("isPatrolling", true);

        //�÷��̾ ��ġ�� �ٷ� ���� �ƴϸ� �÷��̾��� ������ ��ġ���� �̵��� �ڿ� ����..
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
/*        Vector3 tempPos = wayPoint[UnityEngine.Random.Range(0, wayPoint.Count)].transform.position;
        while (tempPos)
        {

        }*/
        agent.SetDestination(wayPoint[UnityEngine.Random.Range(0, wayPoint.Count)].transform.position);
        state = MosterState.Patroll;

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

            if (state == MosterState.Patroll)
            {
                Patroll();
            }

        }


    }
}
