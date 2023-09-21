using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class RangerArrowShot : Skill
    {
       public override bool Cast(Units target)
        {
            target.TakeDamage(damage);
            return true;
        }
    }
}