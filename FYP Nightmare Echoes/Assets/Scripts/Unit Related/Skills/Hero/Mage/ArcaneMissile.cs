using System.Collections;
using System.Collections.Generic;
using NightmareEchoes.Grid;
using UnityEngine;

//Created by JH
namespace NightmareEchoes.Unit
{
    public class ArcaneMissile : Skill
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

            return true;
        }
    }
}
