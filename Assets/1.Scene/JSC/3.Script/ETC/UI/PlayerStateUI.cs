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

    private void Awake()
    {
        Debug.Log(player.MaxHealth);
        hpSlider.value = player.MaxHealth;
        staminaSlider.value = player.MaxStamina;
        manaSlider.value = player.MaxMana;
    }

    public void OnUpdateHp()
    {
        hpSlider.value = player.CurrentHealth; 
    }
    public void OnUpdatestamina()
    {
        staminaSlider.value = player.CurrentStamina;
    }
    public void OnUpdatemana()
    {
        manaSlider.value = player.CurrentMana;
    }
}
