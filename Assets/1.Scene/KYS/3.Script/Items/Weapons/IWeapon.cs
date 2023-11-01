using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface IWeapon
{
    public void Attack();

    public void ChargeAttack();

    public void Parry();

    public void Skill1();
 
    public void Skill2();
}