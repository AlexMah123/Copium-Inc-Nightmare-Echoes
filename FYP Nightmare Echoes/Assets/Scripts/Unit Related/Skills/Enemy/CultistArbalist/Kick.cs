using System.Collections;
using NightmareEchoes.Grid;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    //Written by Ter (stolen from jh)
    public class Kick : Skill
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
