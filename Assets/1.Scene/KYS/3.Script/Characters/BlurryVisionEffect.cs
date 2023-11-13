using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class BlurryVisionEffect : StatusEffect
{
    public BlurryVisionEffect(StatusEffectedCharacter target) : base(StatusEffectType.BlurryVision, target) { }

    public override void ApplyEffect()
    {
        // TODO : Post-processing
    }
}