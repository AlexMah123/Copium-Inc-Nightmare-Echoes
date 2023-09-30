using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace NightmareEchoes.Unit
{
    public class DestroyableProp : Units
    {
        protected override void Awake()
        {
            isProp = true;
            isHostile = true;
            gameObject.layer = LayerMask.NameToLayer("Unit");
            base.Awake();
            StartCoroutine(UpdateTileLocation());
        }

        void Update()
        {
            base.Update();
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
        }

        IEnumerator UpdateTileLocation()
        {
            yield return new WaitForSeconds(1f);
            UpdateLocation();
        }
    }
}
