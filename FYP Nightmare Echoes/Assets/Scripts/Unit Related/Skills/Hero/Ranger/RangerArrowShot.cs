using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class RangerArrowShot : Skill
    {
       public override bool Cast(Entity target)
        {
            base.Cast(target);

            DealDamage(target);
            target.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.WOUND_DEBUFF, 3, 1));

            return true;
        }
    }
}
