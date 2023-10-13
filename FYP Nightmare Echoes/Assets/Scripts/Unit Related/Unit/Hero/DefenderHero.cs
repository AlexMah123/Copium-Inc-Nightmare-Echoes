using NightmareEchoes.Unit.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class DefenderHero : Entity
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

        public override void TakeDamage(int damage, bool checkDodge = true)
        {
            base.TakeDamage(damage, checkDodge);

            if (FindModifier(STATUS_EFFECT.WOUND_DEBUFF))
            {
                buffDebuffList.Remove(FindModifier(STATUS_EFFECT.WOUND_DEBUFF));
            }
        }

        [ContextMenu("Take Damage (2)")]
        public void TestDamage()
        {
            TakeDamage(2);
        }

        #endregion

    }
}
