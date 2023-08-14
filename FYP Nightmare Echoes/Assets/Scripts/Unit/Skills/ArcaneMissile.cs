using System.Collections;
using System.Collections.Generic;
using NightmareEchoes.Grid;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class ArcaneMissile : Skill
    {
        public override bool Cast(Units target)
        {
            target.TakeDamage(damage);
            return true;
        }

        public override bool Cast(OverlayTile target)
        {
            throw new System.NotImplementedException();
        }
    }
}
