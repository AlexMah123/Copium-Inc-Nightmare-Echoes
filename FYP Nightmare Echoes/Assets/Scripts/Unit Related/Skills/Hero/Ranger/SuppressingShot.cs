using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class SuppressingShot : Skill
    {
        public override bool Cast(Entity target)
        {
            base.Cast(target);

            if (DealDamage(target))
            {
                if ((stunChance - target.stats.StunResist) > (Random.Range(0, 101)))
                {
                    target.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.STUN_TOKEN, 1, 1));
                }
            }
            

            return true;
        }
    }
}
