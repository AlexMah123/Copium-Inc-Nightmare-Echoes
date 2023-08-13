using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NightmareEchoes.Inputs;


//created by Alex
namespace NightmareEchoes.TurnOrder
{
    public abstract class Phase
    {
        bool newTurn = false;
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

            //updates the UI during each phase & updates status effect 
            UIManager.Instance.UpdateTurnOrderUI();
            UIManager.Instance.UpdateStatusEffectUI();


            //updates the current unit for turn order
            if (controller.CurrentUnitQueue.Count > 0 ) 
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

            if(controller.currentPhase == controller.playerPhase || controller.currentPhase == controller.enemyPhase)
            {
                CameraControl.Instance.UpdateCameraPan(controller.CurrentUnit.gameObject);
            }

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
