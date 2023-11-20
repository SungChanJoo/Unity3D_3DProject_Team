using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    
    // AttackRate, CurrentWeapon ���� ���� �޾ƿͼ� ���
    private PlayerData data;
    private Animator tempAnimator;
    private CameraController controller;

    //�⺻ ����, ��ų ������ΰ�
    public bool skillEnabled = true;
    public bool attackEnabled = true;
    public bool shield = false;
    public bool charging = false;

    //������¸� ������ ��
    public bool onDefence = false;
    public bool hold = false;
    public bool perfectParrying = false;    


    private bool mana;
    private bool performedChargeAttack = false;
    private float chargingTimer = 0;

    [SerializeField] private ParticleSystem skill_1E;
    [SerializeField] private ParticleSystem skill_2E;
    [SerializeField] private Transform skill_2E_Position;


    private void Awake()
    {
        tempAnimator = GetComponent<Animator>();
        data = GetComponent<PlayerData>();
        controller = GetComponent<CameraController>();
    }

    public void OnAttackingAnimationCompleted()
    {
        hold = false;                
        data.CurrentWeapon.DisableDamaging();        
    }    

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log(chargingTimer);
        }
        if (attackEnabled&&!controller.isRolling&&!onDefence)
        {

            if (Input.GetMouseButtonDown(0)&&!charging) // ���� ���콺 ��ư�� ������
            {
                tempAnimator.SetTrigger("Charge");      // ���� �ִϸ��̼�
                performedChargeAttack = false;          // ������ ���� ������ �� �ְ� ��
                charging = true;
                skillEnabled = false;
            }
            else if (Input.GetMouseButton(0))           // ���� ���콺 ��ư�� (���) ������ �ִ� ���� ��
            {
                chargingTimer += Time.deltaTime;        // ����
                hold = true;
                skillEnabled = false;
                if (CheckIfCharged()                    // ���� �� ������ �� �����̰�
                    && !performedChargeAttack)          // �̹� �ش� ���콺 �������� ���� ���� ������ �� ���°� �ƴ϶��
                    ChargeAttack();                     // ���� ����
            }
            else if (Input.GetMouseButtonUp(0))         // ���� ���콺 ��ư���� ���� ������ ��
            {
                skillEnabled = false;
                if (!CheckIfCharged())                  // ������ �� �� �ƴ϶�� (���������� ���� �ʾҴٸ�) 
                    Attack();                           // �Ϲ� ����
            
                ResetChargingTimer();                   // ���� Ÿ�̸� ����
            }

            if (skillEnabled)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))                   
                    Skill1();
    
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                    Skill2();                        
            }            
        }

        if (shield&&!controller.isRolling)
        {
            Shield();
        }
        
    }
    
    private void ResetChargingTimer() => chargingTimer = 0;

    private bool CheckIfCharged() => chargingTimer >= 1f;

    public void Attack()
    {
        shield = false;
        hold = true;
        tempAnimator.SetTrigger("Attack");
        data.CurrentWeapon.Attack();
        
    }

    public void ChargeAttack()
    {
        shield = false;
        hold = true;
        tempAnimator.SetTrigger("ChargeAttack");
        data.CurrentWeapon.ChargeAttack();

        performedChargeAttack = true;
        ResetChargingTimer();
    }

    public void Shield()
    {
        if (Input.GetMouseButtonDown(1))
        {   

            tempAnimator.SetTrigger("Shield");
            hold = true;
            onDefence = true;
            attackEnabled = false;
            skillEnabled = false;
        }        
        else if (Input.GetMouseButtonUp(1))
        {
            hold = false;
            onDefence = false;
            attackEnabled = true;
            skillEnabled = true;
        }
        tempAnimator.SetBool("Hold", onDefence);

        //���� ����� Ȧ�尡 Ʈ���̸� ������ �����ϰ�
        //�ִϸ��̼� �̺�Ʈ �ɾ�� �����Ӻ��� �� 0.2�ʰ�
        //perfectParrying�� false�� �ٲٰ� parry�� true�� �ٲپ�
        //�ٸ� ����� �����ϰ� ������ ���� ���� ����
    }
    public void ParryEvent()
    {
        StartCoroutine(Parry());
    }

    private IEnumerator Parry()
    {
        
        perfectParrying = true;
        yield return new WaitForSeconds(0.4f);
        perfectParrying = false;        
    }


    public void Skill1()
    {
        shield = false;
        mana =data.UseMana(20);
        if (mana)
        {            
            tempAnimator.SetTrigger("Skill1");
            data.CurrentWeapon.Skill1();
            hold = true;
        }
        else
        {
            Debug.Log("������ �����մϴ�.");
        }
    }

    public void Skill2()
    {
        shield = false;
        mana = data.UseMana(20);
        if (mana)
        {
            tempAnimator.SetTrigger("Skill2");
            data.CurrentWeapon.Skill2();
            hold = true;
        }
        else
        {
            Debug.Log("������ �����մϴ�.");
        }
    }

    // Option 1
    // ���� : ���� Skill2 ���� ���ݿ����� �߰��� ������� �ִ� �̺�Ʈ�� �ʿ��� �� ���� �� �� ����
    // ���� : �̺�Ʈ �ʿ��� �ʼ������� ����� ���� �����ؼ� �Ѱ���� ��
    public void OnAdditionalAttack(float damage)
    {
        data.CurrentWeapon.AdditionalAttack(damage);
    }

    // Option 2
    // ���� : ����� ���� ��ũ��Ʈ �ʿ���, Ư�� ���� �ʿ��� ������ �� ����
    // ���� : Skill2���� ����� �� �ִ� �޼ҵ���
    public void OnAdditionalSkill2()
    {
        data.CurrentWeapon.AdditionalSkill2();
    }

    #region // �ִϸ��̼� �̺�Ʈ ����
    private void MoveHold()
    {
        hold = !hold;

    }
    private void MoveHoldFalse()
    {
        hold = false;
    }
    private void AttackEabled()
    {
        attackEnabled = true;
    }
    private void AttackEabledFalse()
    {
        attackEnabled = false;
    }
    private void ShieldTrue()
    {
        shield = true;
    }
    private void SkillEnabled()
    {
        skillEnabled = true;
    }
    private void Charging()
    {
        charging = false;
    }
    #endregion

    private IEnumerator Skill_1E()
    {
        skill_1E.Play();
        yield return new WaitForSeconds(1f);
        skill_1E.Stop();
    }
    private IEnumerator Skill_2E()
    {
        skill_2E_Position.position = transform.position;
        skill_2E_Position.rotation = transform.rotation;
        skill_2E.Play();
        yield return new WaitForSeconds(0.7f);
        skill_2E.Stop();
    }
}
