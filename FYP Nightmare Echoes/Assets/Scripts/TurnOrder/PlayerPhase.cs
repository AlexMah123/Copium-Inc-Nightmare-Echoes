using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NightmareEchoes.Unit;
using NightmareEchoes.Unit.Combat;

//created by Alex
namespace NightmareEchoes.TurnOrder
{
    public class PlayerPhase : Phase
    {
        protected override void OnEnter()
        {
            //Insert start of turn effects

            controller.StartCoroutine(WaitForTurnEnd());
        }

        protected override void OnUpdate()
        {

        }

        protected override void OnExit()
        {
            //update effects & stats
            if(controller.CurrentUnit != null)
            {
                controller.CurrentUnit.ApplyAllBuffDebuffs();
                controller.CurrentUnit.UpdateAllStatusEffectLifeTime();
                controller.CurrentUnit.UpdateAllStats();
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

            

            CombatManager.Instance.turnEnded = false;
            controller.PassTurn();
        }
        
    }
}
