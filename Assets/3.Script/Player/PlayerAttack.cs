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
    private bool performedChargeAttack = false;    

    private float chargingTimer = 0;

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
        if (Input.GetMouseButtonDown(0))            // 왼쪽 마우스 버튼을 누르면
        {
            tempAnimator.SetTrigger("Charge");      // 차지 애니메이션

            // chargingTimer += Time.deltaTime;을 여기에 넣으면 performedChargeAttack 사용 안 해도 되긴 하는데 다른 사람이 코드를 이해하기 어려울까봐 못 넣겠다.
            performedChargeAttack = false;          // 새로이 차지 공격할 수 있게 됨
        }
        else if (Input.GetMouseButton(0))           // 왼쪽 마우스 버튼을 (계속) 누르고 있는 중일 때
        {
            chargingTimer += Time.deltaTime;        // 차지

            if (CheckIfCharged()                    // 만약 다 차지가 된 상태이고
                && !performedChargeAttack)          // 이미 해당 마우스 누름으로 인해 차지 공격을 한 상태가 아니라면
                ChargeAttack();                     // 차지 공격
        }
        else if (Input.GetMouseButtonUp(0))         // 왼쪽 마우스 버튼에서 손을 떼었을 때
        {
            if (!CheckIfCharged())                  // 차지가 된 게 아니라면 (차지공격을 하지 않았다면) 
                Attack();                           // 일반 공격

            ResetChargingTimer();                   // 차지 타이머 리셋
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))                   
            Skill1();
        
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            skill2();
        
        Shield();
    }

    private void ResetChargingTimer() => chargingTimer = 0;

    private bool CheckIfCharged() => chargingTimer >= 1;

    public void Attack()
    {
        tempAnimator.SetTrigger("Attack");
        data.CurrentWeapon.Attack();
    }

    public void ChargeAttack()
    {
        tempAnimator.SetTrigger("ChargeAttack");
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
