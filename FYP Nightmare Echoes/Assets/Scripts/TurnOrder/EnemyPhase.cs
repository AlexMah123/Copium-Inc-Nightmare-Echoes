using System.Collections;
using System.Collections.Generic;
using NightmareEchoes.Unit;
using UnityEngine;
using NightmareEchoes.Unit.AI;
using NightmareEchoes.Unit.Combat;
using NightmareEchoes.Unit.Pathfinding;
using NightmareEchoes.Inputs;

//created by Alex
namespace NightmareEchoes.TurnOrder
{
    public class EnemyPhase : Phase
    {
        bool runOnce = false;
        bool tempStun = false;

        EnemyAI enemyAI;

        private List<Skill> aoeSkillsPassed = new();

        protected override void OnEnter()
        {
            //Reseting Values
            tempStun = false;
            runOnce = false;
            controller.CurrentUnit.HasMoved = false;
            controller.CurrentUnit.HasAttacked = false;
            controller.CurrentUnit.HighlightUnit();


            #region Insert Start of Turn Effects/Checks
            if (controller.CurrentUnit != null)
            {
                //cache enemyAI
                enemyAI = controller.CurrentUnit.GetComponent<EnemyAI>();

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
            if (!tempStun)
            {
                controller.StartCoroutine(EnemyTurn());
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
            if(controller.CurrentUnit != null)
            {
                controller.CurrentUnit.UnhighlightUnit();

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

        IEnumerator EnemyTurn()
        {
            controller.CurrentUnit.ShowPopUpText(". . .", Color.magenta, duration: controller.enemythinkingDelay, 8);
            yield return new WaitForSeconds(Random.Range(controller.enemythinkingDelay, controller.enemythinkingDelay + 2));

            if (controller.CurrentUnit != null && enemyAI != null)
            {
                enemyAI.Execute();
            }
            else
            {
                controller.StartCoroutine(controller.PassTurn());
            }

            yield return new WaitUntil(() => enemyAI.finalMovePath.Count == 0);

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
            else if (!enemyAI.detectedStealthHero) //if you are just moving normally
            {
                controller.StartCoroutine(controller.PassTurn());
            }
            else //default catch if nothing is happening
            {
                controller.StartCoroutine(controller.PassTurn());
            }

            yield return new WaitUntil(() => CombatManager.Instance.turnEnded);

            controller.StartCoroutine(controller.PassTurn());
        }
    }
}
