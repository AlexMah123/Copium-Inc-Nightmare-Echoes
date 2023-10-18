using System.Collections;
using System.Collections.Generic;
using NightmareEchoes.Grid;
using NightmareEchoes.Sound;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

//Created by JH
namespace NightmareEchoes.Unit
{
    public class ArcaneMissile : Skill
    {
        public override bool Cast(Entity target)
        {
            base.Cast(target);

            DealDamage(target);

            //AudioManager.instance.PlaySFX("ArcaneMissle");
            return true;
        }
    }
}
