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
    //[SerializeField] private AnimationCurve timeOverSpeed;

    public void InitState(float hp, float statmina, float mana)
    {
        hpSlider.maxValue = hp;
        hpSlider.value = hp;
        staminaSlider.maxValue = statmina;
        staminaSlider.value = statmina;
        manaSlider.maxValue = mana;
        manaSlider.value = mana;
    }

    public void UpdateHp()
    {
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

    public void IncreaseMaxHp(float value)
    {
        hpSlider.maxValue += value;
        hpSlider.value += value;
    }
    public void IncreaseMaxMana(float value)
    {
        manaSlider.maxValue += value;
        manaSlider.value += value;
    }
}
