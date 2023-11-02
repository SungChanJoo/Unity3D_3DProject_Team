using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Animator tempAnimator;
    
    // AttackRate, CurrentWeapon 등의 정보 받아와서 사용
    [SerializeField] private PlayerData data;

    public void OnAttackingAnimationCompleted()
    {
        data.CurrentWeapon.DisableDamaging();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            Attack();

        else if (Input.GetKeyDown(KeyCode.L))
            ChargeAttack();
    }

    public void Attack()
    {
        tempAnimator.SetTrigger("Attack");
        data.CurrentWeapon.Attack();
    }

    public void ChargeAttack()
    {
        tempAnimator.SetTrigger("ChargeAttack");
        data.CurrentWeapon.ChargeAttack();
    }
}
