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

        public override void Skill1()
        {
            CombatManager.Instance.SelectSkill(this, skill1);
        }

        public override void Skill2()
        {
            CombatManager.Instance.SelectSkill(this, skill2);
        }
        
        public override void Skill3()
        {
            CombatManager.Instance.SelectSkill(this, skill3);
        }
    }
}
