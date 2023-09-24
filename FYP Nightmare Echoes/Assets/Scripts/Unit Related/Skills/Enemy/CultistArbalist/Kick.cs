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

            if (isBackstabbing)
            {
                target.TakeDamage(damage + backstabBonus);
            }
            else
            {
                target.TakeDamage(damage);
            }
            isBackstabbing = false;

            //knockback

            return true;
        }
    }
}
