using System.Collections;
using System.Collections.Generic;
using NightmareEchoes.Grid;
using NightmareEchoes.Sound;
using NightmareEchoes.Unit.Combat;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class StaticStorm : Skill
    {
        public override bool Cast(Entity target)
        {
            DealDamage(target);

            AudioManager.instance.PlaySFX("StaticStorm");
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
                if (Random.Range(0, 101) > 20)
                {
                    unit.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.WEAKEN_TOKEN, 1, 1));
                }
            }

            CombatManager.Instance.SetActiveAoe(this, copy);
            AudioManager.instance.PlaySFX("StaticStorm");
            return true;
        }
    }
}
