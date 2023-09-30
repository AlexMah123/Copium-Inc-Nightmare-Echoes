using NightmareEchoes.Grid;
using NightmareEchoes.Unit.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class HunterInstinct : Skill
    {
        public override bool Cast(Units target)
        {
            base.Cast(target);

            DealDamage(target);

            return true;
        }

        public override bool Cast(OverlayTile target, List<OverlayTile> aoeTiles)
        {
            var copy = new List<OverlayTile>(aoeTiles);

            CombatManager.Instance.SetActiveAoe(this, copy);
            return true;
        }
    }
}
