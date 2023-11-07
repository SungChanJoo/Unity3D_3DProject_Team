using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData : MonoBehaviour, IDamageable
{
    [SerializeField] private Slider tempHpSlider;
    [SerializeField] private Slider tempStaminaSlider;
    [SerializeField] private Slider tempMpSlider;

    [SerializeField] private Animator tempAnimator;

    [SerializeField] private WeaponBase tempWeapon;
    [SerializeField] private Sword tempSword;


    // 아래의 세 변수들도 set하는 경우가 많아지면 currentHealth처럼 private set 부분에서 slider 값 업데이트 하겠음
    private float maxHealth;
    // 아이템 상황에 따라 maxMana, maxStamina 구현해야 할 수도 있음. 근데 그정도로 스케일 안 큰 듯.
    private float mana;
    private float stamina;

    private float walkSpeed;
    private float runSpeed;

    private float currentHealth;
    public float CurrentHealth
    {
        get => currentHealth;
        private set
        {
            currentHealth = value;
            tempHpSlider.value = CurrentHealth;
        }
    }

    //public bool IsDead { get; private set; }

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

    public Action PlayerDiedEvent;

    private void Awake()
    {
        // 할당하는 값은 나중에 바꿀 것
        maxHealth = 100;
        mana = 100;
        stamina = 100;

        CurrentHealth = maxHealth;
        walkSpeed = 5;
        runSpeed = 8;

        //
        currentWeapon = tempSword;

        tempHpSlider.maxValue = maxHealth;
        tempMpSlider.maxValue = mana;
        tempStaminaSlider.maxValue = stamina;

        tempMpSlider.value = mana;
        tempStaminaSlider.value = stamina;
    }

    private void Update()
    {
        // slider test
        //CurrentHealth -= 0.02f;
        //mana -= 0.01f;
        //tempMpSlider.value = mana;
        //stamina -= 0.1f;
        //tempStaminaSlider.value = stamina;

        if (Input.GetKeyDown(KeyCode.J))
            TakeDamage(5, 1, Vector3.zero, Vector3.zero);
    }

    /// <summary>
    /// 스태미나 사용이 됐을 경우 true, 스태미나가 부족할 경우 false을 반환
    /// </summary>
    public bool UseStamina(float amount)
    {
        if (stamina - amount < 0) return false;

        stamina -= amount;
        tempStaminaSlider.value = stamina;

        return true;
    }

    /// <summary>
    /// 마나 사용이 됐을 경우 true, 마나가 부족할 경우 false을 반환
    /// </summary>
    public bool UseMana(float amount)
    {
        if (mana - amount < 0) return false;

        mana -= amount;
        tempMpSlider.value = mana;

        return true;
    }

    // 추후 State 패턴을 사용한다면 CameraController 쪽에서 bool 값을 파라미터로 넘겨주는 게 아니라
    // PlayerData에서 알아서 State에 따라 speed 값 넘겨주는 것으로 수정하는 것이 좋을 듯
    public float GetCurrentPlayerSpeed(bool isRunning = false)
    {
        return isRunning ? runSpeed : walkSpeed;
    }

    //public bool ChangeCurrentWeapon(IWeapon weapon) // WeaponBase
    //{
    //    // 직접 비교하는 게 아니라 안의 Weapon이라는 enum으로 무기 타입 비교하든지 하기
    //    if (weapon.Equals(currentWeapon))
    //        return false;

    //    return true;
    //}

    // 임시방편
    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;

        if (CurrentHealth < 0)
            Die();
    }

    // knockback 관련 이벤트를 만들어서 playermovement가 sub하게 할까 말까
    public void TakeDamage(float damage, float knockback, Vector3 hitPosition, Vector3 hitNomal)
    {
        // IsParrying 등의 State에 따른 처리 요구
        CurrentHealth -= damage;

        tempAnimator.SetTrigger("Hit");

        if (CurrentHealth < 0)
            Die();
    }

    public void Die()
    {
        PlayerDiedEvent.Invoke();
    }

    #region 아이템이 사용될 때 아이템 쪽에서 접근할 메소드
    // 최대 체력 늘려주는 아이템
    public void IncreaseMaxHealth(float modifier)
    {
        maxHealth += modifier;
    }

    // 체력 회복 아이템
    // 이미 풀피일 때 회복하는 거 방지하고 싶으면 bool값 리턴하기
    public void RestoreHealth(float modifier)
    {
        if (modifier + CurrentHealth > maxHealth)
            modifier = maxHealth - CurrentHealth;

        CurrentHealth += modifier;
    }
    #endregion
}
