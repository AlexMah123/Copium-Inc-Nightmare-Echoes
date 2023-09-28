using NightmareEchoes.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class WarCry : Skill
    {
        public override bool Cast(OverlayTile target, List<OverlayTile> aoeTiles)
        {
            base.Cast(target, aoeTiles);

            var targetType = target.CheckUnitOnTile().GetComponent<Units>();

            if (targetType.IsHostile == false)
            {
                targetType.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.STRENGTH_TOKEN, 1, 1));
                Debug.Log("Applying Strength");
            }
            if (targetType.IsHostile == true)
            {
                targetType.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.WEAKEN_TOKEN, 1, 1));
                Debug.Log("Applying Weaken");
            }

            return true;
        }

        
    }
}
