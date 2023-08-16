using NightmareEchoes.Unit.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.Unit.Enemy
{
    [RequireComponent(typeof(BaseAI))]
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
            Direction = Direction.South;
            
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
            ShowPopUpText(damage.ToString());
            stats.Health -= damage;
        }

        [ContextMenu("Take Damage (2)")]
        public void TestDamage()
        {
            TakeDamage(2);
        }

        #endregion
    }
}
