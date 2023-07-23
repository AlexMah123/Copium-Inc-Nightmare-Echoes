using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.TurnOrder
{
    public class PlayerPhase : Phase
    {
        protected override void OnEnter()
        {
            UIManager.Instance.EnablePlayerUI(true);

        }

        protected override void OnUpdate()
        {
            
        }

        protected override void OnExit()
        {
            if (controller.currentUnitIterator < controller.TurnOrderList.Count)
            {
                controller.CurrentUnit = controller.TurnOrderList[controller.currentUnitIterator++];
            }
        }
    }
}
