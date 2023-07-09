using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NightmareEchoes.TurnOrder
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] Button TestButton;
        [SerializeField] TextMeshProUGUI TurnOrderText;
        [SerializeField] Color playerTurn;
        [SerializeField] Color enemyTurn;


        private void Update()
        {
            switch(TurnOrder.Instance.gameState)
            {
                case GameState.PlayerTurn:
                    //if button is not interactable, on players turn, enable it
                    if(!TestButton.interactable)
                    {
                        TestButton.interactable = true;
                    }
                    TurnOrderText.text = $"Player's Turn";
                    TurnOrderText.color = new Color(playerTurn.r, playerTurn.g, playerTurn.b);

                    break;

                case GameState.EnemyTurn:
                    //if button is interactable, on enemy turn, enable it
                    if (TestButton.interactable)
                    {
                        TestButton.interactable = false;
                    }
                    TurnOrderText.text = $"Enemy's Turn";
                    TurnOrderText.color = new Color(enemyTurn.r, enemyTurn.g, enemyTurn.b);

                    break;
            }
        }

        public void PlayerAttackButton()
        {
            TurnOrder.Instance.gameState = GameState.EnemyTurn;
        }

        
    }
}
