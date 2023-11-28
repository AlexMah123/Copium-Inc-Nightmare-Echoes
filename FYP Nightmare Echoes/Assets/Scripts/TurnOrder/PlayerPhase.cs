using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NightmareEchoes.Unit;
using NightmareEchoes.Unit.Combat;
using NightmareEchoes.Unit.Pathfinding;
using NightmareEchoes.Grid;
using System;

//created by Alex, edited by Ter
namespace NightmareEchoes.TurnOrder
{
    public class PlayerPhase : Phase
    {
        bool updateUIOnce = false;
        bool disableSkillOnce = false;
        bool passTurnOnce = false;
        bool tempStun = false;
        private List<Skill> aoeSkillsPassed = new();

        protected override void OnEnter()
        {
            if(TutorialUIManager.Instance != null)
            {
                if(TutorialUIManager.Instance.InTutorialState())
                {
                    if (TutorialUIManager.Instance.currentPanelIndex <= TutorialUIManager.Instance.currentTutorialGuideCap)
                    {
                        TutorialUIManager.Instance.EnableTutorialCanvas();
                    }
                }
            }

            GameUIManager.Instance.phaseText.text = $"Player's Turn";
            GameUIManager.Instance.phaseText.color = Color.white;

            //Reseting Values
            updateUIOnce = false;
            disableSkillOnce = false;
            passTurnOnce = false;
            tempStun = false;
            controller.CurrentUnit.HasMoved = false;
            controller.CurrentUnit.HasAttacked = false;
            controller.CurrentUnit.HighlightUnit();

            #region Insert Start of Turn Effects/Checks
            if (controller.CurrentUnit != null)
            {
                /*foreach (STATUS_EFFECT statusEffect in Enum.GetValues(typeof(STATUS_EFFECT)))
                {
                    if (statusEffect == STATUS_EFFECT.NONE)
                        continue;

                    controller.CurrentUnit.AddBuff(GetStatusEffect.CreateModifier(statusEffect, 1, 1));
                }*/

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

                #region Tokens
                //enable this if you want to test applying tokens manually in the editor
                //controller.CurrentUnit.ApplyAllTokenEffects();

                if(controller.CurrentUnit != null)
                {
                    if (controller.CurrentUnit.StunToken)
                    {
                        tempStun = true;
                        controller.CurrentUnit.UpdateTokenLifeTime(STATUS_EFFECT.STUN_TOKEN);

                        controller.StartCoroutine(controller.PassTurn());
                    }
                }
                #endregion

                GameUIManager.Instance.UpdateStatusEffectUI();
            }
            #endregion

            //Start Turn
            if (!tempStun && controller.CurrentUnit != null)
            {
                controller.CurrentUnit.UpdateStatusEffectEvent();

                PathfindingManager.Instance.StartPlayerPathfinding(controller.CurrentUnit);
                controller.StartCoroutine(WaitForTurnEnd());
            }
            else if(controller.CurrentUnit == null)
            {
                controller.StartCoroutine(controller.PassTurn());
                passTurnOnce = true;
            }

        }

        protected override void OnFixedUpdate()
        {
            PathfindingManager.Instance.CheckMovement();

            if (PathfindingManager.Instance.isMoving)
            {
                GameUIManager.Instance.EnableCurrentUI(false);
                updateUIOnce = false;
            }
            else
            {
                if (!updateUIOnce)
                {
                    GameUIManager.Instance.EnableCurrentUI(true);
                    updateUIOnce = true;
                }
            }
        }

        protected override void OnUpdate()
        {
            if(CombatManager.Instance.skillIsCasting && !disableSkillOnce)
            {
                GameUIManager.Instance.EnableCurrentUI(false);
                disableSkillOnce = true;
            }

            // unit dependant updates
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

                //check inputs
                CheckHotkeyInputs();

                // stealth token check
                CheckIfDetectedByEnemy();
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
                controller.CurrentUnit.UnhighlightUnit();
                controller.CurrentUnit.ResetAnimator();

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
                controller.CurrentUnit.UpdateAllBuffDebuffLifeTime();
                controller.CurrentUnit.UpdateStatsWithoutEndCycleEffect();

                #endregion

                #region Apply Certain End of Turn Effects/Checks Without Updating Lifetime
                if (tempStun)
                {
                    tempStun = false;

                    controller.CurrentUnit.AddBuff(GetStatusEffect.CreateModifier(STATUS_EFFECT.STUN_RESISTANCE_BUFF, 50, 1));
                    controller.CurrentUnit.UpdateStatsWithoutEndCycleEffect();
                }

                #endregion
            }

            GameUIManager.Instance.UpdateStatusEffectUI();

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

        void CheckHotkeyInputs()
        {
            if (CombatManager.Instance.lockInput) return;
            
            if (Input.GetKeyDown(KeyCode.Z) && !PathfindingManager.Instance.isMoving)
            {
                controller.SkipTurn();
            }

            //right click to cancel
            if (Input.GetMouseButtonDown(1))
            {
                GameUIManager.Instance.CancelActionButton();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1) && !PathfindingManager.Instance.isMoving)
            {
                GameUIManager.Instance.AttackButton();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2) && !PathfindingManager.Instance.isMoving)
            {
                GameUIManager.Instance.Skill1Button();
            }

            if (Input.GetKeyDown(KeyCode.Alpha3) && !PathfindingManager.Instance.isMoving)
            {
                GameUIManager.Instance.Skill2Button();
            }

            if (Input.GetKeyDown(KeyCode.Alpha4) && !PathfindingManager.Instance.isMoving)
            {
                GameUIManager.Instance.Skill3Button();
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
