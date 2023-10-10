using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NightmareEchoes.Unit;
using NightmareEchoes.Unit.Combat;
using NightmareEchoes.Unit.Pathfinding;
using NightmareEchoes.Inputs;

//created by Alex
namespace NightmareEchoes.TurnOrder
{
    public class PlayerPhase : Phase
    {
        bool runOnce = false;
        bool tempStun = false;
        protected override void OnEnter()
        {
            //Reseting Values
            tempStun = false;

            #region Insert Start of Turn Effects/Checks
            if (controller.CurrentUnit != null)
            {
                #region Tokens
                controller.CurrentUnit.ApplyAllTokenEffects();

                if (controller.CurrentUnit.StunToken)
                {
                    controller.CurrentUnit.UpdateTokenLifeTime(STATUS_EFFECT.STUN_TOKEN);

                    UIManager.Instance.EnableCurrentUI(false);
                    controller.StartCoroutine(controller.PassTurn());
                }
                #endregion

                #region BuffDebuff
                for (int i = controller.CurrentUnit.BuffDebuffList.Count - 1; i >= 0; i--)
                {
                    switch (controller.CurrentUnit.BuffDebuffList[i].statusEffect)
                    {
                        case STATUS_EFFECT.WOUND_DEBUFF:
                            controller.CurrentUnit.BuffDebuffList[i].TriggerEffect(controller.CurrentUnit);
                            break;
                    }
                }
                #endregion


                UIManager.Instance.UpdateStatusEffectUI();
                controller.CurrentUnit.UpdateStatusEffectEvent();
            }
            #endregion

            //Start Turn
            //COMMENT IF YOU WANT MANUAL MOVEMENT
            PathfindingManager.Instance.StartPlayerPathfinding(controller.CurrentUnit);
            controller.StartCoroutine(WaitForTurnEnd());
        }

        protected override void OnUpdate()
        {
            //COMMENT IF YOU WANT MANUAL MOVEMENT
            PathfindingManager.Instance.PlayerInputPathfinding();

            //if you cancel movement
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (PathfindingManager.Instance.CurrentPathfindingUnit != null && PathfindingManager.Instance.RevertUnitPosition != null)
                {
                    PathfindingManager.Instance.SetUnitPositionOnTile(PathfindingManager.Instance.RevertUnitPosition, controller.CurrentUnit);
                    PathfindingManager.Instance.CurrentPathfindingUnit.Direction = PathfindingManager.Instance.RevertUnitDirection;
                    controller.CurrentUnit.stats.Health = PathfindingManager.Instance.RevertUnitHealth;

                    //Add Section to have reverts if they hit a trap.

                    //Resets everything, not moving, not dragging, and lastaddedtile is null
                    PathfindingManager.Instance.isMoving = false;
                    PathfindingManager.Instance.hasMoved = false;
                    PathfindingManager.Instance.isDragging = false;
                    PathfindingManager.Instance.lastAddedTile = null;

                    PathfindingManager.Instance.ClearArrow(PathfindingManager.Instance.tempPathList);

                    //cancels the selected skill
                    if (CombatManager.Instance.ActiveSkill != null)
                    {
                        CombatManager.Instance.SelectSkill(controller.CurrentUnit, CombatManager.Instance.ActiveSkill);
                    }

                    //shows back the tiles in range
                    PathfindingManager.Instance.StartPlayerPathfinding(controller.CurrentUnit);
                    CameraControl.Instance.UpdateCameraPan(controller.CurrentUnit.gameObject);

                }
            }

            if(PathfindingManager.Instance.isMoving)
            {
                UIManager.Instance.EnableCurrentUI(false);
                runOnce = false;
            }
            else
            {
                if(!runOnce)
                {
                    UIManager.Instance.EnableCurrentUI(true);
                    runOnce = true;
                }
            }
        }

        protected override void OnExit()
        {
            if (controller.CurrentUnit != null)
            {
                #region End of Turn Effects
                if (controller.CurrentUnit.ImmobilizeToken)
                {
                    controller.CurrentUnit.UpdateTokenLifeTime(STATUS_EFFECT.IMMOBILIZE_TOKEN);
                }
                #endregion

                #region Mandatory Checks
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

                #endregion

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
    }
}
