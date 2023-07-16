using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.TurnOrder
{
    public class TurnOrder : MonoBehaviour
    {
        public static TurnOrder Instance;
        public GameState gameState;

        [SerializeField] float delay;

        private void Awake()
        {
            if(Instance != null && Instance != this)
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
            gameState = GameState.Start;
            StartCoroutine(PlayerTurn());
        }

        IEnumerator PlayerTurn()
        {
            //yield return new WaitForSeconds(2f);
            gameState = GameState.PlayerTurn;

            yield return new WaitUntil(()=> gameState == GameState.EnemyTurn);
            StartCoroutine(EnemyTurn());
        }

        IEnumerator EnemyTurn()
        {
            Debug.Log(gameState);
            yield return new WaitForSeconds(delay);

            gameState = GameState.PlayerTurn;
            yield return new WaitUntil(()=> gameState == GameState.PlayerTurn);
            StartCoroutine(PlayerTurn());
        }

        IEnumerator CheckEffects()
        {
            yield break;
        }

        IEnumerator WinState()
        {
            yield break;
        }

        IEnumerator LoseState()
        {
            yield break;
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

