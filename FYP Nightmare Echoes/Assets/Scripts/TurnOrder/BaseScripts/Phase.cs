using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.TurnOrder
{
    public abstract class Phase
    {
        protected TurnOrderController controller;

        public void OnEnterPhase(TurnOrderController turnOrderController)
        {
            //assigns the controller as reference
            controller = turnOrderController;

            //only run once to calculate the turn order and enqueue till the endPhase
            if(!controller.runOnce)
            {
                controller.CalculateTurnOrder();
                controller.runOnce = true;
            }

            //updates the UI for turn order
            UIManager.Instance.UpdateTurnOrderUI();

            //updates the current unit for turn order

            if(controller.CurrentUnitQueue.Count > 0 ) 
            {
                controller.CurrentUnit = controller.CurrentUnitQueue.Peek();
            }

            #region UI
            if (controller.currentPhase == controller.playerPhase)
            {
                UIManager.Instance.EnablePlayerUI(true);
            }
            else
            {
                UIManager.Instance.EnablePlayerUI(false);
            }

            #endregion

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
