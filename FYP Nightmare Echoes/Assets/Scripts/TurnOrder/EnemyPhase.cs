using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.TurnOrder
{
    public class EnemyPhase : Phase
    {
        protected override void OnEnter()
        {
            //controller.CurrentUnit.TakeDamage(2);
            controller.StartCoroutine(EnemyTurn());

            //insert start of turn effects
        }

        protected override void OnUpdate()
        {

        }

        protected override void OnExit()
        {
            //when you change phases, change the current unit to the next unit
            if (controller.CurrentUnitQueue.Count > 0)
            {
                controller.CurrentUnitQueue.Dequeue();
            }
        }

        IEnumerator EnemyTurn()
        {
            
            yield return new WaitForSeconds(controller.enemyDelay);


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
