using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Animator tempAnimator;
    
    // AttackRate, CurrentWeapon 등의 정보 받아와서 사용
    [SerializeField] private PlayerData data;

    //가드상태를 유지할 때
    private bool hold = false;

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

        Shield();

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

    public void Shield()
    {
        if (Input.GetMouseButtonDown(1))
        {            
            tempAnimator.SetTrigger("Shield");
            hold = true;
           
        }
        else if (Input.GetMouseButtonUp(1))
        {
            hold = false;
        }
        tempAnimator.SetBool("Hold", hold);
    }    

    public IEnumerator Parrying()
    {
        yield return new WaitForSeconds(1f);
    }

    

}
