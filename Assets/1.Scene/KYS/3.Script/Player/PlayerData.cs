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


    private float maxMana;
    private float currentMana;

    private float maxStamina;
    private float currentStamina;

    private float walkSpeed;
    private float runSpeed;

    private float maxHealth;
    private float currentHealth;

    private bool isDead = false;
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
            // 직접 비교하는 게 아니라 안의 Weapon이라는 enum으로 무기 타입 비교하든지 하기
            if (currentWeapon.Equals(value)) return;

            currentWeapon = value;
        }
    }
    // CurrentArmor도 넣어야 하는 Aromor는 구현 방식을 좀 더 고민해 본 뒤 넣을 것

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

        if (Input.GetKeyDown(KeyCode.N))
            RestoreMana(10);
    }

    private void FixedUpdate()
    {
        RestoreMana(0.1f);
        RestoreStamina(0.1f);
    }


    //public bool ChangeCurrentWeapon(IWeapon weapon) // WeaponBase
    //{
    //    // 직접 비교하는 게 아니라 안의 Weapon이라는 enum으로 무기 타입 비교하든지 하기
    //    if (weapon.Equals(currentWeapon))
    //        return false;

    //    return true;
    //}

    // 추후 State 패턴을 사용한다면 CameraController 쪽에서 bool 값을 파라미터로 넘겨주는 게 아니라
    // PlayerData에서 알아서 State에 따라 speed 값 넘겨주는 것으로 수정하는 것이 좋을 듯
    public float GetCurrentPlayerSpeed(bool isRunning = false)
    {
        return isRunning ? runSpeed : walkSpeed;
    }

    public void Die()
    {
        IsDead = true;
    }

    // 최대 체력 늘려주는 아이템
    public void IncreaseMaxHealth(float modifier)
    {
        maxHealth += modifier;
        // slider max value 변경
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

    // knockback 관련 이벤트를 만들어서 playermovement가 sub하게 할까 말까
    public void TakeDamage(float damage, float knockback, Vector3 hitPosition, Vector3 hitNomal)
    {
        TakeDamage(damage);
        tempAnimator.SetTrigger("Hit");
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
}
