using System.Collections;
using System.Collections.Generic;
using NightmareEchoes.Unit.Combat;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.TurnOrder
{
    public class StartPhase : Phase
    {
        protected override void OnEnter()
        {
            GameUIManager.Instance.phaseText.text = $"Start of Round";
            GameUIManager.Instance.phaseText.color = Color.white;

            controller.StartCoroutine(Start());
            //check for any effects
            CombatManager.Instance.OnTurnStart();
        }

        protected override void OnFixedUpdate()
        {
            
        }

        protected override void OnUpdate()
        {
            
        }

        protected override void OnExit()
        {
            
        }

        IEnumerator Start()
        {
            yield return new WaitForSeconds(controller.phaseDelay);

            if(controller.CurrentUnit != null) 
            {
                if (!controller.CurrentUnit.IsHostile)
                {
                    controller.ChangePhase(controller.playerPhase);
                }
                else
                {
                    controller.ChangePhase(controller.enemyPhase);
                }
            }
            
        }
    }
}
