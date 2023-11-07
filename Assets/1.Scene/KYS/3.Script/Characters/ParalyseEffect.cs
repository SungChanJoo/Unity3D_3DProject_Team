using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ParalyseEffect : StatusEffect
{
    public ParalyseEffect(StatusEffectedCharacter target) : base(StatusEffectType.Paralysed, target, "ChargeAttack") { }

    public override void ApplyEffect()
    {
        // CameraController 쪽에서 ParalyseEffect의 저장/삭제에 따른 효과 적용
    }
}