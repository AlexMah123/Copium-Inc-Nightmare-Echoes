using System.Collections;
using System.Collections.Generic;
using NightmareEchoes.Grid;
using NightmareEchoes.Unit.Combat;
using UnityEngine;

//Created by Vinn
namespace NightmareEchoes.Unit
{
    public class BountyHunterSlash : Skill
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
