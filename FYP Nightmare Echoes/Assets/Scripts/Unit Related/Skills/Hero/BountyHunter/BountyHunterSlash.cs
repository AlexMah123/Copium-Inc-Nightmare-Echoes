using System.Collections;
using System.Collections.Generic;
using NightmareEchoes.Grid;
using NightmareEchoes.Unit.Combat;
using UnityEngine;

//Created by Vinn
namespace NightmareEchoes.Unit
{
    public class BountyHunterSlash : Skill
    {
        public override bool Cast(Entity target)
        {
            base.Cast(target);

            DealDamage(target);
            target.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.WOUND_DEBUFF, 2, 1));

            return true;
        }
    }
}
