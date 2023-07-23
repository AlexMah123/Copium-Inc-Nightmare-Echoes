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
            if (controller.currentUnitIterator < controller.TurnOrderList.Count)
            {
                controller.CurrentUnit = controller.TurnOrderList[controller.currentUnitIterator];
            }
        }

        IEnumerator EnemyTurn()
        {
            yield return new WaitForSeconds(controller.delay);
            controller.ChangePhase(controller.playerPhase);

        }
    }
}
