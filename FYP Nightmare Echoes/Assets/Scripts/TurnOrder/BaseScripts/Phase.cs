using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NightmareEchoes.Inputs;
using NightmareEchoes.Unit;
using NightmareEchoes.Unit.Combat;
using NightmareEchoes.Unit.Pathfinding;
using UnityEngine.SceneManagement;


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
            if (!controller.runOnce)
            {
                controller.runOnce = true;
                controller.CalculateTurnOrder();
            }

            //updates the current unit for turn order
            if (controller.CurrentUnitQueue.Count > 0 ) 
            {
                controller.CurrentUnit = controller.CurrentUnitQueue.Peek();
            }

            #region UI
            if ((controller.currentPhase == controller.playerPhase) && !controller.CurrentUnit.StunToken && !controller.CurrentUnit.IsHostile)
            {
                UIManager.Instance.EnableCurrentUI(true);
            }
            else
            {
                UIManager.Instance.EnableCurrentUI(false);
            }

            #endregion

            if(controller.currentPhase == controller.playerPhase || controller.currentPhase == controller.enemyPhase)
            {
                CameraControl.Instance.isPanning = false;

                if (controller.CurrentUnit.gameObject != null)
                {
                    CameraControl.Instance.UpdateCameraPan(controller.CurrentUnit.gameObject);
                }
            }

            //updates the UI during each phase & updates status effect 
            UIManager.Instance.UpdateTurnOrderUI();
            UIManager.Instance.UpdateStatusEffectUI();

            OnEnter();
        }

        public void OnUpdatePhase()
        {
            if (controller.currentPhase != controller.planPhase && controller.currentPhase != controller.startPhase && !controller.gameOver)
            {
                if (controller.FindAllHeros() == null)
                {
                    //Game Over
                    controller.gameOver = true;
                    UIManager.Instance.GameOver();
                }

                if (controller.FindAllEnemies() == null)
                {
                    SceneManager.LoadScene(0);
                }

            }


            OnUpdate();
        }

        public void OnExitPhase()
        {
            OnExit();

            UIManager.Instance.EnableSkillInfo(false);
            PathfindingManager.Instance.ifSelectedUnit = false;
            CombatManager.Instance.turnEnded = false;
            CameraControl.Instance.isPanning = false;
            PathfindingManager.Instance.isMoving = false;

            controller.StopAllCoroutines();
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
