using NightmareEchoes.Unit.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.Unit.Enemy
{
    [RequireComponent(typeof(BasicEnemyAI))]
    public class MeleeEnemy : Units
    {
        public int basicAttackRange;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            /*var rand = Random.Range(0, 2);

            if(rand > 0)
            {
                AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.BARRIER_TOKEN, 1, 1));
            }
            else
            {
                AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.BLOCK_TOKEN, 1, 1));
            }*/
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
            
            
        }

        public override void Passive()
        {
            
        }

        public override void Skill1()
        {
            

        }

        public override void Skill2()
        {
            

        }

        public override void Skill3()
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
