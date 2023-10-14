using NightmareEchoes.Grid;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class WarCry : Skill
    {
        public override bool Cast(OverlayTile target, List<OverlayTile> aoeTiles)
        {
            foreach (var entity in from tile in aoeTiles where tile.CheckEntityGameObjectOnTile() select tile.CheckEntityGameObjectOnTile().GetComponent<Entity>() into entity where !entity.IsProp select entity)
            {
                switch (entity.IsHostile)
                {
                    case false:
                        entity.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.STRENGTH_TOKEN, 1, 1));
                        break;
                    case true:
                        if (debuffChance - entity.stats.Resist > Random.Range(1,101))
                        {
                            entity.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.WEAKEN_TOKEN, 1, 1));
                            entity.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.SPEED_DEBUFF, 3, 1));
                        }
                        break;
                }
            }

            return true;
        }

        
    }
}
