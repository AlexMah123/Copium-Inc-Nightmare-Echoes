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
            //Init here


            //Insert start of turn effects
            if (controller.CurrentUnit != null)
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
                controller.CurrentUnit.ApplyAllTokenEffects();
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
            controller.StartCoroutine(controller.PassTurn());
        }
        
    }
}
