using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator tempAnimator;
    
    // AttackRate, CurrentWeapon ���� ���� �޾ƿͼ� ���
    [SerializeField] private PlayerData data;

    //������¸� ������ ��
    public bool hold = false;
    public bool perfectParrying = false;
    private bool mana;

    private void Awake()
    {
        tempAnimator = GetComponent<Animator>();
    }
    public void OnAttackingAnimationCompleted()
    {
        data.CurrentWeapon.DisableDamaging();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            Attack();

        else if (Input.GetKeyDown(KeyCode.L))
            ChargeAttack();

        else if (Input.GetKeyDown(KeyCode.Alpha1))                   
            Skill1();
        
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            skill2();
        

        Shield();

    }

    public void Attack()
    {
        tempAnimator.SetTrigger("Attack");
        data.CurrentWeapon.Attack();
    }

    public void ChargeAttack()
    {
        tempAnimator.SetTrigger("ChargeAttack");
        data.CurrentWeapon.ChargeAttack();
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
        }
        else
        {
            Debug.Log("������ �����մϴ�.");
        }
    }
    public void skill2()
    {
        mana = data.UseMana(20);
        if (mana)
        {
            tempAnimator.SetTrigger("Skill2");
            data.CurrentWeapon.Skill2();
        }
        else
        {
            Debug.Log("������ �����մϴ�.");
        }
    }
    private IEnumerator skill2_Delay()
    {
        data.CurrentWeapon.Skill2();
        //hold = true;
        yield return new WaitForSeconds(0.5f);
        //data.CurrentWeapon.Skill2();
        //hold = false;
    }
        
    



}
