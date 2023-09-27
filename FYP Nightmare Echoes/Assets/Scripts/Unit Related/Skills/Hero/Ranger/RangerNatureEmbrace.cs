using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class RangerNatureEmbrace : Skill
    {
        public override bool Cast(Units target)
        {
            base.Cast(target);

            if (target.tag = "")

            target.stats.Health += heal;

            return true;
        }

    }
}
