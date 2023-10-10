using NightmareEchoes.Grid;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class WarCry : Skill
    {
        public override bool Cast(OverlayTile target, List<OverlayTile> aoeTiles)
        {
            foreach (var entity in from tile in aoeTiles where tile.CheckEntityOnTile() select tile.CheckEntityOnTile().GetComponent<Entity>() into entity where !entity.IsProp select entity)
            {
                entity.AddBuff(entity.IsHostile
                    ? GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.WEAKEN_TOKEN, 1, 1)
                    : GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.STRENGTH_TOKEN, 1, 1));
            }

            return true;
        }

        
    }
}
