using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.TurnOrder
{
    public abstract class Phase
    {
        protected TurnOrderController controller;

        public void OnEnterPhase(TurnOrderController turnOrderController)
        {
            controller = turnOrderController;
            controller.CalculatedTurnOrder();
            UIManager.Instance.UpdateTurnOrderUI();

            OnEnter();
        }

        public void OnUpdatePhase()
        {
            OnUpdate();
        }

        public void OnExitPhase()
        {
            OnExit();
        }

        //overrides
        protected virtual void OnEnter()
        {

        }

        protected virtual void OnUpdate()
        {

        }

        protected virtual void OnExit() 
        { 
            
        }
    }
}
