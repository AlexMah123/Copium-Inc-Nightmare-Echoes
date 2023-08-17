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
        BasicEnemyAI enemyAI;


        private List<Skill> aoeSkillsPassed = new();

        protected override void OnEnter()
        {
            //insert start of turn effects
            if (controller.CurrentUnit != null)
                enemyAI = controller.CurrentUnit.GetComponent<BasicEnemyAI>();

            controller.StartCoroutine(EnemyTurn());
            
            aoeSkillsPassed.Clear();

        }

        protected override void OnUpdate()
        {
            //start a couroutine to move
            if (enemyAI == null || controller.CurrentUnit == null) return;

            enemyAI.MoveProcess(controller.CurrentUnit);

            var aoeDmg = CombatManager.Instance.CheckAoe(controller.CurrentUnit);
            if (aoeDmg)
            {
                if (aoeSkillsPassed.Contains(aoeDmg)) return;
                aoeDmg.Cast(controller.CurrentUnit);
                aoeSkillsPassed.Add(aoeDmg);
            }
        }

        protected override void OnExit()
        {
            //update effects & stats
            controller.CurrentUnit.ApplyAllBuffDebuffs();
            controller.CurrentUnit.UpdateAllStatusEffectLifeTime();
            controller.CurrentUnit.UpdateAllStats();

            UIManager.Instance.UpdateStatusEffectUI();

            //when you change phases, change the current unit to the next unit
            if (controller.CurrentUnitQueue.Count > 0)
            {
                controller.CurrentUnitQueue.Dequeue();
            }
        }

        IEnumerator EnemyTurn()
        {
            
            yield return new WaitForSeconds(controller.enemyDelay);
            
            if(controller.CurrentUnit != null)
            {
                enemyAI.MakeDecision(controller.CurrentUnit);
                Debug.Log("Make Decision");
            }

            yield return new WaitUntil(() => enemyAI.totalPathList.Count == 0);

            //if there is at least 2 elements in queue
            if (controller.CurrentUnitQueue.Count > 1)
            {
                //if the second element exist, check hostile and change accordingly, else endPhase
                if (controller.CurrentUnitQueue.ToArray()[1].IsHostile)
                {
                    controller.ChangePhase(TurnOrderController.Instance.enemyPhase);
                }
                else
                {
                    controller.ChangePhase(TurnOrderController.Instance.playerPhase);

                }                
            }
            else
            {
                controller.ChangePhase(TurnOrderController.Instance.endPhase);
            }
        }
    }
}
