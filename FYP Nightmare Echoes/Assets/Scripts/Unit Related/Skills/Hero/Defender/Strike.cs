using NightmareEchoes.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class Strike : Skill
    {
        public override bool Cast(Entity target)
        {
            base.Cast(target);
            if (DealDamage(target))
            {
                Knockback(thisUnit.ActiveTile, target);
            }

            return true;
        }
    }
}
