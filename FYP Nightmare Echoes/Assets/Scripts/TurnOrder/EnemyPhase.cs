using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using NightmareEchoes.Unit;
using UnityEngine;
using NightmareEchoes.Unit.AI;
using NightmareEchoes.Unit.Combat;
using NightmareEchoes.Unit.Pathfinding;
using Unity.VisualScripting.Antlr3.Runtime.Collections;

//created by Alex
namespace NightmareEchoes.TurnOrder
{
    public class EnemyPhase : Phase
    {
        bool tempStun = false;
        BasicEnemyAI enemyAI;

        private List<Skill> aoeSkillsPassed = new();

        protected override void OnEnter()
        {
            //Init here
            if (controller.CurrentUnit != null)
                enemyAI = controller.CurrentUnit.GetComponent<BasicEnemyAI>();

            #region Insert Start of Turn Effects/Checks
            //start of turn effects
            if (controller.CurrentUnit != null)
            {
                //checks and applies all the effects (sets things to true if they exist)
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
            controller.StartCoroutine(EnemyTurn());
            aoeSkillsPassed.Clear();
        }

        protected override void OnUpdate()
        {
            //start a couroutine to move
            if (enemyAI == null || controller.CurrentUnit == null) return;

            if (enemyAI.TotalHeroList.Count > 0)
            {
                enemyAI.MoveProcess(controller.CurrentUnit);
            }

            var aoeDmg = CombatManager.Instance.CheckAoe(controller.CurrentUnit);
            if (aoeDmg)
            {
                if (aoeSkillsPassed.Contains(aoeDmg)) return;
                aoeDmg.Cast(controller.CurrentUnit);
                aoeSkillsPassed.Add(aoeDmg);
            }
            
            var trapDmg = CombatManager.Instance.CheckTrap(controller.CurrentUnit);
            if (trapDmg)
            {
                trapDmg.Cast(controller.CurrentUnit);
            }
        }

        protected override void OnExit()
        {
            CombatManager.Instance.turnEnded = false;

            if (controller.CurrentUnit != null && enemyAI != null)
            {
                //Hide tiles only on exit
                if(enemyAI.TilesInRange.Count > 0)
                {
                    PathfindingManager.Instance.HideTilesInRange(enemyAI.TilesInRange);
                }

                //update effects & stats
                controller.CurrentUnit.ApplyAllBuffDebuffs();
                controller.CurrentUnit.ApplyAllTokenEffects();
                controller.CurrentUnit.UpdateBuffDebuffLifeTime();
                controller.CurrentUnit.UpdateStatsWithoutEndCycleEffect();

                #region Apply End of Turn Effects/Checks
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
            
            yield return new WaitForSeconds(controller.enemythinkingDelay);
            
            if(controller.CurrentUnit != null)
            {
                enemyAI.MakeDecision(controller.CurrentUnit);
            }

            if(!enemyAI.inAtkRange && !enemyAI.inMoveAndAttackRange)
            {
                yield return new WaitUntil(() => enemyAI.totalPathList.Count == 0);
            }
            else
            {
                yield return new WaitUntil(() => enemyAI.hasAttacked == true);
            }


            controller.StartCoroutine(controller.PassTurn());
        }
    }
}
