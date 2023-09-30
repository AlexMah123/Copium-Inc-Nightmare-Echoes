using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class DestroyableProp : Units
    {
        protected override void Awake()
        {
            isProp = true;
            isHostile = true;
            gameObject.layer = LayerMask.NameToLayer("Obstacles");
        }
        
        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
        }
    }
}
