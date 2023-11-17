using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Boss
{
    private CapsuleCollider skeletonCapCol;
    [Header("Effect")]
    [SerializeField] private List<GameObject> jumpEffects;
    [SerializeField] private GameObject StrongEffect;
    [SerializeField] private GameObject SwordForceEffect;
    [SerializeField] private int swordForceCount = 3;
    [SerializeField] private GameObject FireField;
    //[SerializeField] private GameObject ThrowSword;

    [Header("등장 이펙트")]
    [SerializeField] private List<GameObject> appearEffects;

    protected override void Awake()
    {
        base.Awake();
        skeletonCapCol = GetComponent<CapsuleCollider>();
        for(int i = 0; i< appearEffects.Count; i++)
        {
            appearEffects[i].SetActive(false);
        }
        
    }
    void OnSpawn()
    {
        canFight = true;
    }
    void OnStartDodge()
    {
        skeletonCapCol.enabled = false;
        transform.Translate(transform.position * -1f * 20f * Time.deltaTime);

    }
    void OnEndDodge()
    {
        skeletonCapCol.enabled = true;

    }

    protected override void OnStartAttack()
    {
        base.OnStartAttack();
        if (isStrong)
        {
            StrongEffect.transform.position = transform.position;
            StrongEffect.SetActive(true);
            StrongEffect.transform.rotation = Quaternion.Euler(-90f, transform.rotation.eulerAngles.z, transform.rotation.eulerAngles.y);
        }
    }
    protected override void OnEndAttack()
    {
        base.OnEndAttack();
        if (jumpEffects[0].activeSelf)
        {
            jumpEffects[0].SetActive(false);
            jumpEffects[1].SetActive(false);
            jumpEffects[2].SetActive(false);
            jumpEffects[3].SetActive(false);
        }

/*        if (SwordForceEffect.activeSelf)
        {
            SwordForceEffect.SetActive(false);
        }*/
    }
    protected override void OnEndAni()
    {
        base.OnEndAni();
        enemyAni.SetBool("isPoint", false);


    }

    void OnEndEffect()
    {
        if (StrongEffect.activeSelf)
        {
            StrongEffect.SetActive(false);
        }
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
                                //Dodge();

                                if (rand > 70) // 35% 확률
                                {
                                    BasicAttack();
                                }
                                else if (rand > 40) //35% 확률
                                {
                                    ComboAttack();
                                }
                                else if (rand > 20)
                                {
                                    Dodge();
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
                            if (PlayerDetectRange(middleDetectRange))
                            {
                                if (rand > 30) // 70%확률
                                {
                                    if (Vector3.SqrMagnitude(transform.position - player.transform.position) <= 8)
                                    {
                                        DashAttack();
                                    }
                                }
                                else
                                {
                                    StartCoroutine(SwordForceAttack_co());
                                }
                            }
                        }
                        else if (bossState == State.Long)
                        {
                            Debug.DrawRay(transform.position + new Vector3(0, 1f, 0), transform.forward * longDetectRange, Color.black);

                            if (PlayerDetectRange(longDetectRange))
                            {
                                if (rand > 30) // 70%확률
                                {
                                    StartCoroutine(PointAttack_co());
                                }
                                else
                                {
                                    StartCoroutine(JumpAttack_co());
                                }
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
        for (int i = 0; i < appearEffects.Count; i++)
        {
            appearEffects[i].SetActive(true);
        }
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
    private void ComboAttack()
    {
        agent.isStopped = true;
        isAttack = true;
        enemyAni.SetBool("isMove", !isAttack);
        enemyAni.SetTrigger("ComboAttack");
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
    private void Dodge()
    {
        agent.enabled = false;

        isAttack = true;
        enemyAni.SetBool("isMove", !isAttack);
       //Vector3 speed = Vector3.zero;
       //Vector3.SmoothDamp(transform.position, transform.forward * -1f*1000f, ref speed, 0.01f);
        enemyAni.SetTrigger("Dodge");
        //StartCoroutine(ThrowSword_co());
    }
    IEnumerator PointAttack_co()
    {
        agent.isStopped = true;
        isAttack = true;
        enemyAni.SetBool("isMove", !isAttack);
        enemyAni.SetTrigger("Point");

        enemyAni.SetBool("isPoint", true);
        yield return new WaitForSeconds(1f);
        FireField.transform.rotation = Quaternion.Euler(-90f, transform.rotation.eulerAngles.y,180f);

        GameObject firefield = Instantiate(FireField, transform.position, FireField.transform.rotation);
        Destroy(firefield, 10f);

        //Instantiate(FireField, transform.position, )

    }
    /*    private IEnumerator JumpAttack_co()
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

        }*/
    private IEnumerator JumpAttack_co()
    {
        isAttack = true;

        enemyRigid.useGravity = false;
        //enemyR.isKinematic = true;
        agent.enabled = false;
        //Debug.Log("JumpAttack 했어용");

        //점프 이펙트
        jumpEffects[0].transform.position = transform.position;
        jumpEffects[0].SetActive(true);
        enemyAni.SetTrigger("JumpAttack");
        yield return new WaitForSeconds(0.3f);
        float jumpY = transform.position.y + jumpPower;

        while (transform.position.y < jumpY - 1f)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, jumpY, transform.position.z), Time.deltaTime * 1f);
            /*
                        if(transform.position.y > jumpY * 0.3)// 일정높이 이상일 때만 플레이어를 바라봄
                        {
                            transform.LookAt(player.transform);
                        }*/
            transform.LookAt(player.transform);

            yield return null;


        }
        //점프 이펙트
        jumpEffects[1].transform.position = transform.position;
        jumpEffects[1].transform.rotation = transform.rotation;
        jumpEffects[1].SetActive(true);
        enemyAni.SetTrigger("JumpIdle");
        GetComponent<BoxCollider>().enabled = true;
        //float distance = 1.2f; 플레이어 앞에서 멈춤... distance를 곱할려고 했으나 먼가 이상... 좀 생각해볼 필요가 있을듯
        Vector3 tempPos = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
        while ((transform.position.y - tempPos.y) > 1f)
        {

            transform.position = Vector3.Lerp(transform.position, tempPos, 5f * Time.deltaTime);
            yield return null;

        }

        enemyAni.SetTrigger("JumpEnd");
        enemyRigid.useGravity = true;
        //enemyR.isKinematic = false;
        agent.enabled = true;
        agent.isStopped = true;
        //점프 이펙트
        jumpEffects[2].transform.position = transform.position;
        jumpEffects[2].SetActive(true);

        jumpEffects[3].transform.position = transform.position;
        jumpEffects[3].SetActive(true);
        enemyAni.SetBool("isMove", !isAttack);

    }
    IEnumerator SwordForceAttack_co()
    {
        agent.isStopped = true;
        isAttack = true;
        enemyAni.SetBool("isMove", !isAttack);
        enemyAni.SetBool("isSwordForce", false);

        enemyAni.SetTrigger("SwordForceAttack");
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < swordForceCount; i++)
        {
            if (!SwordForceEffect.activeSelf)
            {
                SwordForceEffect.SetActive(true);
            }
            SwordForceEffect.transform.rotation = Quaternion.Euler(-90f, transform.rotation.eulerAngles.z, transform.rotation.eulerAngles.y);

            GameObject force = Instantiate(SwordForceEffect, transform.position, SwordForceEffect.transform.rotation);
            Destroy(force, 3f);
            for (int j = 0; j < 50; j++)
            {
                transform.LookAt(player.transform);
                yield return new WaitForSeconds(0.01f);

            }
        }
        enemyAni.SetBool("isSwordForce", true);

    }


/*    IEnumerator ThrowSword_co()
    {
        Vector3 target = transform.position;
        yield return new WaitForSeconds(1f);
        if (!ThrowSword.activeSelf)
        {
            ThrowSword.SetActive(true);
        }
        ThrowSword.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0f);

        GameObject sword = Instantiate(ThrowSword, target, ThrowSword.transform.rotation);
        Destroy(sword, 3f);
    }*/
}
