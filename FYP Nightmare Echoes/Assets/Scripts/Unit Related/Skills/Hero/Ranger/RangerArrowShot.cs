using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class RangerArrowShot : Skill
    {
       public override bool Cast(Entity target)
        {
            base.Cast(target);

            DealDamage(target);

            return true;
        }
    }
}
