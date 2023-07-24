using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.TurnOrder
{
    public class PlayerPhase : Phase
    {
        protected override void OnEnter()
        {

        }

        protected override void OnUpdate()
        {
            if (controller.UnitQueue.Count <= 0)
            {
                controller.ChangePhase(controller.endPhase);
            }
        }

        protected override void OnExit()
        {
            if (controller.UnitQueue.Count > 0)
            {
                controller.CurrentUnit = controller.UnitQueue.Dequeue();
            }
        }
    }
}
