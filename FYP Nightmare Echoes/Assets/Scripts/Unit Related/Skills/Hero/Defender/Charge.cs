using NightmareEchoes.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class Charge : Skill
    {
        public override bool Cast(Entity target)
        {
            base.Cast(target);
            DealDamage(target);

            Knockback(thisUnit.ActiveTile, target);

            return true;
        }
    }
}
