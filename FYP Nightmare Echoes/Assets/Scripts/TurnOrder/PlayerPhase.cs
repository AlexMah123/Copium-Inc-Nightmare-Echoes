using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NightmareEchoes.Unit;
using NightmareEchoes.Unit.Combat;
using NightmareEchoes.Unit.Pathfinding;
using NightmareEchoes.Inputs;
using NightmareEchoes.Grid;

//created by Alex, edited by Ter
namespace NightmareEchoes.TurnOrder
{
    public class PlayerPhase : Phase
    {
        bool updateUIOnce = false;
        bool passTurnOnce = false;
        bool tempStun = false;
        private List<Skill> aoeSkillsPassed = new();

        protected override void OnEnter()
        {
            //Reseting Values
            tempStun = false;
            passTurnOnce = false;
            controller.CurrentUnit.HasMoved = false;
            controller.CurrentUnit.HasAttacked = false;

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

        protected override void OnFixedUpdate()
        {
            PathfindingManager.Instance.CheckMovement();

            if (PathfindingManager.Instance.isMoving)
            {
                UIManager.Instance.EnableCurrentUI(false);
                updateUIOnce = false;
            }
            else
            {
                if (!updateUIOnce)
                {
                    UIManager.Instance.EnableCurrentUI(true);
                    updateUIOnce = true;
                }
            }
        }

        protected override void OnUpdate()
        {
            if (controller.CurrentUnit != null)
            {
                var aoeDmg = CombatManager.Instance.CheckAoe(controller.CurrentUnit);
                if (aoeDmg)
                {
                    if (aoeSkillsPassed.Contains(aoeDmg))
                        return;
                    if (aoeDmg.Cast(controller.CurrentUnit))
                        aoeSkillsPassed.Add(aoeDmg);
                }

                PathfindingManager.Instance.PlayerInputPathfinding();

                CheckCancelAction();

                #region stealth token check
                CheckIfDetectedByEnemy();
                #endregion
            }
            else
            {
                if(!passTurnOnce)
                {
                    controller.StartCoroutine(controller.PassTurn());
                    passTurnOnce = true;
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

                if(controller.CurrentUnit.DeathMarkToken)
                {
                    controller.CurrentUnit.UpdateTokenLifeTime(STATUS_EFFECT.DEATHMARK_TOKEN);
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
            controller.CurrentUnit.HasAttacked = true;

            controller.StartCoroutine(controller.PassTurn());
        }

        void CheckCancelAction()
        {
            //if you cancel movement
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (PathfindingManager.Instance.CurrentPathfindingUnit != null && PathfindingManager.Instance.RevertUnitPosition != null)
                {
                    PathfindingManager.Instance.SetUnitPositionOnTile(controller.CurrentUnit, PathfindingManager.Instance.RevertUnitPosition);
                    PathfindingManager.Instance.CurrentPathfindingUnit.Direction = PathfindingManager.Instance.RevertUnitDirection;
                    controller.CurrentUnit.stats.Health = PathfindingManager.Instance.RevertUnitHealth;

                    //Resets everything, not moving, not dragging, and lastaddedtile is null
                    controller.CurrentUnit.HasMoved = false;
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
        }

        void CheckIfDetectedByEnemy()
        {
            if (controller.CurrentUnit.StealthToken)
            {
                var grid = CombatManager.Instance.SquareRange(controller.CurrentUnit.ActiveTile, 1);
                var cleanedGrid = OverlayTileManager.Instance.TrimOutOfBounds(grid);
                var enemiesInRange = new List<Entity>();

                for (int i = 0; i < cleanedGrid.Count; i++)
                {
                    if (!cleanedGrid[i].CheckEntityGameObjectOnTile())
                        continue;

                    var target = cleanedGrid[i].CheckEntityGameObjectOnTile().GetComponent<Entity>();

                    if (!target.IsHostile || target.IsProp)
                        continue;
                    enemiesInRange.Add(target);
                }

                //Check if this unit is in range of said enemies
                for (int i = 0; i < enemiesInRange.Count; i++)
                {
                    grid = CombatManager.Instance.FrontalRange(enemiesInRange[i].ActiveTile, 1, enemiesInRange[i]);
                    cleanedGrid = OverlayTileManager.Instance.TrimOutOfBounds(grid);

                    for (int j = 0; j < cleanedGrid.Count; j++)
                    {
                        if (controller.CurrentUnit.ActiveTile == cleanedGrid[j])
                        {
                            enemiesInRange[i].ShowPopUpText("Detected Stealth Hero!!", Color.red);
                            controller.CurrentUnit.UpdateTokenLifeTime(STATUS_EFFECT.STEALTH_TOKEN);
                            PathfindingManager.Instance.RevertUnitPosition = null;
                        }
                    }
                }
            }
        }
    }
}
