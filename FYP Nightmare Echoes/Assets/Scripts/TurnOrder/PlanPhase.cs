using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.TurnOrder
{
    public class PlanPhase : Phase
    {
        protected override void OnEnter()
        {
            controller.StartCoroutine(Planning());
            //insert planning faze, placing heros
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
