using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    //Written by Ter (stolen from jh)
    public class ShieldBash : Skill
    {
        public override bool Cast(Units target)
        {
            base.Cast(target);

            if(DealDamage(target))
            {
                if ((StunChance - target.stats.StunResist) > (Random.Range(0,101)))
                {
                    target.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.STUN_TOKEN, 1, 1));
                }
            }

            

            return true;
        }
    }
}
