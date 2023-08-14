using System.Collections.Generic;
using NightmareEchoes.Grid;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class ArcaneBlast : Skill
    {
        [SerializeField] private int secondaryDamage = 4;

        public override bool Cast(Units target)
        {
            throw new System.NotImplementedException();
        }
        
        public override bool Cast(OverlayTile target, List<OverlayTile> aoeTiles)
        {
            if (target.CheckUnitOnTile())
            {
                var unit = target.CheckUnitOnTile().GetComponent<Units>();
                unit.TakeDamage(damage);
            }

            aoeTiles.Remove(target);
            
            foreach (var tile in aoeTiles)
            {
                if (!tile.CheckUnitOnTile()) continue;
  
                var unit = tile.CheckUnitOnTile().GetComponent<Units>();
                unit.TakeDamage(secondaryDamage);
                
                var direction = tile.transform.position - target.transform.position;
                
                unit.transform.position += direction;
                unit.UpdateLocation();
            }
            
            return true;
        }
    }
}
