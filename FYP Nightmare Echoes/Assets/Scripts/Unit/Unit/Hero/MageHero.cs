using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NightmareEchoes.Unit.Combat;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class MageHero : Units
    {
        public override void BasicAttack()
        {
            CombatManager.Instance.SelectSkill(this, basicAttack);
        }
    }
}
