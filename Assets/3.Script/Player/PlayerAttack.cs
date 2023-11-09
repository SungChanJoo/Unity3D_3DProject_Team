using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator tempAnimator;
    
    // AttackRate, CurrentWeapon 등의 정보 받아와서 사용
    [SerializeField] private PlayerData data;

    //가드상태를 유지할 때
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

        //막기 사용중 홀드가 트루이면 데미지 감소하고
        //애니메이션 이벤트 걸어둔 프레임부터 약 0.2초간
        //perfectParrying를 false로 바꾸고 parry를 true로 바꾸어
        //다른 모션을 실행하고 데미지 감소 없이 진행
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
            Debug.Log("마나가 부족합니다.");
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
            Debug.Log("마나가 부족합니다.");
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
