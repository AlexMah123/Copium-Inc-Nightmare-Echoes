using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    //Written by Ter (stolen from jh)
    public class ArrowShot : Skill
    {
        public override bool Cast(Units target)
        {
            target.TakeDamage(damage);
            return true;
        }
    }
}
