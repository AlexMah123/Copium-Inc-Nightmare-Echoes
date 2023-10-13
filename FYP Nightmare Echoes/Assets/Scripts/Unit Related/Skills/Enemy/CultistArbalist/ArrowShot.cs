using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    //Written by Ter (stolen from jh)
    public class ArrowShot : Skill
    {
        public override bool Cast(Entity target)
        {
            base.Cast(target);

            if(DealDamage(target))
            {
                target.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.WOUND_DEBUFF, 1, 2));
            }


            return true;
        }
    }
}
