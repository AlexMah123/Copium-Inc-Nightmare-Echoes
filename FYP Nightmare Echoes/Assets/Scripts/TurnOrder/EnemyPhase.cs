using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.TurnOrder
{
    public class EnemyPhase : Phase
    {
        protected override void OnEnter()
        {
            controller.StartCoroutine(EnemyTurn());
        }

        protected override void OnUpdate()
        {
            if (controller.UnitQueue.Count <= 0)
            {
                controller.ChangePhase(controller.endPhase);
            }
        }

        protected override void OnExit()
        {
            
        }

        IEnumerator EnemyTurn()
        {
            yield return new WaitForSeconds(controller.delay);
            controller.ChangePhase(controller.playerPhase);

        }
    }
}
