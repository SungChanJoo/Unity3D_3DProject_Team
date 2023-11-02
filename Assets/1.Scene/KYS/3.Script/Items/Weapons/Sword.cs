using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class Sword : MonoBehaviour, IWeapon
{
    public void Attack()
    {
        Debug.Log("Attacked");
    }

    public void ChargeAttack()
    {
        Debug.Log("Charge Attacked");
    }

    public void Drop()
    {
        throw new NotImplementedException();
    }

    public void Parry()
    {
        throw new NotImplementedException();
    }

    public void Skill1()
    {
        throw new NotImplementedException();
    }

    public void Skill2()
    {
        throw new NotImplementedException();
    }

    public void Store()
    {
        throw new NotImplementedException();
    }

    public void Use(PlayerData player)
    {
        // ChangeCurrentWeapon용으로 사용할 수도 있고,
        // IITem에서 Use를 아예 뺄 수도 있음.
    }
}