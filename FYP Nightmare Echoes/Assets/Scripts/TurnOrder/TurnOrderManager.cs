using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NightmareEchoes.Unit;
using System.Linq;

//created by Alex
namespace NightmareEchoes.TurnOrder
{
    public class TurnOrderManager : MonoBehaviour
    {
        public static TurnOrderManager Instance;

        [Header("Turn Order")]
        private BaseUnit[] unitArray;
        [SerializeField] List<BaseUnit> turnOrderList;
        [SerializeField] GameState gameState;
        [SerializeField] float delay;

        BaseUnit currentUnit;
        private int currentUnitIterator = -1;

        #region Class Properties
        public BaseUnit CurrentUnit
        {
            get { return currentUnit; }
            private set { currentUnit = value; }
        }

        public List<BaseUnit> TurnOrderList
        {
            get { return turnOrderList; }
            private set { turnOrderList = value; }
        }

        public GameState GameState
        {
            get { return gameState; }
            set { gameState = value; }
        }
        #endregion

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
            InitializeTurn();
        }


        IEnumerator PlayerTurn()
        {
            //yield return new WaitForSeconds(2f);
            currentUnitIterator++;
            if(currentUnitIterator < unitArray.Length)
            {
                currentUnit = unitArray[currentUnitIterator];
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


        void InitializeTurn()
        {
            gameState = GameState.Start;
            currentUnitIterator = -1;
            CalculatedTurnOrder();

            StartCoroutine(PlayerTurn());
        }

        public void CalculatedTurnOrder()
        {
            unitArray = FindObjectsOfType<BaseUnit>();
            turnOrderList = unitArray.ToList();
            turnOrderList.Sort(CompareSpeed); //sorts in ascending order
            turnOrderList.Reverse();

            //Debug.Log("Calculate turn order");
            //Debug.Log(turnOrderList[0]);
        }

        //delegate for sort()
        int CompareSpeed(BaseUnit _a, BaseUnit _b)
        {
            return _a.Speed.CompareTo(_b.Speed);
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

