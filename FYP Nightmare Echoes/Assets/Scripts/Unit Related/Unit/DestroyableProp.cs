using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace NightmareEchoes.Unit
{
    public class DestroyableProp : Entity
    {
        protected override void Awake()
        {
            base.Awake();
            StartCoroutine(UpdateTileLocation());
        }

        protected override void Update()
        {
            base.Update();
        }

        IEnumerator UpdateTileLocation()
        {
            yield return new WaitForSeconds(1f);
            UpdateLocation();
        }
    }
}
