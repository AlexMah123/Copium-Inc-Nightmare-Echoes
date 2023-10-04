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
        public List<Entity> turnOrderList = new List<Entity>();
        Queue<Entity> currentUnitQueue = new Queue<Entity>();
        //Queue<Units> nextUnitQueue = new Queue<Units>();
        public int cycleCount;
        public float phaseDelay;
        public float enemythinkingDelay;
        public float passTurnDelay;

        [SerializeField] Entity currentUnit;
        public bool runOnce = false;
        public bool gameOver = false;

        [Header("Hero List")]
        List<Entity> totalUnitList = new List<Entity>();
        List<Entity> totalHeroList = new List<Entity>();
        List<Entity> totalEnemiesList = new List<Entity>();


        #region Class Properties
        public Entity CurrentUnit
        {
            get { return currentUnit; }
            set { currentUnit = value; }
        }

        public Queue<Entity> CurrentUnitQueue
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
            Invoke("StartGame", 1f);
        }

        void Update()
        {
            if(currentPhase != null)
            {
                currentPhase.OnUpdatePhase();
            }
        }

        void StartGame()
        {
            cycleCount = 1;
            ChangePhase(planPhase);
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

        #region Utility

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

        [ContextMenu("Skip turn")]
        public void SkipTurn()
        {
            StartCoroutine(PassTurn());
        }

        #endregion

        #region Turn Order Calculations
        public void CalculateTurnOrder()
        {
            turnOrderList = new List<Entity>(FindObjectsOfType<Entity>());
            
            var iterator = new List<Entity>(turnOrderList);
            foreach (var unit in iterator.Where(unit => unit.IsProp))
            {
                turnOrderList.Remove(unit);
            }
            
            turnOrderList.Sort(CompareSpeed); //sorts in ascending order
            turnOrderList.Reverse();

            CurrentUnitQueue.Clear();

            for(int i = 0; i < turnOrderList.Count; i++)
            {
                CurrentUnitQueue.Enqueue(turnOrderList[i]);
                turnOrderList[i].OnDestroyedEvent += OnUnitDestroy;
                turnOrderList[i].OnAddBuffEvent += UIManager.Instance.UpdateStatusEffectUI;
            }
        }

        private void OnUnitDestroy(Entity destroyedUnit)
        {
            if(destroyedUnit == currentUnit)
            {
                StartCoroutine(PassTurn());
            }
            else
            {
                CurrentUnitQueue = new Queue<Entity>(CurrentUnitQueue.Where(x => x != destroyedUnit));
            }

            //unsub the destroyed unit's events
            destroyedUnit.OnDestroyedEvent -= OnUnitDestroy;
            destroyedUnit.OnAddBuffEvent -= UIManager.Instance.UpdateStatusEffectUI;


            if (UIManager.Instance != null) 
            {
                if(UIManager.Instance.turnOrderSpritePool.Count > 0)
                {
                    UIManager.Instance.UpdateTurnOrderUI();
                }
            }
        }

        //delegate for sort()
        int CompareSpeed(Entity _a, Entity _b)
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


        public List<Entity> FindAllHeros()
        {
            //check if any hero exist
            totalHeroList.Clear();
            totalUnitList = FindObjectsOfType<Entity>().ToList();

            //filter by heroes
            foreach (var unit in totalUnitList)
            {
                if (!unit.IsHostile && !unit.IsProp)
                {
                    totalHeroList.Add(unit);
                }
            }

            if (totalHeroList.Count > 0) 
            {
                return totalUnitList;
            }
            else
            {
                return null;
            }
        }

        public List<Entity> FindAllEnemies()
        {
            //check if any hero exist
            totalEnemiesList.Clear();
            totalUnitList = FindObjectsOfType<Entity>().ToList();

            //filter by heroes
            foreach (var unit in totalUnitList)
            {
                if (unit.IsHostile && !unit.IsProp)
                {
                    totalEnemiesList.Add(unit);
                }
            }

            if (totalEnemiesList.Count > 0)
            {
                return totalEnemiesList;
            }
            else
            {
                return null;
            }
        }



    }
}
