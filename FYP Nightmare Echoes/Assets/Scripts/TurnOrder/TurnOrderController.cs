using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using NightmareEchoes.UI;
using NightmareEchoes.Grid;
using NightmareEchoes.Unit;
using NightmareEchoes.Unit.Combat;
using NightmareEchoes.Scene;
using NightmareEchoes.Unit.Pathfinding;

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

        public List<Entity> cachedHeroesList = null;

        public TutorialPart tutorialPart;

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
            cycleCount = 1;
            ChangePhase(planPhase);

            //if you are in the tutorial level but tutorial hasnt started
            if (InTutorialLevel() && TutorialUIManager.Instance.InTutorialState() == false)
            {
                tutorialPart = TutorialPart.Part1;

                TutorialUIManager.Instance.StartCoroutine(TutorialUIManager.Instance.StartTutorial());
                TutorialUIManager.Instance.StartCoroutine(TutorialSteps());
            }
        }

        void Update()
        {
            if(currentPhase != null)
            {
                currentPhase.OnUpdatePhase();
            }
        }

        private void FixedUpdate()
        {
            if(currentPhase != null)
            {
                currentPhase.OnFixedUpdatePhase();
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

        #region Tutorial Section
        public bool InTutorialLevel()
        {
            return SceneManager.GetActiveScene().buildIndex == (int)SCENEINDEX.TUTORIAL_SCENE ? true : false;
        }

        #endregion

        #region Utility
        [ContextMenu("Skip Turn")]
        public void SkipTurn()
        {
            StartCoroutine(PassTurn());
            GameUIManager.Instance.PassTurnButton();
        }

        public IEnumerator PassTurn()
        {
            GameUIManager.Instance.EnableCurrentUI(false);

            //wait for all popuptext to clear before actually changing turn
            while (AnyUnitHasPendingPopups())
            {
                yield return null;
            }
            
            yield return new WaitForSeconds(passTurnDelay);

            if(CurrentUnit != null)
            {
                if (!CurrentUnit.IsHostile)
                {
                    PathfindingManager.Instance.playerTilesInRange.Clear();
                    yield return CombatManager.Instance.StartCoroutine(CombatManager.Instance.ChooseFacingDirection(CurrentUnit));
                }
            }

            yield return new WaitForSeconds(0.5f);

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

        public List<Entity> FindAllHeros()
        {
            //check if any hero exist
            totalHeroList.Clear();
            totalUnitList = FindObjectsOfType<Entity>().ToList();

            //filter by heroes
            for (int i = totalUnitList.Count - 1; i >= 0; i--)
            {
                if (!totalUnitList[i].IsHostile && !totalUnitList[i].IsProp)
                {
                    totalHeroList.Add(totalUnitList[i]);
                }
            }

            if (totalHeroList.Count > 0)
            {
                return totalUnitList;
            }
            else
            {
                return new List<Entity>();
            }
        }

        public List<Entity> FindAllEnemies()
        {
            //check if any hero exist
            totalEnemiesList.Clear();
            totalUnitList = FindObjectsOfType<Entity>().ToList();

            //filter by heroes

            for (int i = totalUnitList.Count - 1; i >= 0; i--)
            {
                if (totalUnitList[i].IsHostile && !totalUnitList[i].IsProp)
                {
                    totalEnemiesList.Add(totalUnitList[i]);
                }
            }

            if (totalEnemiesList.Count > 0)
            {
                return totalEnemiesList;
            }
            else
            {
                return new List<Entity>();
            }
        }

        private bool AnyUnitHasPendingPopups()
        {
            for (int i = CurrentUnitQueue.Count - 1; i >= 0; i--)
            {
                if(CurrentUnitQueue.ToArray()[i] == null)
                    continue;

                if (CurrentUnitQueue.ToArray()[i].PopupTextQueue.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Turn Order Calculations
        public void CalculateTurnOrder()
        {
            turnOrderList = new List<Entity>(FindObjectsOfType<Entity>());
            
            for(int i = turnOrderList.Count - 1; i >= 0; i--)
            {
                if (turnOrderList[i].IsProp)
                {
                    turnOrderList.RemoveAt(i);
                }
            }
            
            turnOrderList.Sort(CompareSpeed); //sorts in ascending order
            turnOrderList.Reverse();

            CurrentUnitQueue.Clear();

            for(int i = 0; i < turnOrderList.Count; i++)
            {
                CurrentUnitQueue.Enqueue(turnOrderList[i]);
                turnOrderList[i].OnDestroyedEvent += OnUnitDestroy;
                turnOrderList[i].OnAddBuffEvent += GameUIManager.Instance.UpdateStatusEffectUI;
            }
        }

        private void OnUnitDestroy(Entity destroyedUnit)
        {
            if(destroyedUnit == currentUnit)
            {
                CurrentUnitQueue = new Queue<Entity>(CurrentUnitQueue.Where(x => x != destroyedUnit));
                StartCoroutine(PassTurn());
            }
            else
            {
                CurrentUnitQueue = new Queue<Entity>(CurrentUnitQueue.Where(x => x != destroyedUnit));
            }

            //unsub the destroyed unit's events
            destroyedUnit.OnDestroyedEvent -= OnUnitDestroy;
            destroyedUnit.OnAddBuffEvent -= GameUIManager.Instance.UpdateStatusEffectUI;


            if (GameUIManager.Instance != null) 
            {
                if(GameUIManager.Instance.turnOrderSpritePool.Count > 0)
                {
                    GameUIManager.Instance.UpdateTurnOrderUI();
                }
            }

            Destroy(destroyedUnit.gameObject);
        }

        //delegate for sort()
        int CompareSpeed(Entity _a, Entity _b)
        {
            if (_a.stats.Speed == _b.stats.Speed)
            {
                int rand = UnityEngine.Random.Range(0, 2);

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

        public IEnumerator TutorialSteps()
        {
            //waiting for part 1 to be completed
            yield return new WaitUntil(() => tutorialPart == TutorialPart.Part2);

            #region Part 2
            //reset for part 2
            TutorialUIManager.Instance.currentTutorialGuideCap = 3;
            ResetStage(TutorialPart.Part2);
            #endregion

            yield return new WaitUntil(() => tutorialPart == TutorialPart.Part3);

            #region Part 3
            //reset for part 3
            TutorialUIManager.Instance.currentTutorialGuideCap = 4;

            //in part 3, find bounty hunter and add buffs
            for (int i = 0; i < CurrentUnitQueue.Count; i++)
            {
                if (CurrentUnitQueue.ToList()[i].GetComponent<BountyHunter>())
                {
                    CurrentUnitQueue.ToList()[i].AddBuff(GetStatusEffect.CreateModifier(STATUS_EFFECT.STEALTH_TOKEN, 1, 1));
                    CurrentUnitQueue.ToList()[i].AddBuff(GetStatusEffect.CreateModifier(STATUS_EFFECT.STRENGTH_TOKEN, 1, 1));
                    break;
                }
            }
            #endregion

            yield return new WaitUntil(() => tutorialPart == TutorialPart.Part4);

            #region Part 4
            //reset for part 4
            TutorialUIManager.Instance.currentTutorialGuideCap = 5;
            ResetStage(TutorialPart.Part4);
            #endregion

            yield return new WaitUntil(() => tutorialPart == TutorialPart.COMPLETED);

            #region Load Level 1
            OverlayTileManager.Instance.tileMapList.Last().gameObject.SetActive(false);
            LevelManager.Instance.LoadScene((int)SCENEINDEX.GAME_SCENE);
            #endregion
        }

        //mostly used for tutorial
        public void ResetStage(TutorialPart part)
        {
            OverlayTileManager.Instance.InitOverlayTiles(OverlayTileManager.Instance.tileMapList[(int)part]);
            CombatManager.Instance.OnBattleStart();

            //reset
            runOnce = false;
            cycleCount = 1;
            ChangePhase(planPhase);
        }
    }

   

    public enum TutorialPart
    {
        Part1 = 0,
        Part2 = 1,
        Part3 = 2,
        Part4 = 3,
        COMPLETED = 4,
    }
}
