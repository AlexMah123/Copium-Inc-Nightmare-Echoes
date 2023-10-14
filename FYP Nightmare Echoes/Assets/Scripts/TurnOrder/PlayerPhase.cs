using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NightmareEchoes.Unit;
using NightmareEchoes.Unit.Combat;
using NightmareEchoes.Unit.Pathfinding;
using NightmareEchoes.Inputs;
using NightmareEchoes.Grid;

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
                //enable this if you want to test applying tokens manually in the editor
                //controller.CurrentUnit.ApplyAllTokenEffects();

                if (controller.CurrentUnit.StunToken)
                {
                    tempStun = true;
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
            if(!tempStun)
            {
                PathfindingManager.Instance.StartPlayerPathfinding(controller.CurrentUnit);
                controller.StartCoroutine(WaitForTurnEnd());
            }
            
        }

        protected override void OnUpdate()
        {
            PathfindingManager.Instance.PlayerInputPathfinding();

            //if you cancel movement
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (PathfindingManager.Instance.CurrentPathfindingUnit != null && PathfindingManager.Instance.RevertUnitPosition != null)
                {
                    PathfindingManager.Instance.SetUnitPositionOnTile(PathfindingManager.Instance.RevertUnitPosition, controller.CurrentUnit);
                    PathfindingManager.Instance.CurrentPathfindingUnit.Direction = PathfindingManager.Instance.RevertUnitDirection;
                    controller.CurrentUnit.stats.Health = PathfindingManager.Instance.RevertUnitHealth;

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
                        CombatManager.Instance.ClearPreviews();
                    }

                    //shows back the tiles in range
                    PathfindingManager.Instance.StartPlayerPathfinding(controller.CurrentUnit);
                    CameraControl.Instance.UpdateCameraPan(controller.CurrentUnit.gameObject);

                }
                else
                {
                    controller.CurrentUnit.ShowPopUpText("Cannot Cancel Action!", Color.red);
                }
            }

            #region stealth token check
            if (controller.CurrentUnit.StealthToken)
            {
                var grid = CombatManager.Instance.SquareRange(controller.CurrentUnit.ActiveTile, 1);
                var cleanedGrid = OverlayTileManager.Instance.TrimOutOfBounds(grid);
                var enemiesInRange = new List<Entity>();

                foreach (var tile in cleanedGrid)
                {
                    if (!tile.CheckEntityGameObjectOnTile())
                        continue;

                    var target = tile.CheckEntityGameObjectOnTile().GetComponent<Entity>();

                    if (!target.IsHostile || target.IsProp)
                        continue;
                    enemiesInRange.Add(target);
                }

                //Check if this unit is in range of said enemies
                foreach (var enemy in enemiesInRange)
                {
                    grid = CombatManager.Instance.FrontalRange(enemy.ActiveTile, 1, enemy);
                    cleanedGrid = OverlayTileManager.Instance.TrimOutOfBounds(grid);

                    foreach (var tile in cleanedGrid)
                    {
                        if (controller.CurrentUnit.ActiveTile == tile)
                        {
                            enemy.ShowPopUpText("Detected Stealth Hero!!", Color.red);
                            controller.CurrentUnit.UpdateTokenLifeTime(STATUS_EFFECT.STEALTH_TOKEN);
                            PathfindingManager.Instance.RevertUnitPosition = null;
                        }
                    }
                }
            }
            #endregion

            if (PathfindingManager.Instance.isMoving)
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

                #region Tokens
                if (controller.CurrentUnit.ImmobilizeToken)
                {
                    controller.CurrentUnit.UpdateTokenLifeTime(STATUS_EFFECT.IMMOBILIZE_TOKEN);
                }
                #endregion

                #region BuffDebuff
                for (int i = controller.CurrentUnit.BuffDebuffList.Count - 1; i >= 0; i--)
                {
                    switch (controller.CurrentUnit.BuffDebuffList[i].statusEffect)
                    {
                        case STATUS_EFFECT.RESTORATION_BUFF:
                            controller.CurrentUnit.BuffDebuffList[i].TriggerEffect(controller.CurrentUnit);
                            break;
                    }
                }
                #endregion

                #endregion

                #region Mandatory Checks
                //Hide tiles only on exit
                if (PathfindingManager.Instance.playerTilesInRange.Count > 0)
                {
                    PathfindingManager.Instance.HideTilesInRange(PathfindingManager.Instance.playerTilesInRange);
                }

                //update effects & stats

                //should not need this but just checking
                //controller.CurrentUnit.ApplyAllBuffDebuffs();
                //controller.CurrentUnit.ApplyAllTokenEffects(); 
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
            PathfindingManager.Instance.RevertUnitPosition = null;
            controller.StartCoroutine(controller.PassTurn());
        }
    }
}
