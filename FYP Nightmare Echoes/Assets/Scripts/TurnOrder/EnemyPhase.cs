using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.TurnOrder
{
    public class EnemyPhase : Phase
    {
        protected override void OnEnter()
        {
            UIManager.Instance.EnablePlayerUI(false);
            controller.StartCoroutine(EnemyTurn());
        }

        protected override void OnUpdate()
        {
            
        }

        protected override void OnExit()
        {
            if (controller.currentUnitIterator < controller.unitArray.Length)
            {
                controller.CurrentUnit = controller.unitArray[controller.currentUnitIterator];
            }
        }

        IEnumerator EnemyTurn()
        {
            yield return new WaitForSeconds(controller.delay);
            controller.ChangePhase(controller.playerPhase);

        }
    }
}
