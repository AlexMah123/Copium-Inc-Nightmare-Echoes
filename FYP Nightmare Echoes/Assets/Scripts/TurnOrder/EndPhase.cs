using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.TurnOrder
{
    public class EndPhase : Phase
    {
        protected override void OnEnter()
        {
            #region End of Cycle Effects

            for(int i = controller.turnOrderList.Count - 1; i >= 0; i--)
            {
                if (controller.turnOrderList[i].HasteToken)
                {
                    //apply the end of cycle stats before ending the turn
                    controller.turnOrderList[i].UpdateStatsWithEndCycleEffect();
                    controller.turnOrderList[i].HasteToken = false;
                }

                if (controller.turnOrderList[i].VertigoToken)
                {
                    //apply the end of cycle stats before ending the turn
                    controller.turnOrderList[i].UpdateStatsWithEndCycleEffect();
                    controller.turnOrderList[i].VertigoToken = false;
                }

            }

            UIManager.Instance.UpdateStatusEffectUI();

            #endregion

            controller.StartCoroutine(newTurn());
            
            //insert end of turn effects
        }

        protected override void OnUpdate()
        {

        }

        protected override void OnExit()
        {
            controller.cycleCount++;
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
