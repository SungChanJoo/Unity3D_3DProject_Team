using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateUI : MonoBehaviour
{
    [SerializeField] private PlayerData player;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Slider manaSlider;
    [SerializeField] private AnimationCurve timeOverSpeed;

    private void Awake()
    {
        Debug.Log(player.MaxHealth);
        hpSlider.maxValue = player.MaxHealth;
        hpSlider.value = player.MaxHealth;
        staminaSlider.maxValue = player.MaxStamina;
        staminaSlider.value = player.MaxHealth;
        manaSlider.maxValue = player.MaxMana;
        manaSlider.value = player.MaxHealth;
    }

    public void UpdateHp()
    {
        Debug.Log("플레이어 현재 피 : " + player.CurrentHealth);

        hpSlider.value = player.CurrentHealth; 
    }
    public void UpdateStamina()
    {
        staminaSlider.value = player.CurrentStamina;
    }
    public void UpdateMana()
    {
        manaSlider.value = player.CurrentMana;
    }


}
