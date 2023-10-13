using System.Collections.Generic;
using NightmareEchoes.Grid;
using NightmareEchoes.Sound;
using UnityEngine;

//Created by JH
namespace NightmareEchoes.Unit
{
    public class ArcaneBlast : Skill
    {
        public override bool Cast(OverlayTile target, List<OverlayTile> aoeTiles)
        {
            base.Cast(target, aoeTiles);

            if (target.CheckEntityGameObjectOnTile())
            {
                var unit = target.CheckEntityGameObjectOnTile().GetComponent<Entity>();
                DealDamage(unit, default, checkBlind: false);
            }

            aoeTiles.Remove(target);
            
            foreach (var tile in aoeTiles)
            {
                if (!tile.CheckEntityGameObjectOnTile()) continue;
  
                var unit = tile.CheckEntityGameObjectOnTile().GetComponent<Entity>();

                DealDamage(unit, secondaryDamage, checkBlind:false);
                Knockback(target, unit);
            }
            AudioManager.instance.PlaySFX("ArcaneBlast");
            return true;
        }
    }
}
