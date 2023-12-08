using System.Collections;
using System.Collections.Generic;
using NightmareEchoes.Unit;
using UnityEngine;
using NightmareEchoes.Unit.AI;
using NightmareEchoes.Unit.Combat;
using NightmareEchoes.Unit.Pathfinding;
using System.Linq;


//created by Alex
namespace NightmareEchoes.TurnOrder
{
    public class EnemyPhase : Phase
    {
        bool runOnce = false;
        bool tempStun = false;
        bool passTurnOnce = false;

        EnemyAI enemyAI;

        private List<Skill> aoeSkillsPassed = new();

        protected override void OnEnter()
        {
            GameUIManager.Instance.phaseText.text = $"Enemy's Turn";
            GameUIManager.Instance.phaseText.color = Color.white;

            //Reseting Values
            tempStun = false;
            runOnce = false;
            passTurnOnce = false;
            controller.CurrentUnit.HasMoved = false;
            controller.CurrentUnit.HasAttacked = false;
            controller.CurrentUnit.HighlightUnit();


            #region Insert Start of Turn Effects/Checks
            if (controller.CurrentUnit != null)
            {
                //cache enemyAI
                enemyAI = controller.CurrentUnit.GetComponent<EnemyAI>();

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

                if (controller.CurrentUnit.StunToken)
                {
                    tempStun = true;
                    controller.CurrentUnit.UpdateTokenLifeTime(STATUS_EFFECT.STUN_TOKEN);

                    controller.StartCoroutine(controller.PassTurn());
                }

                #endregion

                GameUIManager.Instance.UpdateStatusEffectUI();
            }
            #endregion

            //Start Turn
            if (!tempStun && controller.CurrentUnit != null)
            {
                controller.CurrentUnit.UpdateStatusEffectEvent();
                controller.StartCoroutine(EnemyTurn());
            }
            else if (controller.CurrentUnit == null)
            {
                controller.StartCoroutine(controller.PassTurn());
                passTurnOnce = true;
            }

            aoeSkillsPassed.Clear();
        }

        protected override void OnFixedUpdate()
        {
            //start a couroutine to move
            if (enemyAI == null || controller.CurrentUnit == null) return;

            if (enemyAI.finalMovePath.Count > 0)
            {
                enemyAI.MoveProcess(controller.CurrentUnit);
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

                var trapDmg = CombatManager.Instance.CheckTrap(controller.CurrentUnit);
                if (trapDmg)
                {
                    trapDmg.Cast(controller.CurrentUnit);
                }
            }
        }

        protected override void OnExit()
        {
            if (controller.CurrentUnit != null && enemyAI != null)
            {
                controller.CurrentUnit.UnhighlightUnit();
                controller.CurrentUnit.ResetAnimator();

                #region End of Turn Effects

                #region Tokens
                if (controller.CurrentUnit.ImmobilizeToken)
                {
                    controller.CurrentUnit.UpdateTokenLifeTime(STATUS_EFFECT.IMMOBILIZE_TOKEN);
                }

                if (controller.CurrentUnit.DeathMarkToken)
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
                if (enemyAI.walkableThisTurnTiles?.Count > 0)
                {
                    PathfindingManager.Instance.HideTilesInRange(enemyAI.walkableThisTurnTiles);
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

        IEnumerator EnemyTurn()
        {
            yield return new WaitForSeconds(1f);

            if (controller.CurrentUnit != null && enemyAI != null)
            {
                controller.CurrentUnit.ShowPopUpText(". . .", Color.yellow, duration: controller.enemythinkingDelay, 40);
                yield return new WaitForSeconds(controller.enemythinkingDelay);

                enemyAI.Execute();
                if(enemyAI.finalMovePath.Count > 0)
                {
                    if(enemyAI.finalMovePath.Last().GetComponent<Entity>())
                    {
                        enemyAI.finalMovePath.RemoveAt(enemyAI.finalMovePath.Count - 1);
                    }
                }

                yield return new WaitUntil(() => enemyAI.finalMovePath.Count == 0);
            }

            //if you have reached the end, and are suppose to attack, havent attacked, havent foundStealthHero and there is a target.
            if ((enemyAI.attack || enemyAI.moveAndAttack) && !enemyAI.detectedStealthHero)
            {
                //if you are not immobilized, just attack
                if (!controller.CurrentUnit.ImmobilizeToken)
                {
                    enemyAI.AttackProcess(controller.CurrentUnit, enemyAI.targetTileToAttack);
                }
                //if you are immobilized, but your range is within your selected attack range, attack
                else if (controller.CurrentUnit.ImmobilizeToken && enemyAI.FindDistanceBetweenUnit(controller.CurrentUnit, enemyAI.targetHero) <= enemyAI.currSelectedSkill.Range)
                {
                    enemyAI.AttackProcess(controller.CurrentUnit, enemyAI.targetTileToAttack);
                }
                else
                {
                    controller.StartCoroutine(controller.PassTurn());
                }
            }
            else if (enemyAI.detectedStealthHero)
            {
                enemyAI.AttackProcess(controller.CurrentUnit, enemyAI.targetTileToAttack);
            }
            else if(enemyAI != null)//default catch if nothing is happening
            {
                controller.StartCoroutine(controller.PassTurn());
            }

            yield return new WaitUntil(() => CombatManager.Instance.turnEnded);

            controller.StartCoroutine(controller.PassTurn());
        }
    }
}
