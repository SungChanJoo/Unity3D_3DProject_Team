using System.Collections;
using UnityEngine;
public enum AttackSound
{
    Attack,
    ChargeAttack,
    Skill1,
    Skill2,
    Parrying
}
public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip chargeAttackClip;
    [SerializeField] private AudioClip skill1Clip;
    [SerializeField] private AudioClip skill2Clip;
    [SerializeField] private AudioClip skill2AdditionalClip;
    [SerializeField] private AudioClip shieldClip;

    // AttackRate, CurrentWeapon 등의 정보 받아와서 사용
    private PlayerData data;
    private Animator tempAnimator;
    private CameraController controller;

    //기본 공격, 스킬 사용중인가
    public bool skillEnabled = true;
    public bool attackEnabled = true;
    public bool shield = false;
    public bool charging = false;

    //가드상태를 유지할 때
    public bool onDefence = false;
    public bool hold = false;
    public bool perfectParrying = false;    


    private bool mana;
    private bool performedChargeAttack = false;
    private float chargingTimer = 0;


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

    public void OnPlayAttackSound(AttackSound soundType)
    {
        switch (soundType)
        {
            case AttackSound.Attack:
                audioSource.PlayOneShot(attackClip);
                break;
            case AttackSound.ChargeAttack:
                audioSource.PlayOneShot(chargeAttackClip);
                break;
            case AttackSound.Skill1:
                audioSource.PlayOneShot(skill1Clip);
                break;
            case AttackSound.Skill2:
                audioSource.PlayOneShot(skill2Clip);
                break;
            case AttackSound.Parrying:
                audioSource.PlayOneShot(shieldClip);
                break;
            default:
                break;
        }
    }
    

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log(chargingTimer);
        }
        if (attackEnabled&&!controller.isRolling&&!onDefence)
        {

            if (Input.GetMouseButtonDown(0)&&!charging) // 왼쪽 마우스 버튼을 누르면
            {
                tempAnimator.SetTrigger("Charge");      // 차지 애니메이션
                performedChargeAttack = false;          // 새로이 차지 공격할 수 있게 됨
                charging = true;
                skillEnabled = false;
            }
            else if (Input.GetMouseButton(0))           // 왼쪽 마우스 버튼을 (계속) 누르고 있는 중일 때
            {
                chargingTimer += Time.deltaTime;        // 차지
                hold = true;
                skillEnabled = false;
                if (CheckIfCharged()                    // 만약 다 차지가 된 상태이고
                    && !performedChargeAttack)          // 이미 해당 마우스 누름으로 인해 차지 공격을 한 상태가 아니라면
                    ChargeAttack();                     // 차지 공격
            }
            else if (Input.GetMouseButtonUp(0))         // 왼쪽 마우스 버튼에서 손을 떼었을 때
            {
                skillEnabled = false;
                if (!CheckIfCharged())                  // 차지가 된 게 아니라면 (차지공격을 하지 않았다면) 
                    Attack();                           // 일반 공격
            
                ResetChargingTimer();                   // 차지 타이머 리셋
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

        //막기 사용중 홀드가 트루이면 데미지 감소하고
        //애니메이션 이벤트 걸어둔 프레임부터 약 0.2초간
        //perfectParrying를 false로 바꾸고 parry를 true로 바꾸어
        //다른 모션을 실행하고 데미지 감소 없이 진행
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
            Debug.Log("마나가 부족합니다.");
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
            Debug.Log("마나가 부족합니다.");
        }
    }

    // Option 1
    // 장점 : 추후 Skill2 외의 공격에서도 추가적 대미지를 주는 이벤트가 필요할 때 재사용 할 수 있음
    // update => audioSource.PlayOneShot(attackClip);을 추가해서 재사용 불가능. 재사용 할려면 알맞게 수정해야함
    // 단점 : 이벤트 쪽에서 필수적으로 대미지 값을 지정해서 넘겨줘야 함
    public void OnAdditionalAttack(float damage)
    {
        audioSource.PlayOneShot(skill2AdditionalClip);
        data.CurrentWeapon.AdditionalAttack(damage);
    }

    // Option 2
    // 장점 : 대미지 값을 스크립트 쪽에서, 특히 무기 쪽에서 제어할 수 있음
    // 단점 : Skill2에만 사용할 수 있는 메소드임
    public void OnAdditionalSkill2()
    {
        audioSource.PlayOneShot(skill2AdditionalClip);
        data.CurrentWeapon.AdditionalSkill2();
    }

    #region // 애니메이션 이벤트 조건
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
}
