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
                Vector3.SqrMagnitude(targetEntity.transform.position - transform.position) < 150f) // �÷��̾� �Ÿ��� Ž�� �����ȿ� ���� ��  
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
        //���߿� ����..!
    }

    protected void OnTriggerEnter(Collider other)
    {
            if (other.TryGetComponent(out Entity e))

            {
                if (targetEntity.Equals(e))
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
        agent.isStopped = false;
        enemyAni.SetBool("isMove", !isAttack);
        Debug.Log("move�� " + !isAttack);
    }
    private IEnumerator UpdataTargetPosition()
    {
        RaycastHit raycastHit;
        Ray ray;
        while (!IsDead)
        {
            hpSlider.value = Health; // �� �����̴� ���� ���ϴ� �� �𸣰ڳ�...

            if (isTarget)
            {           
                ray = new Ray(transform.position + new Vector3(0, 1f, 0), transform.forward );

                Debug.DrawRay(transform.position + new Vector3(0, 1f, 0), transform.forward * attackDistance, Color.red);
                //float distance = Vector3.Distance(targetEntity.transform.position, transform.position);
                //�Ÿ��� ���ݹ������� ������ �������� �ƴҶ� //, 10f, LayerMask.NameToLayer("Player")
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
                //���� ��ġ���� 20 ���������� ������ ���� ����� TargetLayer�� ���� �ݶ��̴� ����
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
