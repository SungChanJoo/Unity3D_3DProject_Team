using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Animator tempAnimator;

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
    }

    public void ChargeAttack()
    {
        tempAnimator.SetTrigger("ChargeAttack");
    }
}
