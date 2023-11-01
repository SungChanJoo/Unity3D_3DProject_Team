using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData : MonoBehaviour, IDamageable
{
    [SerializeField] private Slider tempHpSlider;
    [SerializeField] private Slider tempMpSlider;
    [SerializeField] private Slider tempStaminaSlider;

    // �Ʒ��� �� �����鵵 set�ϴ� ��찡 �������� currentHealthó�� private set �κп��� slider �� ������Ʈ �ϰ���
    private float maxHealth;
    // ������ ��Ȳ�� ���� maxMana, maxStamina �����ؾ� �� ���� ����. �ٵ� �������� ������ �� ū ��.
    private float mana;
    private float stamina;

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
    public float MoveSpeed { get; private set; } 
    public float Damage { get; private set; }
    public float AttackRate { get; private set; }

    //public bool IsDead { get; private set; }

    // ���� �޼ҵ� ������ ���Ŀ� �� ��
    private List<StatusEffect> statusEffects;

    private IWeapon currentWeapon;
    // CurrentArmor�� �־�� �ϴ� Aromor�� ���� ����� �� �� ����� �� �� ���� ��

    // StatusEffect �� �����ϱ� ���� �ʵ��� List�� �ƴ� Array�� �Ҵ�
    public Action<StatusEffect[]> StatusEffectChangedEvent;
    public Action PlayerDiedEvent;


    private void Awake()
    {
        // �Ҵ��ϴ� ���� ���߿� �ٲ� ��
        maxHealth = 100;
        mana = 100;
        stamina = 100;

        CurrentHealth = maxHealth;
        MoveSpeed = 5;
        Damage = 10;
        AttackRate = 1;

        currentWeapon = null;

        tempHpSlider.maxValue = maxHealth;
        tempMpSlider.maxValue = mana;
        tempStaminaSlider.maxValue = stamina;

        tempMpSlider.value = mana;
        tempStaminaSlider.value = stamina;
    }

    private void Update()
    {
        // test
        CurrentHealth -= 0.02f;
        mana -= 0.01f;
        tempMpSlider.value = mana;
        stamina -= 0.1f;
        tempStaminaSlider.value = stamina;

        Debug.Log($"hp:{CurrentHealth}\nmp:{mana}\nstamina:{stamina}");
    }

    /// <summary>
    /// ���� ����� ���� ��� true, ������ ������ ��� false�� ��ȯ
    /// </summary>
    public bool UseMana(float manaToUse)
    {
        if (mana - manaToUse < 0) return false;

        mana -= manaToUse;
        tempMpSlider.value = mana;

        return true;
    }

    /// <summary>
    /// ���¹̳� ����� ���� ��� true, ���¹̳��� ������ ��� false�� ��ȯ
    /// </summary>
    public bool UseStamina(float staminaToUse)
    {
        if (stamina - staminaToUse < 0) return false;

        stamina -= staminaToUse;
        tempStaminaSlider.value = stamina;

        return true;
    }

    public bool ChangeCurrentWeapon(IWeapon weapon)
    {
        // ���� ���ϴ� �� �ƴ϶� ���� Weapon�̶�� enum���� ���� Ÿ�� ���ϵ��� �ϱ�
        if (weapon.Equals(currentWeapon))
            return false;

        return true;
    }

    // knockback ���� �̺�Ʈ�� ���� playermovement�� sub�ϰ� �ұ� ����
    public void TakeDamage(float damage, float knockback, Vector3 hitPosition, Vector3 hitNomal)
    {
        // IsParrying ���� State�� ���� ó�� �䱸
        CurrentHealth -= damage;

        if (CurrentHealth < 0)
            Die();
    }

    public void Die()
    {
        PlayerDiedEvent.Invoke();
    }

    #region �������� ���� �� ������ �ʿ��� ������ �޼ҵ�
    // �ִ� ü�� �÷��ִ� ������
    public void IncreaseMaxHealth(float modifier)
    {
        maxHealth += modifier;
    }

    public void IncreaseDamage(float modifier)
    {
        Damage += modifier;
    }

    // ü�� ȸ�� ������
    // �̹� Ǯ���� �� ȸ���ϴ� �� �����ϰ� ������ bool�� �����ϱ�
    public void RestoreHealth(float modifier)
    {
        if (modifier + CurrentHealth > maxHealth)
            modifier = maxHealth - CurrentHealth;

        CurrentHealth += modifier;
    }
    #endregion
}
