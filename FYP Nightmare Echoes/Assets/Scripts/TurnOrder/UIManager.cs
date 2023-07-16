using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NightmareEchoes.Unit;
using System;

namespace NightmareEchoes.TurnOrder
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [Header("Test Buttons")] // basic attack
        [SerializeField] Button testButton;

        [Header("Turn Order Bar")]
        [SerializeField] GameObject turnOrderPanel;
        [SerializeField] TextMeshProUGUI turnOrderText;
        [SerializeField] Color playerTurn;
        [SerializeField] Color enemyTurn;

        [Header("Hotbar Info")]
        [SerializeField] List<Button> turnOrderButtons;
        [SerializeField] TextMeshProUGUI currentUnitNameText;
        public BaseUnit currentUnit;
        
        GameObject temporaryInspectedUnit;

        [Header("Settings")]
        [SerializeField] Button settingButton;
        [SerializeField] GameObject settingsPanel;
        public bool gameIsPaused = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            #region TurnOrderBar
            switch (TurnOrder.Instance.gameState)
            {
                case GameState.PlayerTurn:
                    EnablePlayerUI(true);

                    turnOrderText.text = $"Player's Turn";
                    turnOrderText.color = new Color(playerTurn.r, playerTurn.g, playerTurn.b);

                    break;

                case GameState.EnemyTurn:
                    EnablePlayerUI(false);

                    turnOrderText.text = $"Enemy's Turn";
                    turnOrderText.color = new Color(enemyTurn.r, enemyTurn.g, enemyTurn.b);

                    break;
            }
            #endregion

            #region UnitInfo
            currentUnitNameText.text = $"{currentUnit.Name}";
            //unitHealthText.text = $"Health: {currentUnit.Health}";
            //unitSpeedText.text = $"Speed: {currentUnit.Speed}";

            #endregion

            if(Input.GetMouseButtonDown(0)) // rightclick on an inspectable unit
            {

            }
        }


        #region Button Functions
        public void PlayerAttackButton()
        {
            TurnOrder.Instance.gameState = GameState.EnemyTurn;
            currentUnit.BasicAttack();
        }

        public void SettingsButton()
        {
            if(Time.timeScale > 0)
            {
                gameIsPaused = true;
                Time.timeScale = 0;
                settingsPanel.SetActive(true);
            }
            else
            {
                gameIsPaused = false;
                Time.timeScale = 1;
                settingsPanel.SetActive(false);
            }
        }

        #endregion


        void EnablePlayerUI(bool enable)
        {
            foreach (Button button in turnOrderButtons)
            {
                button.interactable = enable;
            }
        }
    }
}
