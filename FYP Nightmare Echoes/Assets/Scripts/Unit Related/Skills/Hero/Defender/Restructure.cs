using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class Restructure : Skill
    {
        public override bool Cast(Entity target)
        {

            base.Cast(target);

            target.stats.Health += heal;

            target.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.RESTORATION_BUFF, 3, 1));


            target.ClearAllStatusEffect(target.TokenList, ModifierType.NEGATIVETOKEN);

            return true;
        }
    }
}
