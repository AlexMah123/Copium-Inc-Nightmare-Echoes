using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.TurnOrder
{
    public class PlanPhase : Phase
    {
        protected override void OnEnter()
        {
            //calculate once
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

            yield return new WaitForSeconds(controller.phaseDelay);
            controller.ChangePhase(controller.startPhase);

        }
    }
}
