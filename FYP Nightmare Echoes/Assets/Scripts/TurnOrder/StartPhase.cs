using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.TurnOrder
{
    public class StartPhase : Phase
    {
        protected override void OnEnter()
        {
            controller.StartCoroutine(Start());
        }

        protected override void OnUpdate()
        {
            
        }

        protected override void OnExit()
        {
            
        }

        IEnumerator Start()
        {
            yield return new WaitForSeconds(controller.delay);

            if (!controller.CurrentUnit.IsHostile)
            {
                controller.ChangePhase(controller.playerPhase);
            }
            else
            {
                controller.ChangePhase(controller.enemyPhase);
            }
        }
    }
}
