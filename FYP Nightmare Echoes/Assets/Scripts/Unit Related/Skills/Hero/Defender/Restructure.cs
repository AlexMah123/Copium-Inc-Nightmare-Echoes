using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class Restructure : Skill
    {
        public override bool Cast(Units target)
        {

            base.Cast(target);

            target.stats.Health += heal;
            Debug.Log("Target" + target + "has been healed +" + heal);

            target.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.RESTORATION_BUFF, 3, 1));
            Debug.Log("Applying 3 restore");


            //target.ClearAllStatusEffect(List <ModifierType.DEBUFF>, ModifierType.NEGATIVETOKEN);

            return true;
        }
    }
}
