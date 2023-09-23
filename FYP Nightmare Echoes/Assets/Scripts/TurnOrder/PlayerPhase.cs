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
        bool tempStun = false;

        protected override void OnEnter()
        {
            tempStun = false;
            //Init here
            #region Insert Start of Turn Effects/Checks
            if (controller.CurrentUnit != null)
            {
                //TESTING
                //controller.CurrentUnit.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.HASTE_TOKEN, 99, 1));

                controller.CurrentUnit.ApplyAllTokenEffects();

                if (controller.CurrentUnit.StunToken)
                {
                    controller.CurrentUnit.StunToken = false;

                    UIManager.Instance.EnableCurrentUI(false);
                    UIManager.Instance.UpdateStatusEffectUI();

                    controller.StartCoroutine(controller.PassTurn());
                }



                controller.CurrentUnit.UpdateStatusEffectEvent();
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
            CombatManager.Instance.turnEnded = false;

            if (controller.CurrentUnit != null)
            {
                #region End of Turn Effects
                if (controller.CurrentUnit.ImmobilizeToken)
                {
                    controller.CurrentUnit.ImmobilizeToken = false;
                }
                #endregion

                //Hide tiles only on exit
                if (PathfindingManager.Instance.playerTilesInRange.Count > 0)
                {
                    PathfindingManager.Instance.HideTilesInRange(PathfindingManager.Instance.playerTilesInRange);
                }

                //update effects & stats
                controller.CurrentUnit.ApplyAllBuffDebuffs();
                controller.CurrentUnit.ApplyAllTokenEffects();
                controller.CurrentUnit.UpdateBuffDebuffLifeTime();
                controller.CurrentUnit.UpdateStatsWithoutEndCycleEffect();

                #region Apply Certain End of Turn Effects/Checks Without Updating Lifetime
                if (tempStun)
                {
                    tempStun = false;

                    controller.CurrentUnit.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.STUN_RESISTANCE_BUFF, 50, 1));
                    controller.CurrentUnit.UpdateStatsWithoutEndCycleEffect();
                }

                
                #endregion
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
