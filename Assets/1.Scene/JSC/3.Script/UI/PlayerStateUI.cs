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

    [Header("독 -> 마비 -> 어지러움")]
    [SerializeField] private List<GameObject> stateList;
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
        int rand = Random.Range(0,3);
        ViewStateUI(StatusEffectType.Poisoned, 5f);
        ViewStateUI(StatusEffectType.Paralysed, 10f);
        ViewStateUI(StatusEffectType.Dizzy, 15f);
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

    private IEnumerator currentPoisoned = null;
    private IEnumerator currentParalysed = null;
    private IEnumerator currentDizzy = null;

    public void ViewStateUI(StatusEffectType statusEffectType, float duration)
    {
        switch (statusEffectType)
        {
            case StatusEffectType.Poisoned:
                stateList[0].GetComponent<StateEffectedUI>().image.color = new Color(stateList[0].GetComponent<StateEffectedUI>().image.color.r,
                                                                                     stateList[0].GetComponent<StateEffectedUI>().image.color.g, 
                                                                                     stateList[0].GetComponent<StateEffectedUI>().image.color.b, 1f);
                if (currentPoisoned != null)
                {
                    StopCoroutine(currentPoisoned);
                    // 즉시 중단하게 되면 해줘야할 로직
                    stateList[0].SetActive(false);
                    stateList[0].GetComponent<StateEffectedUI>().image.color = new Color(stateList[0].GetComponent<StateEffectedUI>().image.color.r,
                                                                                 stateList[0].GetComponent<StateEffectedUI>().image.color.g,
                                                                                 stateList[0].GetComponent<StateEffectedUI>().image.color.b, 1f);
                    currentPoisoned = null;
                }
                stateList[0].SetActive(true);

                currentPoisoned = stateList[0].GetComponent<StateEffectedUI>().SetDuration_co(duration);
                StartCoroutine(currentPoisoned);
                break;

            case StatusEffectType.Paralysed:
                stateList[1].GetComponent<StateEffectedUI>().image.color = new Color(stateList[1].GetComponent<StateEffectedUI>().image.color.r,
                                                                                     stateList[1].GetComponent<StateEffectedUI>().image.color.g,
                                                                                     stateList[1].GetComponent<StateEffectedUI>().image.color.b, 1f);

                if (currentParalysed != null)
                {
                    StopCoroutine(currentParalysed);
                    // 즉시 중단하게 되면 해줘야할 로직
                    stateList[1].SetActive(false);
                    stateList[1].GetComponent<StateEffectedUI>().image.color = new Color(stateList[1].GetComponent<StateEffectedUI>().image.color.r,
                                                                                         stateList[1].GetComponent<StateEffectedUI>().image.color.g,
                                                                                         stateList[1].GetComponent<StateEffectedUI>().image.color.b, 1f);
                    currentParalysed = null;
                }
                stateList[1].SetActive(true);
                currentParalysed = stateList[1].GetComponent<StateEffectedUI>().SetDuration_co(duration);
                StartCoroutine(currentParalysed);
                break;

            case StatusEffectType.Dizzy:
                stateList[2].GetComponent<StateEffectedUI>().image.color = new Color(stateList[2].GetComponent<StateEffectedUI>().image.color.r,
                                                                                     stateList[2].GetComponent<StateEffectedUI>().image.color.g,
                                                                                     stateList[2].GetComponent<StateEffectedUI>().image.color.b, 1f);
                if (currentDizzy != null)
                {
                    StopCoroutine(currentDizzy);
                    // 즉시 중단하게 되면 해줘야할 로직
                    stateList[2].SetActive(false);
                    stateList[2].GetComponent<StateEffectedUI>().image.color = new Color(stateList[2].GetComponent<StateEffectedUI>().image.color.r,
                                                                                         stateList[2].GetComponent<StateEffectedUI>().image.color.g,
                                                                                         stateList[2].GetComponent<StateEffectedUI>().image.color.b, 1f);
                    currentDizzy = null;
                }
                stateList[2].SetActive(true);
                currentDizzy = stateList[2].GetComponent<StateEffectedUI>().SetDuration_co(duration);
                StartCoroutine(currentDizzy);
                break;

        }

    }

}
