using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyControll : MonoBehaviour, IDamageable
{
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
    [SerializeField] private float startAttackTime = 0.3f; // ���ݼӵ�
    private float lastAttackTimebet;
    [SerializeField] private Slider hpSlider;
    private Animator enemyAni;
    private Rigidbody enemyRigid;
    public event Action OnDead;
    private bool isAttack = false;
    private bool isAttackTime = false; //Į�� ����ĥ �� �������� �ް� ����
    [SerializeField] private GameObject weapon;


    private bool isTarget
    {
        get
        {
            if (targetEntity != null && !targetEntity.IsDead)
            {
                return true;
            }
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
        //�´� �ִϸ��̼� �߰����� ������ todo 1031
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
        //�״� �ִϸ��̼� �߰����� ������ todo 1031
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (!IsDead && Time.time + startAttackTime >= lastAttackTimebet + timebetAttack)
        {
            lastAttackTimebet = Time.time;

            if (other.TryGetComponent(out Entity e))

            {
                if (targetEntity.Equals(e))
                {
                    //���� �ִϸ��̼� �߰����� todo 1031
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
        /*        if (isAttack)
                {
                    agent.SetDestination(transform.position);

                    yield return new WaitForSeconds(0.4f);

                }
                yield return new WaitForSeconds(1.10f-0.4f);

                yield return new WaitForSeconds(0.9f);*/
        weapon.GetComponent<BoxCollider>().enabled = true;

        yield return new WaitForSeconds(timebetAttack - startAttackTime);
        weapon.GetComponent<BoxCollider>().enabled = false;

        isAttack = false;
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

                    Debug.Log(raycastHit.transform.gameObject);

                    isAttack = true;
                    agent.isStopped = true;

                    enemyAni.SetTrigger("Attack");
                    enemyAni.SetBool("isAttack", true);

                    StartCoroutine(DelayAttack_co());
                }
                else if(!isAttack)
                {

                    agent.isStopped = false;
                    agent.SetDestination(targetEntity.transform.position);
                }
                else
                {

                }
            }
            else
            {
                agent.isStopped = true;
                //���� ��ġ���� 20 ���������� ������ ���� ����� TargetLayer�� ���� �ݶ��̴� ����
                Collider[] coll = Physics.OverlapSphere(transform.position, 20f, TargetLayer);
                for(int i = 0; i < coll.Length; i++)
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
        }


    }
}
