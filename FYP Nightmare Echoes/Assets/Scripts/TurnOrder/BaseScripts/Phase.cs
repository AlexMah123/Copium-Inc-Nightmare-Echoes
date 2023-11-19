using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NightmareEchoes.Inputs;
using NightmareEchoes.Unit;
using NightmareEchoes.Unit.Combat;
using NightmareEchoes.Unit.Pathfinding;
using UnityEngine.SceneManagement;
using NightmareEchoes.Grid;


//created by Alex
namespace NightmareEchoes.TurnOrder
{
    public abstract class Phase
    {
        protected TurnOrderController controller;
        bool progressTutorial = false;


        public void OnEnterPhase(TurnOrderController turnOrderController)
        {
            //assigns the controller as reference
            controller = turnOrderController;

            if (progressTutorial)
            {
                progressTutorial = false;
            }

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
                GameUIManager.Instance.EnableCurrentUI(true);
            }
            else
            {
                GameUIManager.Instance.EnableCurrentUI(false);
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
            GameUIManager.Instance.UpdateTurnOrderUI();
            GameUIManager.Instance.UpdateStatusEffectUI();

            OnEnter();
        }

        public void OnFixedUpdatePhase()
        {
            if (controller.gameOver)
                return;

            if (controller.cachedHeroesList == null)
            {
                controller.cachedHeroesList = controller.FindAllHeros();
            }

            if (controller.currentPhase != controller.planPhase && controller.currentPhase != controller.startPhase && !controller.gameOver)
            {
                //checking for players
                if (controller.FindAllHeros().Count == 0)
                {
                    //Game Over
                    controller.gameOver = true;
                    GeneralUIController.Instance.GameOver();
                }
            }

            //checking for enemies to progress
            if (controller.FindAllEnemies().Count == 0 && !controller.InTutorialLevel())
            {
                GeneralUIController.Instance.GameVictory();
            }
            else if (controller.FindAllEnemies().Count == 0 && controller.InTutorialLevel() && !progressTutorial)
            {
                controller.tutorialPart = (TutorialPart)((int)(controller.tutorialPart + 1));
                progressTutorial = true;
            }

            OnFixedUpdate();
        }

        public void OnUpdatePhase()
        {
            //if you want to pause
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if(GeneralUIController.Instance.guidePanel.activeSelf)
                {
                    GeneralUIController.Instance.GuideButton();
                }
                else
                {
                    GeneralUIController.Instance.PauseButton();

                }
            }

            // if you want to open up the guide
            if (Input.GetKeyDown(KeyCode.F1))
            {
                //GeneralUIController.Instance.GuideButton();
            }

            OnUpdate();
        }

        public void OnExitPhase()
        {
            OnExit();

            //disable skill info
            GameUIManager.Instance.EnableSkillInfo(false);

            //reset pathfinding
            PathfindingManager.Instance.isMoving = false;
            PathfindingManager.Instance.hasMoved = false;
            PathfindingManager.Instance.isDragging = false;
            PathfindingManager.Instance.lastAddedTile = null;

            if (PathfindingManager.Instance.tempPathList.Count > 0)
            {
                PathfindingManager.Instance.ClearArrow(PathfindingManager.Instance.pathList);
            }

            if (PathfindingManager.Instance.tempPathList.Count > 0)
            {
                PathfindingManager.Instance.ClearArrow(PathfindingManager.Instance.tempPathList);
            }
            
            PathfindingManager.Instance.pathList.Clear();
            PathfindingManager.Instance.tempPathList.Clear();

            //clear all rendering 
            RenderOverlayTile.Instance.ClearTargetingRenders();
            CombatManager.Instance.ClearPreviews();
            CombatManager.Instance.turnEnded = false;

            //reset camera panning
            CameraControl.Instance.isPanning = false;

            controller.StopAllCoroutines();
        }

        //overrides
        protected virtual void OnEnter()
        {

        }

        protected virtual void OnFixedUpdate()
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
