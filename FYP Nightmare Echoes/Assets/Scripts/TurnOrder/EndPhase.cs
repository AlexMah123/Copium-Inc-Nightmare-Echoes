using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.TurnOrder
{
    public class EndPhase : Phase
    {
        protected override void OnEnter()
        {
            controller.StartCoroutine(newTurn());
            
            //insert end of turn effects
        }

        protected override void OnUpdate()
        {

        }

        protected override void OnExit()
        {
            controller.turnCount++;
        }

        IEnumerator newTurn()
        {
            yield return new WaitForSeconds(controller.phaseDelay);

            //resets
            controller.runOnce = false;
            controller.ChangePhase(controller.startPhase);
        }
    }
}
