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
            //Init here
            if (controller.CurrentUnit != null)
                enemyAI = controller.CurrentUnit.GetComponent<BasicEnemyAI>();


            //start of turn effects
            if(controller.CurrentUnit != null)
            {
                controller.CurrentUnit.ApplyAllTokenEffects();

                if (controller.CurrentUnit.StunToken == true)
                {
                    controller.CurrentUnit.StunToken = false;

                    UIManager.Instance.EnableCurrentUI(false);
                    UIManager.Instance.UpdateStatusEffectUI();

                    controller.StartCoroutine(controller.PassTurn());
                }
            }

            
            //Start Turn
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
            if (controller.CurrentUnit != null)
            {
                //Hide tiles only on exit
                PathfindingManager.Instance.HideTilesInRange(enemyAI.tilesInRange);

                //update effects & stats
                controller.CurrentUnit.ApplyAllBuffDebuffs();
                controller.CurrentUnit.ApplyAllTokenEffects();
                controller.CurrentUnit.UpdateAllStatusEffectLifeTime();
                controller.CurrentUnit.UpdateAllStats();
            }
            

            UIManager.Instance.UpdateStatusEffectUI();

            //when you change phases, change the current unit to the next unit
            if (controller.CurrentUnitQueue.Count > 0)
            {
                controller.CurrentUnitQueue.Dequeue();
                //controller.passedTurnOrderList.Add(controller.CurrentUnit);
            }
        }

        IEnumerator EnemyTurn()
        {
            
            yield return new WaitForSeconds(controller.enemythinkingDelay);
            
            if(controller.CurrentUnit != null)
            {
                enemyAI.MakeDecision(controller.CurrentUnit);
            }

            yield return new WaitUntil(() => enemyAI.totalPathList.Count == 0);

            controller.StartCoroutine(controller.PassTurn());
        }
    }
}
