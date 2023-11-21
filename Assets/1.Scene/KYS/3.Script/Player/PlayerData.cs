using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour, IDamageable
{
    [SerializeField] private Animator tempAnimator;

    [SerializeField] private Sword tempSword;

    [SerializeField] private PlayerAttack attack;

    [SerializeField] private Collider col;

    [SerializeField] private PlayerStateUI playerStateUI;

    [SerializeField] private GameOver gameOver;

    [SerializeField] private PlayerDataJson playerData;

    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip getHitClip;

    private List<IItem> items = new List<IItem>();
        
    public Action<List<IItem>> ItemChangedEvent;

    public bool stop = false;

    private Rigidbody rigid;

    // 아이템 추가용
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out IItem item)) return;

        items.Add(item);
        ItemChangedEvent?.Invoke(items);

        Destroy(other.gameObject);
        Debug.Log(item.Name + "을 획득. 인벤토리에 아이템이 " + items.Count + "만큼 있음");
    }

    public void UseItem(IItem usedItem)
    {
        usedItem.Use(this);
        items.Remove(usedItem);
        ItemChangedEvent?.Invoke(items);
        Debug.Log(usedItem.Name + "을 사용. 인벤토리에 아이템이 " + items.Count + "만큼 있음");
    }

    private float maxMana;
    public float MaxMana
    {
        get => maxMana;
        private set
        {
            maxMana = value;
            // UI_슬라이더
            // UI쪽에 바뀐 값 넘겨주기
        }
    }
    private float currentMana;
    public float CurrentMana
    {
        get => currentMana;
        private set
        {
            currentMana = value;

            if(playerStateUI != null)
               playerStateUI.UpdateMana();
        }
    }

    private float maxStamina;
    public float MaxStamina
    {
        get => maxStamina;
        private set
        {
            maxStamina = value;
        }
    }
    private float currentStamina;
    public float CurrentStamina
    {
        get => currentStamina;
        private set
        {
            currentStamina = value;

            if (playerStateUI != null)
                playerStateUI.UpdateStamina();
        }
    }

    private float maxHealth;
    public float MaxHealth
    {
        get => maxHealth;
        private set
        {
            maxHealth = value;
            // UI_슬라이더
            // UI쪽에 바뀐 값 넘겨주기
        }
    }
    private float currentHealth;
    public float CurrentHealth
    {
        get => currentHealth;
        private set
        {
            currentHealth = value;

        }
    }

    private float walkSpeed;
    private float runSpeed;

    private bool isDead = false;

    //무적조건
    
    public bool IsDead
    {
        get => isDead;
        private set => isDead = value;
    }

    private WeaponBase currentWeapon;
    public WeaponBase CurrentWeapon
    {
        get => currentWeapon;
        set => currentWeapon = value;
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();

        MaxHealth = 100;
        MaxMana = 100;
        MaxStamina = 100;

        CurrentMana = MaxMana;
        CurrentStamina = MaxStamina;
        CurrentHealth = MaxHealth;
        walkSpeed = 2;
        runSpeed = 5;
        CurrentWeapon = tempSword;
        if(playerStateUI != null)
            playerStateUI.InitState(MaxHealth, MaxStamina, MaxMana);
    }

    private void Start()
    {
        try
        {
            playerData = GameManager.Instance.Load();
            MaxHealth = playerData.maxHealth;
            MaxMana = playerData.maxMana;
            MaxStamina = playerData.maxStamina;

            CurrentMana = playerData.currentMana;
            CurrentStamina = playerData.currentStamina;
            CurrentHealth = playerData.currentHealth;

            transform.position = new Vector3(playerData.PlayerPosition_x,
                                             playerData.PlayerPosition_y,
                                             playerData.PlayerPosition_z);
        }
        catch (Exception e)
        {
            Debug.Log("플레이어 데이터로드 실패, (새 게임이거나 인트로를 통해서 실행하지 않았을 경우");
        }
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
        
        tempAnimator.SetTrigger("Die");
        gameOver.LoadGameOver(); //gameover UI 호출
    }


    // IDamageable의 TakeDamage의 범용성이 떨어져서 만든 임시방편. 추후 수정할 거 같음.
    public void TakeDamage(float damage)
    {
        if (currentHealth - damage <= 0)
        {
            Die();
            return;
        }

        currentHealth -= damage;
    }

    public void TakeDamage(float damage, float knockback, Vector3 hitPosition, Vector3 hitNomal)
    {
        audioSource.PlayOneShot(getHitClip);
        Debug.Log("Hp" + currentHealth);

        if (playerStateUI != null)
            playerStateUI.UpdateHp();
        stop = true;
        if (currentHealth - damage <= 0)
        {
            Die();
            this.gameObject.tag = "Enemy";
            Debug.Log("플레이어 뒤짐");
            return;
        }
        else
        {


            if (attack.onDefence)
            {

                if (UseStamina(10f) && !attack.perfectParrying)
                {
                    TakeDamage(damage * 0.3f);
                    attack.hold = true;
                    rigid.AddForce(-transform.forward * 100f, ForceMode.Impulse);
                    Debug.Log("맞음");
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
                    attack.attackEnabled = false;
                    attack.hold = true;
                    attack.charging = false;
                }


            }
            else
            {

                TakeDamage(damage);
                tempAnimator.SetTrigger("Hit");
                attack.attackEnabled = false;
                attack.hold = true;
            }
        }
        StartCoroutine(TakeDamgeAni());


    }

    public void IncreaseMaxHealth(float modifier) => MaxHealth += modifier;
    public void IncreaseMaxMana(float modifier) => MaxMana += modifier;

    /// <summary>
    /// 스태미나 사용이 됐을 경우 true, 스태미나가 부족할 경우 false을 반환
    /// </summary>
    public bool UseStamina(float amount)
    {
        if (CurrentStamina - amount < 0) return false;

        CurrentStamina -= amount;

        return true;
    }

    /// <summary>
    /// 마나 사용이 됐을 경우 true, 마나가 부족할 경우 false을 반환
    /// </summary>
    public bool UseMana(float amount)
    {
        if (CurrentMana - amount < 0) return false;

        CurrentMana -= amount;

        return true;
    }

    public bool RestoreHealth(float amount)
    {
        if (CurrentHealth == MaxHealth) return false;

        if (CurrentHealth + amount > MaxHealth)
            amount = MaxHealth - CurrentHealth;

        CurrentHealth += amount;

        return true;
    }

    public bool RestoreStamina(float amount)
    {
        if (CurrentStamina == MaxStamina) return false;

        if (CurrentStamina + amount > MaxStamina)
            amount = MaxStamina - CurrentStamina;

        CurrentStamina += amount;

        return true;
    }

    public bool RestoreMana(float amount)
    {
        if (CurrentMana == MaxMana) return false;

        if (CurrentMana + amount > MaxMana)
            amount = MaxMana - CurrentMana;

        CurrentMana += amount;

        return true;
    }

    private IEnumerator Invincibility()
    {        
        this.gameObject.tag = "Enemy";
        yield return new WaitForSeconds(2f);
        attack.hold = false;
        this.gameObject.tag = "Player";        
    }
    private IEnumerator TakeDamgeAni()
    {
        
        yield return new WaitForSeconds(0.667f);
        attack.attackEnabled = true;
    }
}
