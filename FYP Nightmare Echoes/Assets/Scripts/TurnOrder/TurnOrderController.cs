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
        public List<Units> turnOrderList = new List<Units>();
        public List<Units> passedTurnOrderList = new List<Units>();
        Queue<Units> currentUnitQueue = new Queue<Units>();
        //Queue<Units> nextUnitQueue = new Queue<Units>();
        public int turnCount;
        public float phaseDelay;
        public float enemythinkingDelay;
        public float passTurnDelay;

        [SerializeField] Units currentUnit;
        public bool runOnce = false;

        #region Class Properties
        public Units CurrentUnit
        {
            get { return currentUnit; }
            set { currentUnit = value; }
        }

        public Queue<Units> CurrentUnitQueue
        {
            get { return currentUnitQueue; }
            private set { currentUnitQueue = value; }
        }

        /*public Queue<Units> NextUnitQueue
        {
            get { return nextUnitQueue; }
            private set { nextUnitQueue = value; }
        }*/

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

        public IEnumerator PassTurn()
        {
            yield return new WaitForSeconds(passTurnDelay);

            //if there is at least 2 elements in queue
            if (CurrentUnitQueue.Count > 1)
            {
                //if the second element exist, check hostile and change accordingly, else endPhase
                if (CurrentUnitQueue.ToArray()[1].IsHostile)
                {
                   ChangePhase(enemyPhase);
                }
                else
                {
                    ChangePhase(playerPhase);
                }
            }
            else
            {
                ChangePhase(endPhase);
            }
        }

        #region Turn Order Calculations
        public void CalculateTurnOrder()
        {
            turnOrderList = FindObjectsOfType<Units>().ToList();
            turnOrderList.Sort(CompareSpeed); //sorts in ascending order
            turnOrderList.Reverse();

            CurrentUnitQueue.Clear();

            for(int i = 0; i < turnOrderList.Count; i++)
            {
                CurrentUnitQueue.Enqueue(turnOrderList[i]);
            }
        }

        //delegate for sort()
        int CompareSpeed(Units _a, Units _b)
        {
            if (_a.stats.Speed == _b.stats.Speed)
            {
                int rand = Random.Range(0, 2);

                switch (rand)
                {
                    case 0:
                        return -1;
                    case 1:
                        return 1;
                }
            }

            return _a.stats.Speed.CompareTo(_b.stats.Speed);
        }
        #endregion
    }
}
