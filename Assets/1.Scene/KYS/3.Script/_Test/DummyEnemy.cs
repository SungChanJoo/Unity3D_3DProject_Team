using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DummyEnemy : MonoBehaviour, IDamageable
{
    [SerializeField] private Slider hpSlider;

    private float hp = 100f;

    private void Awake()
    {
        hpSlider.maxValue = hp;
        hpSlider.value = hp;
    }

    public void TakeDamage(float damage, float knockBack, Vector3 hitposition, Vector3 hitNomal)
    {
        Debug.Log("Dummy Enemy Took Damage");
        hp -= damage;
        hpSlider.value = hp;

        if (hp < 0)
            Debug.Log("Enemy Died");
    }
}
