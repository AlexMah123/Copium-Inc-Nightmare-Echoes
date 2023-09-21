using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NightmareEchoes.Unit;
using NightmareEchoes.Unit.Combat;
using NightmareEchoes.Unit.Pathfinding;

//created by Alex
namespace NightmareEchoes.TurnOrder
{
    public class PlayerPhase : Phase
    {
        protected override void OnEnter()
        {
            //Init here
            #region Insert Start of Turn Effects/Checks
            if (controller.CurrentUnit != null)
            {
                controller.CurrentUnit.ApplyAllTokenEffects();

                if (controller.CurrentUnit.StunToken == true)
                {
                    controller.CurrentUnit.StunToken = false;

                    UIManager.Instance.EnableCurrentUI(false);
                    UIManager.Instance.UpdateStatusEffectUI();

                    controller.StartCoroutine(controller.PassTurn());
                }
            }
            #endregion

            //Start Turn
            controller.StartCoroutine(WaitForTurnEnd());
        }

        protected override void OnUpdate()
        {

        }

        protected override void OnExit()
        {
            #region Apply End of Turn Effects/Checks
            
            #endregion

            CombatManager.Instance.turnEnded = false;

            if (controller.CurrentUnit != null)
            {
                //Hide tiles only on exit
                if(PathfindingManager.Instance.playerTilesInRange.Count > 0)
                {
                    PathfindingManager.Instance.HideTilesInRange(PathfindingManager.Instance.playerTilesInRange);
                }

                //update effects & stats
                controller.CurrentUnit.ApplyAllBuffDebuffs();
                controller.CurrentUnit.ApplyAllTokenEffects();
                controller.CurrentUnit.UpdateBuffDebuffLifeTime();
                controller.CurrentUnit.UpdateStatsWithoutEndCycleEffect();
            }
            
            UIManager.Instance.UpdateStatusEffectUI();

            //when you change phases, change the current unit to the next unit
            if (controller.CurrentUnitQueue.Count > 0)
            {
                controller.CurrentUnitQueue.Dequeue();
            }


        }
        
        IEnumerator WaitForTurnEnd()
        {
            yield return new WaitUntil(() => CombatManager.Instance.turnEnded);

            controller.StartCoroutine(controller.PassTurn());
        }

        #region Status Effect Checks

        #endregion

    }
}
