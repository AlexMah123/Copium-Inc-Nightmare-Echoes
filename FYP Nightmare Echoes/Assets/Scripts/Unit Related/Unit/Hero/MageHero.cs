using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NightmareEchoes.Unit.AI;
using NightmareEchoes.Unit.Combat;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class MageHero : Units
    {
        protected override void Awake()
        {
            base.Awake();  
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }

        #region Abilities
        public override void Move()
        {
            
        }

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

        public override void Passive()
        {
            
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);


        }

        [ContextMenu("Take Damage (2)")]
        public void TestDamage()
        {
            TakeDamage(2);
        }

        #endregion
    }
}
