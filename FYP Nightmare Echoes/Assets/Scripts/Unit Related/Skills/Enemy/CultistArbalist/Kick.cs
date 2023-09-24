using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    //Written by Ter (stolen from jh)
    public class Kick : Skill
    {
        public override bool Cast(Units target)
        {
            base.Cast(target);
            target.TakeDamage(damage);
            //knockback
            return true;
        }
    }
}
