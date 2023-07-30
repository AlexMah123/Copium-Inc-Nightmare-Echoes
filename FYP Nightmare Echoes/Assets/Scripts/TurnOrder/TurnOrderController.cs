using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NightmareEchoes.Unit;
using System.Linq;

//created by Alex
namespace NightmareEchoes.TurnOrder
{
    public class TurnOrderController : MonoBehaviour
    {
        public static TurnOrderController Instance;

        public Phase currentPhase;
        public PlanPhase planPhase = new PlanPhase();
        public StartPhase startPhase = new StartPhase();
        public PlayerPhase playerPhase = new PlayerPhase();
        public EnemyPhase enemyPhase = new EnemyPhase();
        public EndPhase endPhase = new EndPhase();

        [Header("Turn Order")]
        [SerializeField] List<BaseUnit> turnOrderList = new List<BaseUnit>();
        [SerializeField] List<BaseUnit> nextTurnOrderList = new List<BaseUnit>();
        Queue<BaseUnit> currentUnitQueue = new Queue<BaseUnit>();
        Queue<BaseUnit> nextUnitQueue = new Queue<BaseUnit>();
        public int turnCount;
        public float phaseDelay;
        public float enemyDelay;

        [SerializeField] BaseUnit currentUnit;
        public bool runOnce = false;

        #region Class Properties
        public BaseUnit CurrentUnit
        {
            get { return currentUnit; }
            set { currentUnit = value; }
        }

        public Queue<BaseUnit> CurrentUnitQueue
        {
            get { return currentUnitQueue; }
            private set { currentUnitQueue = value; }
        }

        public Queue<BaseUnit> NextUnitQueue
        {
            get { return nextUnitQueue; }
            private set { nextUnitQueue = value; }
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

        void Start()
        {
            turnCount = 1;
            ChangePhase(planPhase);
        }

        void Update()
        {
            

            if(currentPhase != null)
            {
                currentPhase.OnUpdatePhase();
            }

        }

        public void ChangePhase(Phase newPhase)
        {
            if(currentPhase != null) 
            {
                currentPhase.OnExitPhase();
            }

            currentPhase = newPhase;
            currentPhase.OnEnterPhase(this);
        }


        #region Turn Order Calculations
        public void CalculateTurnOrder()
        {
            turnOrderList = FindObjectsOfType<BaseUnit>().ToList();
            turnOrderList.Sort(CompareSpeed); //sorts in ascending order
            turnOrderList.Reverse();

            CurrentUnitQueue.Clear();

            for(int i = 0; i < turnOrderList.Count; i++)
            {
                CurrentUnitQueue.Enqueue(turnOrderList[i]);
            }
        }

        //delegate for sort()
        int CompareSpeed(BaseUnit _a, BaseUnit _b)
        {
            return _a.Speed.CompareTo(_b.Speed);
        }
        #endregion
    }
}
