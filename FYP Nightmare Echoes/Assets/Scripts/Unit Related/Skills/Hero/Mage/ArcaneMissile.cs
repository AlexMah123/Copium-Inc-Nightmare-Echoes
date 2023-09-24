using System.Collections;
using System.Collections.Generic;
using NightmareEchoes.Grid;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

//Created by JH
namespace NightmareEchoes.Unit
{
    public class ArcaneMissile : Skill
    {
        public override bool Cast(Units target)
        {
            base.Cast(target);

            DealDamage(target);

            return true;
        }
    }
}
