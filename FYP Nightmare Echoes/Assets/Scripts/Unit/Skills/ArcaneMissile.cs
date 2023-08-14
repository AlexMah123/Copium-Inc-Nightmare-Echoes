using System.Collections;
using System.Collections.Generic;
using NightmareEchoes.Grid;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class ArcaneMissile : Skill
    {
        public override void Cast(Units target)
        {
            target.TakeDamage(damage);
            GetComponent<Units>().ShowPopUpText(skillName);
        }

        public override void Cast(OverlayTile target)
        {
            throw new System.NotImplementedException();
        }
    }
}
