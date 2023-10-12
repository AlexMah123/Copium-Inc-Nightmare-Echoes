using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
        bool tempStun = false;
        private bool madeDecision;

        BasicEnemyAI enemyAI;

        private List<Skill> aoeSkillsPassed = new();

        protected override void OnEnter()
        {
            //Reseting Values
            tempStun = false;
            madeDecision = false;

            #region Insert Start of Turn Effects/Checks
            if (controller.CurrentUnit != null)
            {
                //cache enemyAI
                enemyAI = controller.CurrentUnit.GetComponent<BasicEnemyAI>();

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
            if (!madeDecision) return;
            
            var aoeDmg = CombatManager.Instance.CheckAoe(controller.CurrentUnit);
            if (aoeDmg)
            {
                if (aoeSkillsPassed.Contains(aoeDmg)) return;
                if (aoeDmg.Cast(controller.CurrentUnit)) aoeSkillsPassed.Add(aoeDmg);
            }
            
            var trapDmg = CombatManager.Instance.CheckTrap(controller.CurrentUnit);
            if (trapDmg)
            {
                trapDmg.Cast(controller.CurrentUnit);
            }
        }

        protected override void OnExit()
        {
            if (controller.CurrentUnit != null && enemyAI != null)
            {
                #region End of Turn Effects
                if (controller.CurrentUnit.ImmobilizeToken)
                {
                    controller.CurrentUnit.UpdateTokenLifeTime(STATUS_EFFECT.IMMOBILIZE_TOKEN);
                }
                #endregion

                #region Mandatory Checks

                //Hide tiles only on exit
                if (enemyAI.TilesInRange?.Count > 0)
                {
                    PathfindingManager.Instance.HideTilesInRange(enemyAI.TilesInRange);
                }

                //update effects & stats
                controller.CurrentUnit.ApplyAllBuffDebuffs();
                //should not need this but just checking
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
            
            yield return new WaitForSeconds(controller.enemythinkingDelay);
            
            if(controller.CurrentUnit != null)
            {
                enemyAI.MakeDecision(controller.CurrentUnit);
            }

            madeDecision = true;
            yield return new WaitUntil(() => enemyAI.totalPathList.Count == 0);

            if(controller.CurrentUnit.ImmobilizeToken)
            {
                controller.StartCoroutine(controller.PassTurn());
            }
            else if (!enemyAI.inAtkRange && !enemyAI.inMoveAndAttackRange)
            {
                controller.StartCoroutine(controller.PassTurn());
            }
            else
            {
                yield return new WaitUntil(() => CombatManager.Instance.turnEnded);
                controller.StartCoroutine(controller.PassTurn());
            }


        }
    }
}
