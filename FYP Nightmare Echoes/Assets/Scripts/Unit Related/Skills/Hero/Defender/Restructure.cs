using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class Restructure : Skill
    {
        public override bool Cast()
        {
            thisUnit.stats.Health += heal;
            //thisUnit.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.RESTORATION_BUFF, 3, 1));
            thisUnit.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.BARRIER_TOKEN, 1, 1));
            thisUnit.ClearAllStatusEffectOfType(ModifierType.NEGATIVETOKEN);

            return true;
        }
    }
}
