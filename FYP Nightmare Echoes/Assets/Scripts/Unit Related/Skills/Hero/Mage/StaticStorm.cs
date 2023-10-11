using System.Collections;
using System.Collections.Generic;
using NightmareEchoes.Grid;
using NightmareEchoes.Unit.Combat;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class StaticStorm : Skill
    {
        public override bool Cast(Entity target)
        {
            DealDamage(target);

            return true;
        }

        public override bool Cast(OverlayTile target, List<OverlayTile> aoeTiles)
        {
            base.Cast(target, aoeTiles);
            
            var copy = new List<OverlayTile>(aoeTiles);
            
            foreach (var tile in aoeTiles)
            {
                if (!tile.CheckEntityGameObjectOnTile()) continue;
  
                var unit = tile.CheckEntityGameObjectOnTile().GetComponent<Entity>();
                DealDamage(unit);
            }

            CombatManager.Instance.SetActiveAoe(this, copy);
            return true;
        }
    }
}
