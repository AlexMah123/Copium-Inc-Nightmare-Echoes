using System.Collections;
using System.Collections.Generic;
using NightmareEchoes.Grid;
using NightmareEchoes.Unit.Combat;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class StaticStorm : Skill
    {
        public override bool Cast(Units target)
        {
            base.Cast(target);
            target.TakeDamage(damage);
            return true;
        }

        public override bool Cast(OverlayTile target, List<OverlayTile> aoeTiles)
        {
            var copy = new List<OverlayTile>(aoeTiles);
            
            foreach (var tile in aoeTiles)
            {
                if (!tile.CheckUnitOnTile()) continue;
  
                var unit = tile.CheckUnitOnTile().GetComponent<Units>();
                unit.TakeDamage(damage);
            }

            CombatManager.Instance.SetActiveAoe(this, copy);
            return true;
        }
    }
}
