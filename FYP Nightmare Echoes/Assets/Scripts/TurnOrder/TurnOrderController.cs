using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NightmareEchoes.Unit;
using System.Linq;
using System;
using UnityEditor.Search;

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
        [SerializeField] List<BaseUnit> turnOrderList;
        Queue<BaseUnit> unitQueue = new Queue<BaseUnit>();
        public float delay;

        [SerializeField] BaseUnit currentUnit;
        public bool runOnce = false;

        #region Class Properties
        public BaseUnit CurrentUnit
        {
            get { return currentUnit; }
            set { currentUnit = value; }
        }

        public Queue<BaseUnit> UnitQueue
        {
            get { return unitQueue; }
            private set { unitQueue = value; }
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
        public void CalculatedTurnOrder()
        {
            turnOrderList = FindObjectsOfType<BaseUnit>().ToList();
            turnOrderList.Sort(CompareSpeed); //sorts in ascending order
            turnOrderList.Reverse();

            UnitQueue.Clear();

            for(int i = 0; i < turnOrderList.Count; i++)
            {
                UnitQueue.Enqueue(turnOrderList[i]);
            }

            //Debug.Log("Calculate turn order");
            //Debug.Log(turnOrderList[0]);
        }

        //delegate for sort()
        int CompareSpeed(BaseUnit _a, BaseUnit _b)
        {
            return _a.Speed.CompareTo(_b.Speed);
        }
        #endregion
    }
}
