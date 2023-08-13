using System.Collections;
using System.Collections.Generic;
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
    }
}
