using NightmareEchoes.Unit.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.Unit
{
    public class RangerHero : Entity
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

        #region Abilities()
        public override void Move()
        {

            throw new System.NotImplementedException();
        }

        public override void BasicAttack()
        {
            CombatManager.Instance.SelectSkill(this, basicAttack);
        }

        public override void Passive()
        {
            
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

        [ContextMenu("Take Damage (2)")]
        public void TestDamage()
        {
            TakeDamage(2);
        }

        

        #endregion

    }
}
