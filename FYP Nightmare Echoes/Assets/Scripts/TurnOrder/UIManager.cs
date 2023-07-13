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

        [Header("Test Buttons")]
        [SerializeField] Button testButton;

        [Header("Action Bar")]
        [SerializeField] GameObject actionBarPanel;
        [SerializeField] TextMeshProUGUI turnOrderText;
        [SerializeField] Color playerTurn;
        [SerializeField] Color enemyTurn;

        [Header("Hotbar Info")]
        [SerializeField] TextMeshProUGUI unitNameText;
        public BaseUnit currentUnit;

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
            #region ActionBar
            switch (TurnOrder.Instance.gameState)
            {
                case GameState.PlayerTurn:
                    //if button is not interactable, on players turn, enable it
                    if (!testButton.interactable)
                    {
                        testButton.interactable = true;
                    }
                    turnOrderText.text = $"Player's Turn";
                    turnOrderText.color = new Color(playerTurn.r, playerTurn.g, playerTurn.b);

                    break;

                case GameState.EnemyTurn:
                    //if button is interactable, on enemy turn, enable it
                    if (testButton.interactable)
                    {
                        testButton.interactable = false;
                    }
                    turnOrderText.text = $"Enemy's Turn";
                    turnOrderText.color = new Color(enemyTurn.r, enemyTurn.g, enemyTurn.b);

                    break;
            }
            #endregion

            #region UnitInfo
            unitNameText.text = $"Name: {currentUnit.Name}";
            //unitHealthText.text = $"Health: {currentUnit.Health}";
            //unitSpeedText.text = $"Speed: {currentUnit.Speed}";

            #endregion
        }


        #region Button Functions
        public void PlayerAttackButton()
        {
            TurnOrder.Instance.gameState = GameState.EnemyTurn;

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

    }
}
