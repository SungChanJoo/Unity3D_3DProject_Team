using System.Collections;
using UnityEngine;
using UnityEngine.VFX;


public enum States
{
    Idle,
    Attack,
    Skillmove,
    Shield,
    Hit,
    Rolling,
}

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

    // AttackRate, CurrentWeapon 
    private PlayerData data;
    private Animator tempAnimator;
    private CameraController controller;

    
    public bool onDefence = false;    
    public bool perfectParrying = false;
    public bool isActing = false;

    private bool mana;    

    [SerializeField] private ParticleSystem skill_1E;
    [SerializeField] private ParticleSystem skill_2E;
    [SerializeField] private Transform skill_2E_Position;
    [SerializeField] private VisualEffect slashEffect;

    public States state = States.Idle;

    private void Awake()
    {
        tempAnimator = GetComponent<Animator>();
        data = GetComponent<PlayerData>();
        controller = GetComponent<CameraController>();
        if (data==null)
        {
            Debug.Log("null인디?");
        }
        slashEffect.Stop();
        
    }

    public void OnAttackingAnimationCompleted()
    {
        isActing = false;
        state = States.Idle;
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
        if (state != States.Attack)
        {
            Shield();
        }

        if (isActing) return;

        if (state ==States.Idle)
        {
            
            if (Input.GetMouseButtonUp(0))
            {                
                Attack();                
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {                
                Skill1();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {                
                Skill2();                        
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {                
                ChargeAttack(); 
            }
            
        }        
        
    }    
    

    public void Attack()
    {
        isActing = true;
        state = States.Attack;
        tempAnimator.SetTrigger("Attack");
        data.CurrentWeapon.Attack();
        StartCoroutine(SlashEffect());
    }

    public void ChargeAttack()
    {
        isActing = true;
        state = States.Attack;
        tempAnimator.SetTrigger("ChargeAttack");
        data.CurrentWeapon.ChargeAttack();
        
    }

    public void Shield()
    {
        if (Input.GetMouseButtonDown(1))
        {
            state = States.Shield;
            isActing = true;
            tempAnimator.SetTrigger("Shield");            
            onDefence = true;            
        }        
        else if (Input.GetMouseButtonUp(1))
        {
            state = States.Idle;
            isActing = false;           
            onDefence = false;            
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
        state = States.Attack;
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

    public void Skill2()
    {
        isActing = true;
        state = States.Attack;
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

    
    public void OnAdditionalAttack(float damage)
    {
        audioSource.PlayOneShot(skill2AdditionalClip);
        data.CurrentWeapon.AdditionalAttack(damage);
    }
    
    public void OnAdditionalSkill2()
    {
        audioSource.PlayOneShot(skill2AdditionalClip);
        data.CurrentWeapon.AdditionalSkill2();
    }

    #region // 스킬 애니메이션 이벤트 상태 설정
    private void SkillMove()
    {
        state = States.Skillmove;

    }
    private void SkillIdle()
    {
        state = States.Attack;

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
