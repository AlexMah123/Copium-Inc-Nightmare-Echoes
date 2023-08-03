using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.Unit
{
    public class BountyHunter : BaseUnit
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
            Direction = Direction.West;
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
            ShowDamage(damage.ToString());
            Health -= damage;

        }

        [ContextMenu("Take Damage (2)")]
        public void TestDamage()
        {
            TakeDamage(2);
        }

        #endregion
    }
}