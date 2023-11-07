using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class BlurryVisionEffect : StatusEffect
{
    public BlurryVisionEffect(StatusEffectedCharacter target) : base(StatusEffectType.BlurryVision, target, "Hold") { }

    public override void ApplyEffect()
    {
        // UI 쪽에서 BlurryVisionEffect 저장/삭제에 따른 효과 적용하면 될 듯
    }
}