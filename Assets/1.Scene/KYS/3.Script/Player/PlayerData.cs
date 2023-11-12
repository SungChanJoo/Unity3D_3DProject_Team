using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData : MonoBehaviour, IDamageable
{
    [SerializeField] private Slider tempHpSlider;
    [SerializeField] private Slider tempStaminaSlider;
    [SerializeField] private Slider tempMpSlider;

    [SerializeField] private Animator tempAnimator;

    [SerializeField] private Sword tempSword;

    [SerializeField] private PlayerAttack attack;

    [SerializeField] private Collider col;


    private float maxMana;
    private float currentMana;

    private float maxStamina;
    private float currentStamina;

    private float walkSpeed;
    private float runSpeed;

    private float maxHealth;
    private float currentHealth;

    private bool isDead = false;

    //무적조건
    public bool invincibility = false;
    public bool IsDead
    {
        get => isDead;
        private set => isDead = value;
    }

    private WeaponBase currentWeapon;
    public WeaponBase CurrentWeapon
    {
        get => currentWeapon;
        set
        {
            if (currentWeapon.Equals(value)) return;

            currentWeapon = value;
        }
    }

    private void Awake()
    {
        // 할당하는 값은 나중에 바꿀 것
        maxHealth = 100;
        maxMana = 100;
        maxStamina = 100;

        currentMana = maxMana;
        currentStamina = maxStamina;
        currentHealth = maxHealth;
        walkSpeed = 5;
        runSpeed = 8;

        // test
        currentWeapon = tempSword;

        tempHpSlider.maxValue = maxHealth;
        tempMpSlider.maxValue = maxMana;
        tempStaminaSlider.maxValue = maxStamina;

        tempMpSlider.value = currentMana;
        tempStaminaSlider.value = currentStamina;
    }

    private void Update()
    {
        // test
        if (Input.GetKeyDown(KeyCode.J))
            TakeDamage(5, 1, Vector3.zero, Vector3.zero);
    }

    private void FixedUpdate()
    {
        RestoreAsTimePass();
    }

    private void RestoreAsTimePass()
    {
        RestoreMana(0.01f);
        RestoreStamina(0.01f);
    }

    public float GetCurrentPlayerSpeed(bool isRunning = false)
    {
        return isRunning ? runSpeed : walkSpeed;
    }

    public void Die()
    {
        IsDead = true;
    }

    public void IncreaseMaxHealth(float modifier)
    {
        maxHealth += modifier;
        tempHpSlider.maxValue = maxHealth;
    }
    public void IncreaseMaxMana(float modifier)
    {
        maxMana += modifier;
        tempMpSlider.maxValue = maxMana;
    }

    /// <summary>
    /// 스태미나 사용이 됐을 경우 true, 스태미나가 부족할 경우 false을 반환
    /// </summary>
    public bool UseStamina(float amount) => Use(ref currentStamina, amount, tempStaminaSlider);

    /// <summary>
    /// 마나 사용이 됐을 경우 true, 마나가 부족할 경우 false을 반환
    /// </summary>
    public bool UseMana(float amount) => Use(ref currentMana, amount, tempMpSlider);

    // IDamageable의 TakeDamage의 범용성이 떨어져서 만든 임시방편. 추후 수정할 거 같음.
    public void TakeDamage(float damage)
    {
        if (currentHealth - damage <= 0)
        {
            Die();
            return;
        }

        currentHealth -= damage;
        tempHpSlider.value = currentHealth;
        
    }


    public void TakeDamage(float damage, float knockback, Vector3 hitPosition, Vector3 hitNomal)
    {
        if (attack.onDefence)
        {
                
            if (UseStamina(10f)&&!attack.perfectParrying)
            {
                TakeDamage(damage * 0.3f);
                attack.hold = true;
            }
            else if (attack.perfectParrying)
            {
                attack.hold = true;
                tempAnimator.SetTrigger("Parry");
                StartCoroutine(Invincibility());

            }
            else
            {
                TakeDamage(damage);
                tempAnimator.SetTrigger("Hit");
                attack.skillEnabled = false;
                attack.hold = true;
            }
            
            
        }
        else if (invincibility)
        {
            //여기에 뭔가해야함 반대로 여기서 공격을 해야한다던지
            return;
            
        }
        else 
        {

            TakeDamage(damage);
            tempAnimator.SetTrigger("Hit");
            attack.skillEnabled = false;
            attack.hold = true;
        }
    }

    private bool Use(ref float target, float amount, Slider slider) // slider는 추후 제거 예정. 어차피 UI에서 세 스탯 보여주니까.
    {
        if (target - amount < 0) return false;

        target -= amount;
        slider.value = target;

        return true;
    }

    public bool RestoreHealth(float amount) => Restore(ref currentHealth, maxHealth, amount, tempHpSlider);
    public bool RestoreStamina(float amount) => Restore(ref currentStamina, maxStamina, amount, tempStaminaSlider);
    public bool RestoreMana(float amount) => Restore(ref currentMana, maxMana, amount, tempMpSlider);

    private bool Restore(ref float target, float max, float amount, Slider slider) // slider는 추후 제거 예정. 어차피 UI에서 세 스탯 보여주니까.
    {
        if (target == max) return false;

        if (target + amount > max)
            amount = max - target;

        target += amount;

        slider.value = target;

        //StackTrace stackTrace = new StackTrace();
        //UnityEngine.Debug.Log("Caller" + stackTrace.GetFrame(1).GetMethod().Name + $"\nValue target - {target} max - {max} amount - {amount}");
        
        return true;
    }

    private IEnumerator Invincibility()
    {
        attack.skillEnabled = false;
        invincibility = true;        
        yield return new WaitForSeconds(2f);
        attack.hold = false;
        invincibility = false;
        attack.skillEnabled = true;
    }
}
