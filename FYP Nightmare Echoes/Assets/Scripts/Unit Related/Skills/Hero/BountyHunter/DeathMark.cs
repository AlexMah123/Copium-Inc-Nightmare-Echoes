using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class DeathMark : Skill
    {
        public override bool Cast(Units target)
        {
            base.Cast(target);

            target.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.BLIND_TOKEN, 1, 1));
            Debug.Log("Applying 1 blind");
            target.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.VULNERABLE_TOKEN, 2, 1));
            Debug.Log("Applying 2 Vulnerable");

            return true;
        }
    }
}
