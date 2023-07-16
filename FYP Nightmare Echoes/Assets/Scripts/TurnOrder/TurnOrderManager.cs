using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NightmareEchoes.Unit;

namespace NightmareEchoes.TurnOrder
{
    public class TurnOrderManager : MonoBehaviour
    {
        public static TurnOrderManager Instance;

        [Header("Turn Order")]
        public BaseUnit[] turnOrderList;
        public GameState gameState;
        [SerializeField] float delay;

        BaseUnit currentUnit;
        private int currentUnitIterator = -1;

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
            gameState = GameState.Start;
            currentUnitIterator = -1;
            turnOrderList = FindObjectsOfType<BaseUnit>();
            

            StartCoroutine(PlayerTurn());
        }

        IEnumerator PlayerTurn()
        {
            //yield return new WaitForSeconds(2f);
            currentUnitIterator++;
            if(currentUnitIterator < turnOrderList.Length)
            {
                currentUnit = turnOrderList[currentUnitIterator];
            }

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

        void CheckEffects()
        {
            
        }

        IEnumerator WinState()
        {
            yield break;
        }

        IEnumerator LoseState()
        {
            yield break;
        }


        public BaseUnit GetCurrentUnit()
        {
            return currentUnit;
        }
        
    }
    public enum GameState
    {
        Start = 0,
        PlayerTurn = 1,
        EnemyTurn = 2,
        CheckEffects = 3,
        WinState = 4,
        LoseState = 5,
    }

}

