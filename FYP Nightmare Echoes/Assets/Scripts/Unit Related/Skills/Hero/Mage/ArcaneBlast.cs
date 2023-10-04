using System.Collections.Generic;
using NightmareEchoes.Grid;
using UnityEngine;

//Created by JH
namespace NightmareEchoes.Unit
{
    public class ArcaneBlast : Skill
    {
        public override bool Cast(OverlayTile target, List<OverlayTile> aoeTiles)
        {
            base.Cast(target, aoeTiles);

            if (target.CheckUnitOnTile())
            {
                var unit = target.CheckUnitOnTile().GetComponent<Entity>();
                DealDamage(unit);
            }

            aoeTiles.Remove(target);
            
            foreach (var tile in aoeTiles)
            {
                if (!tile.CheckUnitOnTile()) continue;
  
                var unit = tile.CheckUnitOnTile().GetComponent<Entity>();

                //manually check for token
                if (thisUnit.WeakenToken)
                {
                    int newDamage = Mathf.RoundToInt(secondaryDamage * 0.5f);
                    unit.TakeDamage(newDamage);

                    thisUnit.ShowPopUpText($"Attack was weakened!", Color.red);
                    thisUnit.UpdateTokenLifeTime(STATUS_EFFECT.WEAKEN_TOKEN);
                }
                else if (thisUnit.StrengthToken)
                {
                    int newDamage = Mathf.RoundToInt(secondaryDamage * 1.5f);
                    unit.TakeDamage(newDamage);

                    thisUnit.ShowPopUpText($"Attack was strengthen!", Color.red);
                    thisUnit.UpdateTokenLifeTime(STATUS_EFFECT.STRENGTH_TOKEN);
                }
                else
                {
                    unit.TakeDamage(secondaryDamage);
                }
                
                Knockback(target, unit);
            }
            
            return true;
        }
    }
}
