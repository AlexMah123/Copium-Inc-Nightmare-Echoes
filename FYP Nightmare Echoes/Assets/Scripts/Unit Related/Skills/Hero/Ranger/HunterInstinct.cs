using NightmareEchoes.Grid;
using NightmareEchoes.Unit.Combat;
using System.Collections;
using System.Collections.Generic;
using NightmareEchoes.Unit.AI;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class HunterInstinct : Skill
    {
        public override bool Cast(Entity target)
        {
            if (target.GetComponent<BasicEnemyAI>().hasMoved)
            {
                DealDamage(target);
                CombatManager.Instance.ClearActiveAoe(this);
                return true;
            }
            
            if (target.GetComponent<BasicEnemyAI>().hasAttacked)
            {
                DealDamage(target, secondaryDamage);
                CombatManager.Instance.ClearActiveAoe(this);
                return true;
            }

            return false;
        }

        public override bool Cast(OverlayTile target, List<OverlayTile> aoeTiles)
        {
            var copy = new List<OverlayTile>(aoeTiles);
            CombatManager.Instance.SetActiveAoe(this, copy);
            base.Cast(target, aoeTiles);
            return true;
        }
    }
}
