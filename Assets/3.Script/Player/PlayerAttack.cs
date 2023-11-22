using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
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
    public bool isActing = false;

    private bool mana;
    private float chargingTimer = 0;

    [SerializeField] private ParticleSystem skill_1E;
    [SerializeField] private ParticleSystem skill_2E;
    [SerializeField] private Transform skill_2E_Position;
    [SerializeField] private VisualEffect slashEffect;


    private void Awake()
    {
        tempAnimator = GetComponent<Animator>();
        data = GetComponent<PlayerData>();
        controller = GetComponent<CameraController>();
    }

    public void OnAttackingAnimationCompleted()
    {
        isActing = false;
        hold = false;                
        data.CurrentWeapon.DisableDamaging();        
    }

    public void OnPlayAttackSound(AttackSound soundType)
    {
        audioSource.Stop();

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
        if (attackEnabled&&!controller.isRolling&&!onDefence)
        {                        
            if (Input.GetMouseButtonUp(0))
            {
                skillEnabled = false;                
                Attack();                
            }

            if (skillEnabled)
            {
                if (Input.GetKeyDown(KeyCode.Alpha2))
                    Skill1();

                else if (Input.GetKeyDown(KeyCode.Alpha3))
                    Skill2();                        
                else if (Input.GetKeyDown(KeyCode.Alpha1))
                {                           
                    skillEnabled = false;                                   
                    ChargeAttack(); 
                }
            }
        }

        if (shield&&!controller.isRolling)
        {
            Shield();
        }
        //if (Input.GetMouseButtonDown(0))
        //    Attack();
        //else if (skillEnabled)
        //{
        //    if (Input.GetKeyDown(KeyCode.Alpha1))
        //        ChargeAttack();
        //    else if (Input.GetKeyDown(KeyCode.Alpha2)) // skillEnabled을 사용 안 하는데 괜찮은지 확인 부탁 
        //        Skill1();
        //    else if (Input.GetKeyDown(KeyCode.Alpha3))
        //        Skill2();
        //}
        //
        
    }
    
    private void ResetChargingTimer() => chargingTimer = 0;

    private bool CheckIfCharged() => chargingTimer >= 1f;

    public void Attack()
    {
        isActing = true;
        shield = false;
        hold = true;
        tempAnimator.SetTrigger("Attack");
        data.CurrentWeapon.Attack();
        StartCoroutine(SlashEffect());
    }

    public void ChargeAttack()
    {
        isActing = true;
        shield = false;
        hold = true;
        tempAnimator.SetTrigger("ChargeAttack");
        data.CurrentWeapon.ChargeAttack();

        ResetChargingTimer();
    }

    public void Shield()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isActing = true;
            tempAnimator.SetTrigger("Shield");
            hold = true;
            onDefence = true;
            attackEnabled = false;
            skillEnabled = false;
        }        
        else if (Input.GetMouseButtonUp(1))
        {
            isActing = false;
            hold = false;
            onDefence = false;
            attackEnabled = true;
            skillEnabled = true;
        }
        tempAnimator.SetBool("Hold", onDefence);
        
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
        isActing = true;
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
        isActing = true;
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
    // update => audioSource.PlayOneShot(attackClip);�� �߰��ؼ� ���� �Ұ���. ���� �ҷ��� �˸°� �����ؾ���
    // ���� : �̺�Ʈ �ʿ��� �ʼ������� ����� ���� �����ؼ� �Ѱ���� ��
    public void OnAdditionalAttack(float damage)
    {
        audioSource.PlayOneShot(skill2AdditionalClip);
        data.CurrentWeapon.AdditionalAttack(damage);
    }

    // Option 2
    // ���� : ����� ���� ��ũ��Ʈ �ʿ���, Ư�� ���� �ʿ��� ������ �� ����
    // ���� : Skill2���� ����� �� �ִ� �޼ҵ���
    public void OnAdditionalSkill2()
    {
        audioSource.PlayOneShot(skill2AdditionalClip);
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

    private IEnumerator SlashEffect()
    {
        yield return new WaitForSeconds(0.6f);
        slashEffect.Play();
        yield return new WaitForSeconds(1f);
        slashEffect.Stop();
    }
}
