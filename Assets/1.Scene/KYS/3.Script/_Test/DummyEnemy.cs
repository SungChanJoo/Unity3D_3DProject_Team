using UnityEngine;
using UnityEngine.UI;

// �÷��̾� �׼� test�� ���ʹ�. ���� ������� ����.
public class DummyEnemy : Enemy, IDamageable
{
    [SerializeField] private Slider tempHpSlider;

    private float hp = 500f;

    private void Awake()
    {
        tempHpSlider.maxValue = hp;
        tempHpSlider.value = hp;
    }

    public void TakeDamage(float damage, float knockBack, Vector3 hitposition, Vector3 hitNomal)
    {
        Debug.Log($"Dummy Enemy Took Damage : {damage}");
        hp -= damage;
        tempHpSlider.value = hp;

        if (hp < 0)
            Debug.Log("Enemy Died");
    }
}
