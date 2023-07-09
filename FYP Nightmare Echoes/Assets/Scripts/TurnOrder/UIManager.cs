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

        [SerializeField] Button TestButton;

        [Header("Action Bar")]
        [SerializeField] GameObject actionBarPanel;
        [SerializeField] TextMeshProUGUI turnOrderText;
        [SerializeField] Color playerTurn;
        [SerializeField] Color enemyTurn;

        [Header("Unit Info")]
        [SerializeField] TextMeshProUGUI unitNameText;
        [SerializeField] TextMeshProUGUI unitHealthText;
        [SerializeField] TextMeshProUGUI unitSpeedText;
        public BaseUnit currentUnit;

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
                    if (!TestButton.interactable)
                    {
                        TestButton.interactable = true;
                    }
                    turnOrderText.text = $"Player's Turn";
                    turnOrderText.color = new Color(playerTurn.r, playerTurn.g, playerTurn.b);

                    break;

                case GameState.EnemyTurn:
                    //if button is interactable, on enemy turn, enable it
                    if (TestButton.interactable)
                    {
                        TestButton.interactable = false;
                    }
                    turnOrderText.text = $"Enemy's Turn";
                    turnOrderText.color = new Color(enemyTurn.r, enemyTurn.g, enemyTurn.b);

                    break;
            }
            #endregion

            #region UnitInfo
            unitNameText.text = $"Name: {currentUnit.Name}";
            unitHealthText.text = $"Health: {currentUnit.Health}";
            unitSpeedText.text = $"Speed: {currentUnit.Speed}";

            #endregion
        }

        public void PlayerAttackButton()
        {
            TurnOrder.Instance.gameState = GameState.EnemyTurn;

        }

        
    }
}
