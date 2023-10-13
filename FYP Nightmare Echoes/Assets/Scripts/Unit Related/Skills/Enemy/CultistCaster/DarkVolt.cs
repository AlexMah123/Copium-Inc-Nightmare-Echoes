using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    //Written by Ter (stolen from jh)
    public class DarkVolt : Skill
    {
        public override bool Cast(Entity target)
        {
            base.Cast(target);

            if(DealDamage(target))
            {
                target.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.WEAKEN_TOKEN, 1, 1));
            }


            return true;
        }
    }
}
