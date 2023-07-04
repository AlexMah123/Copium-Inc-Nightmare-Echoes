using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Serialization;
using UnityEngine;

namespace NightmareEchoes.TurnOrder
{
    public class TurnOrder : MonoBehaviour
    {
        GameState gameState;

        private void Start()
        {
            gameState = GameState.Start;
            StartCoroutine(PlayerAction());
        }

        IEnumerator PlayerAction()
        {
            Debug.Log(gameState);
            yield return new WaitForSeconds(2f);

            gameState = GameState.EnemyTurn;
            yield return new WaitUntil(()=> gameState == GameState.EnemyTurn);
            StartCoroutine(EnemyAction());
        }

        IEnumerator EnemyAction()
        {
            Debug.Log(gameState);
            yield break;
            //yield return new WaitUntil(()=> gameState == GameState.PlayerTurn);
        }

    }

    public enum GameState
    {
        Start = 1,
        PlayerTurn = 2,
        EnemyTurn = 3,
        CheckEffects = 4,
        WinState = 5,
        LoseState = 6,
    }
}

