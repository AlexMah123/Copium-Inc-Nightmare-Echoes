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

            AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.SPEED_BUFF, 10, 2));
            //AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.HASTE_TOKEN, 30, 1));
            //AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.STUN_TOKEN, 30, 1));
            AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.WOUND_DEBUFF, 1, 2));

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

        #endregion
    }
}
