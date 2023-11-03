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
    [SerializeField] private float startAttackTime = 0.3f; // ���ݽ��۽ð�
    [SerializeField] private float endAttackTime = 1.5f; // ��������ð�

    private bool isPatroll = true;
    private bool isMiss = false;
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


    protected override void Awake()
    {
        base.Awake();
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
    
    IEnumerator DelayAttack_co()
    {
        float aniTime = timebetAttack;
        yield return new WaitForSeconds(startAttackTime); //���� ���۽� ���� �ݶ��̴� Ȱ��ȭ
        aniTime -= startAttackTime;
        weapon.GetComponent<BoxCollider>().enabled = true;

        yield return new WaitForSeconds(timebetAttack - endAttackTime); //���� ������ ���� �ݶ��̴� ��Ȱ��ȭ
        aniTime -= endAttackTime;
        weapon.GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(aniTime); //���� �ִϸ��̼� ���

        yield return new WaitForSeconds(aniTime); //���� �ִϸ��̼� ���
        isAttack = false;
        agent.isStopped = false;
        enemyAni.SetBool("isAttack", false);
        enemyAni.SetBool("isMove", !isAttack);


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
                //�Ÿ��� ���ݹ������� ������ �������� �ƴҶ� //, 10f, LayerMask.NameToLayer("Player")
                if (Physics.Raycast(ray, out raycastHit, attackDistane, TargetLayer)  )
                {
                    if (!IsDead && Time.time >= lastAttackTimebet && !isAttack)
                    {
                        lastAttackTimebet = Time.time;
                        lastAttackTimebet += timebetAttack;
                        agent.isStopped = true;

                        isAttack = true;
                        enemyAni.SetBool("isMove", !isAttack);

                        enemyAni.SetTrigger("Attack");
                        enemyAni.SetBool("isAttack", isAttack);

                        StartCoroutine(DelayAttack_co());
                    }
                    //Debug.Log(raycastHit.transform.gameObject);
                }
                else if(!isAttack)
                {
                    agent.SetDestination(targetEntity.transform.position);

                }
                isMiss = true;
            }
            else
            {

                //agent.isStopped = true;
                //���� ��ġ���� 20 ���������� ������ ���� ����� TargetLayer�� ���� �ݶ��̴� ����
                Collider[] coll = Physics.OverlapSphere(transform.position + transform.forward * (detectPlayerRange-1f), detectPlayerRange, TargetLayer);

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
                agent.SetDestination(wayPoint[UnityEngine.Random.Range(0, wayPoint.Length)].transform.position);

        }

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
        Gizmos.DrawWireSphere(transform.position + transform.forward * (detectPlayerRange - 1f), detectPlayerRange);
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
            if(isPatroll)
            {
                Patroll();
            }

        }


    }
}
