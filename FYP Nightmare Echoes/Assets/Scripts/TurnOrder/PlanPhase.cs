using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.TurnOrder
{
    public class PlanPhase : Phase
    {
        protected override void OnEnter()
        {
            controller.StartCoroutine(Planning());
        }

        protected override void OnUpdate()
        {

        }

        protected override void OnExit()
        {
            
        }

        IEnumerator Planning()
        {

            yield return new WaitForSeconds(controller.delay);
            controller.ChangePhase(controller.startPhase);

        }
    }
}
