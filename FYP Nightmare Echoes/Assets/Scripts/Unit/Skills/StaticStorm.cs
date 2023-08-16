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
            throw new System.NotImplementedException();
        }

        public override bool Cast(OverlayTile target, List<OverlayTile> aoeTiles)
        {
            CombatManager.Instance.SetActiveAoe(this, aoeTiles);

            return true;
        }
    }
}
