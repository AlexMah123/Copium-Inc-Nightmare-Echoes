using System.Collections;
using System.Collections.Generic;
using NightmareEchoes.Grid;
using NightmareEchoes.Unit.Combat;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class ShadowStep : Skill
    {
        public override bool Cast()
        {
            if (thisUnit.FindModifier(STATUS_EFFECT.STEALTH_TOKEN)) return false;
            
            //Get enemies in nearby proximity
            var grid = CombatManager.Instance.SquareRange(thisUnit.ActiveTile, 2);
            var cleanedGrid = OverlayTileManager.Instance.TrimOutOfBounds(grid);
            var enemiesInRange = new List<Entity>();
            foreach (var tile in cleanedGrid)
            {
                if (!tile.CheckUnitOnTile()) continue;
                var target = tile.CheckUnitOnTile().GetComponent<Entity>();
                if (!target.IsHostile || target.IsProp) continue;
                enemiesInRange.Add(target);
            }
            
            //Check if this unit is in range of said enemies
            foreach (var enemy in enemiesInRange)
            {
                grid = CombatManager.Instance.FrontalRange(enemy.ActiveTile, 1, enemy);
                cleanedGrid = OverlayTileManager.Instance.TrimOutOfBounds(grid);

                foreach (var tile in cleanedGrid)
                {
                    if (thisUnit.ActiveTile == tile)
                        return false;
                }
            }
            
            thisUnit.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.STRENGTH_TOKEN, 1, 3));
            thisUnit.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.STEALTH_TOKEN, 1, 3));
            
            return true;
        }
    }
}