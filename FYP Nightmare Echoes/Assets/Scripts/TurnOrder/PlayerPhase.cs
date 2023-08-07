using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.TurnOrder
{
    public class PlayerPhase : Phase
    {
        protected override void OnEnter()
        {
            controller.CurrentUnit.TakeDamage(2);
            //Debug.Log($"{controller.CurrentUnit.Name} - Taking Damage, Current Health: {controller.CurrentUnit.Health}");

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
    }
}
