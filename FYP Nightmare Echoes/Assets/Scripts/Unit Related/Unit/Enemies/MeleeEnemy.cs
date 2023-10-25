using NightmareEchoes.Unit.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.Unit.Enemy
{
    [RequireComponent(typeof(EnemyAI))]
    public class MeleeEnemy : Entity
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

        [ContextMenu("Take Damage (2)")]
        public void TestDamage()
        {
            TakeDamage(2);
        }

        #endregion
    }
}
