using System.Collections;
using System.Collections.Generic;
using NightmareEchoes.Grid;
using NightmareEchoes.Unit.Combat;
using UnityEngine;

//Created by JH
namespace NightmareEchoes.Unit
{
    public class Teleport : Skill
    {
        public override bool Cast(Units target)
        {
            CombatManager.Instance.SecondaryTargeting();
            return true;
        }

        public override bool SecondaryCast(Units target)
        {
            throw new System.NotImplementedException();
        }

        public override bool SecondaryCast(OverlayTile target, List<OverlayTile> aoeTiles)
        {
            throw new System.NotImplementedException();
        }
        
        public virtual IEnumerator SecondaryStep()
        {
            throw new System.NotImplementedException();
        }

        private IEnumerator Test()
        {
            yield return null;
        }
    }
}
