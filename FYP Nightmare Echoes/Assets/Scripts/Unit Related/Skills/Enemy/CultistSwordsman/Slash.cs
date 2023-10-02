using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    //Written by Ter (stolen from jh)
    public class Slash : Skill
    {
        public override bool Cast(Entity target)
        {
            base.Cast(target);

            DealDamage(target);

            return true;
        }
    }
}
