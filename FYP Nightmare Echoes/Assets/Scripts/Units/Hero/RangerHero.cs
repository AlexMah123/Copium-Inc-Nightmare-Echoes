using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class RangerHero : BaseUnit
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
        public override void BasicAttack()
        {
            direction = Direction.West;
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

        #endregion

    }
}
