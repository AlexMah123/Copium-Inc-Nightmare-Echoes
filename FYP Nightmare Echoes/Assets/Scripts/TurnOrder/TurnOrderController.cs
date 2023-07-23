using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NightmareEchoes.Unit;
using System.Linq;
using System;

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
        [NonSerialized] public BaseUnit[] unitArray;
        [SerializeField] List<BaseUnit> turnOrderList;
        public float delay;

        [SerializeField] BaseUnit currentUnit;
        public int currentUnitIterator = 0;

        #region Class Properties
        public BaseUnit CurrentUnit
        {
            get { return currentUnit; }
            set { currentUnit = value; }
        }

        public List<BaseUnit> TurnOrderList
        {
            get { return turnOrderList; }
            private set { turnOrderList = value; }
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
        #endregion
    }
}
