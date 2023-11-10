using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator tempAnimator;
    
    // AttackRate, CurrentWeapon ���� ���� �޾ƿͼ� ���
    [SerializeField] private PlayerData data;

    //��ų ������ΰ�
    private bool skillEnabled = true;

    //������¸� ������ ��
    public bool hold = false;
    public bool perfectParrying = false;
    private bool mana;
    private bool performedChargeAttack = false;

    private float chargingTimer = 0;

    private void Awake()
    {
        tempAnimator = GetComponent<Animator>();
    }

    public void OnAttackingAnimationCompleted()
    {
        hold = false;
        data.CurrentWeapon.DisableDamaging();        
    }
    private void MoveHold()
    {
        hold = !hold;
        
    }
    private void SkillEabled()
    {
        skillEnabled = !skillEnabled;
    }
    void Update()
    {
        if (skillEnabled)
        {
            if (Input.GetMouseButtonDown(0))            // ���� ���콺 ��ư�� ������
            {
                tempAnimator.SetTrigger("Charge");      // ���� �ִϸ��̼�
            
                // chargingTimer += Time.deltaTime;�� ���⿡ ������ performedChargeAttack ��� �� �ص� �Ǳ� �ϴµ� �ٸ� ����� �ڵ带 �����ϱ� ������� �� �ְڴ�.
                performedChargeAttack = false;          // ������ ���� ������ �� �ְ� ��
            }
            else if (Input.GetMouseButton(0))           // ���� ���콺 ��ư�� (���) ������ �ִ� ���� ��
            {
                chargingTimer += Time.deltaTime;        // ����
                
                if (CheckIfCharged()                    // ���� �� ������ �� �����̰�
                    && !performedChargeAttack)          // �̹� �ش� ���콺 �������� ���� ���� ������ �� ���°� �ƴ϶��
                    ChargeAttack();                     // ���� ����
            }
            else if (Input.GetMouseButtonUp(0))         // ���� ���콺 ��ư���� ���� ������ ��
            {
                if (!CheckIfCharged())                  // ������ �� �� �ƴ϶�� (���������� ���� �ʾҴٸ�) 
                    Attack();                           // �Ϲ� ����
            
                ResetChargingTimer();               // ���� Ÿ�̸� ����
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))                   
                Skill1();
        
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                Skill2();                        
            
        }
            Shield();
    }

    private void ResetChargingTimer() => chargingTimer = 0;

    private bool CheckIfCharged() => chargingTimer >= 1;

    public void Attack()
    {

        tempAnimator.SetTrigger("Attack");
        hold = true;
        data.CurrentWeapon.Attack();
        
    }

    public void ChargeAttack()
    {
        tempAnimator.SetTrigger("ChargeAttack");
        hold = true;
        data.CurrentWeapon.ChargeAttack();

        performedChargeAttack = true;
    }

    public void Shield()
    {
        if (Input.GetMouseButtonDown(1))
        {            
            tempAnimator.SetTrigger("Shield");
            hold = true;
           
        }
        else if (Input.GetMouseButtonUp(1))
        {
            hold = false;
        }
        tempAnimator.SetBool("Hold", hold);

        //���� ����� Ȧ�尡 Ʈ���̸� ������ �����ϰ�
        //�ִϸ��̼� �̺�Ʈ �ɾ�� �����Ӻ��� �� 0.2�ʰ�
        //perfectParrying�� false�� �ٲٰ� parry�� true�� �ٲپ�
        //�ٸ� ����� �����ϰ� ������ ���� ���� ����
    }
    public void ParryEvent()
    {
        perfectParrying = true;
    }


    public void Skill1()
    {
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
}
