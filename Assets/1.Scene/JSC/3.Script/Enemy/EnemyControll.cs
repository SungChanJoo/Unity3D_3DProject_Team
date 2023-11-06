using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyControll : MonoBehaviour, IDamageable
{
    [Header("AI Ȱ��ȭ")]
    public bool isAI = true;
    [Header("������ ��� ���̾�")]
    public LayerMask TargetLayer;
    private Entity targetEntity;

    [SerializeField] private Transform player;
    //��θ� ����� AI Agent 
    private NavMeshAgent agent;

    [Header("�� ����")]
    public float MaxHeath = 100f;
    public float Health { get; protected set; }
    public bool IsDead { get; protected set; }

    [SerializeField] private float damage = 20f;
    [SerializeField] private float force = 0f; // �̴���
    [SerializeField] private float attackDistane = 2f; //���ݹ���
    [SerializeField] private float timebetAttack = 2.267f; // ���ݼӵ�
    [SerializeField] private float startAttackTime = 0.3f; // ���ݽ��۽ð�
    [SerializeField] private float endAttackTime = 1.5f; // ��������ð�
    [SerializeField] private float detectPlayerRange = 5f; // �÷��̾� Ž�� ����
    private float lastAttackTimebet;

    [Header("ETC")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private GameObject weapon;
    [SerializeField] private GameObject[] wayPoint;
    private Animator enemyAni;
    private Rigidbody enemyRigid;
    private bool isAttack = false;
    private bool isAttackTime = false; //Į�� ����ĥ �� �������� �ް� ����
    private bool isPatroll = true;
    private bool isMiss = false;
    public event Action OnDead;

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

    protected virtual void OnEnable()
    {
        IsDead = false;
        Health = MaxHeath;
    }

    protected virtual void Awake()
    {
        TryGetComponent(out agent);
        TryGetComponent(out enemyAni);
        TryGetComponent(out enemyRigid);
        weapon.GetComponent<BoxCollider>().enabled = false;
        hpSlider.value = MaxHeath;
    }



    public virtual void TakeDamage(float damage, float knockBack, Vector3 hitposition, Vector3 hitNomal)
    {
        enemyAni.SetTrigger("TakeDamage");
        transform.LookAt(targetEntity.transform.position);
        Health -= damage;
        hpSlider.value = Health;
        Debug.Log("�� ������..");
        if (Health <= 0 && !IsDead)
        {
            Die();
        }
    }
    
    public virtual void Die()
    {
        Debug.Log("������");

        if (OnDead != null)
        {
            OnDead();
        }
        IsDead = true;

        //enemy�� ������ �ִ� collider ����
        Collider[] colls = GetComponents<Collider>();
    
        foreach(Collider c in colls)
        {
            c.enabled = false;
        }

        agent.isStopped = true;
        agent.enabled = false;
        enemyAni.SetTrigger("Death");
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (!IsDead && Time.time >= lastAttackTimebet + timebetAttack)
        {
            lastAttackTimebet = Time.time;

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

        isAttack = false;
        agent.isStopped = false;
        enemyAni.SetBool("isAttack", false);

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
                if (Physics.Raycast(ray, out raycastHit, attackDistane, TargetLayer) && !isAttack )
                {

                    //Debug.Log(raycastHit.transform.gameObject);

                    isAttack = true;
                    agent.isStopped = true;

                    enemyAni.SetTrigger("Attack");
                    enemyAni.SetBool("isAttack", isAttack);

                    StartCoroutine(DelayAttack_co());
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
