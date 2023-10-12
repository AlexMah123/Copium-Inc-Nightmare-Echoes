using NightmareEchoes.Unit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
                    controller.turnOrderList[i].UpdateTokenLifeTime(STATUS_EFFECT.HASTE_TOKEN);
                }

                if (controller.turnOrderList[i].VertigoToken)
                {
                    //apply the end of cycle stats before ending the turn
                    controller.turnOrderList[i].UpdateStatsWithEndCycleEffect();
                    controller.turnOrderList[i].UpdateTokenLifeTime(STATUS_EFFECT.VERTIGO_TOKEN);
                }

            }

            #endregion

            UIManager.Instance.UpdateStatusEffectUI();
            controller.StartCoroutine(newTurn());
            
        }

        protected override void OnUpdate()
        {
            if (controller.FindAllEnemies() == null)
            {
                SceneManager.LoadScene(0);
            }
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
