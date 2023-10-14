using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class DeathMark : Skill
    {
        public override bool Cast(Entity target)
        {
            if (!target.IsHostile) return false;
            
            base.Cast(target);

            target.RemoveBuff(STATUS_EFFECT.BLOCK_TOKEN);

            target.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.BLIND_TOKEN, 1, 1));
            target.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.VULNERABLE_TOKEN, 2, 1));
            target.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.DEATHMARK_TOKEN, 1, 2));

            return true;
        }
    }
}
