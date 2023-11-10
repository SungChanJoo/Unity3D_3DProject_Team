using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Boss
{
    protected override void Awake()
    {
        base.Awake();
    }
    private IEnumerator UpdataTargetPosition()
    {

        while (!IsDead)
        {
            if(canFight)
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
                            bossState = State.Short;
                            SetRangeAni(bossState);
                        }
                        else if (DetectPlayer(middleDetectRange) && bossState == State.Idle) // 대쉬 공격
                        {
                            bossState = State.Middle;
                            SetRangeAni(bossState);

                            agent.speed *= 3;
                        }
                        else if (DetectPlayer(longDetectRange) && bossState == State.Idle) // 점프 공격
                        {
                            bossState = State.Long;
                            SetRangeAni(bossState);
                        }

                        if (bossState == State.Short)
                        {
                            if (PlayerDetectRange(attackDistance))
                            {
                                if (rand > 30) // 70%확률
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
                            float dashIdle = 2.5f;
                            if (PlayerDetectRange(middleDetectRange * dashIdle))
                            {
                                DashAttack();
                            }
                        }
                        else if (bossState == State.Long)
                        {
                            Debug.DrawRay(transform.position + new Vector3(0, 1f, 0), transform.forward * longDetectRange, Color.black);

                            if (PlayerDetectRange(longDetectRange))
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
            }

            yield return null;
        }//end while

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


    protected void Start()
    {
        if (isAI)
        {
            StartCoroutine(UpdataTargetPosition());
        }

    }



    private void Update()
    {

        if (isAI)
        {
            if(player != null)
            {
                if (!player.IsDead)
                {
                    enemyAni.SetBool("HasTarget", isTarget);
                }
            }


        }

    }
    //보스 스킬
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
        //enemyR.isKinematic = true;
        agent.enabled = false;
        //Debug.Log("JumpAttack 했어용");

        enemyAni.SetTrigger("JumpAttack");
        yield return new WaitForSeconds(0.3f);
        float jumpY = transform.position.y + jumpPower;
        while (transform.position.y < jumpY - 1f)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, jumpY, transform.position.z), 1f * Time.deltaTime);
            /*
                        if(transform.position.y > jumpY * 0.3)// 일정높이 이상일 때만 플레이어를 바라봄
                        {
                            transform.LookAt(player.transform);
                        }*/
            transform.LookAt(player.transform);

            yield return null;


        }

        enemyAni.SetTrigger("JumpIdle");
        GetComponent<BoxCollider>().enabled = true;
        //float distance = 1.2f; 플레이어 앞에서 멈춤... distance를 곱할려고 했으나 먼가 이상... 좀 생각해볼 필요가 있을듯
        Vector3 tempPos = new Vector3( player.transform.position.x, player.transform.position.y, player.transform.position.z);
        while ((transform.position.y - tempPos.y) > 1f)
        {

            transform.position = Vector3.Lerp(transform.position, tempPos, 5f * Time.deltaTime);
            yield return null;

        }

        enemyAni.SetTrigger("JumpEnd");
        enemyR.useGravity = true;
        //enemyR.isKinematic = false;
        agent.enabled = true;
        agent.isStopped = true;
        enemyAni.SetBool("isMove", !isAttack);

    }
}
