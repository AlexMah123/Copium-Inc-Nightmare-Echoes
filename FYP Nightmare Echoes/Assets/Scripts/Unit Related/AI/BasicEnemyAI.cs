using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NightmareEchoes.Grid;
using NightmareEchoes.Unit.Pathfinding;
using UnityEngine.Tilemaps;
using System.Linq;
using NightmareEchoes.Inputs;
using NightmareEchoes.Unit.Combat;

//by Terrence, editted by alex
namespace NightmareEchoes.Unit.AI
{
    public class BasicEnemyAI : MonoBehaviour
    {
        [Header("Hero List + Unit List")]
        public List<Entity> totalHeroList, totalUnitList;
        [SerializeField] List<Entity> totalPropList;

        [Space(20), Header("Path List to hero")]
        public List<OverlayTile> totalPathList = new List<OverlayTile>();

        [Space(20), Header("Enemy Specifics")]
        public float attackDelay = 1f;
        public Entity thisUnit;

        //used for decision making
        public Entity targetHero, closestHero;
        float rangeToTarget, rangeToClosestHero;
        List<OverlayTile> pathA = new List<OverlayTile>();
        List<OverlayTile> pathB = new List<OverlayTile>();
        List<OverlayTile> pathC = new List<OverlayTile>();
        List<OverlayTile> pathD = new List<OverlayTile>();
        List<OverlayTile> bestPath = new List<OverlayTile>();
        public List<OverlayTile> pathThinker = new List<OverlayTile>();
        public List<OverlayTile> pathThinkerPhase = new List<OverlayTile>();
        bool phaseThinking = false;

        //checking if this unit can attk, move & attk and has attacked.
        public bool inAtkRange, inMoveAndAttackRange, detectedStealthHero;
        public int selectedAttackRange;
        int selectedAttackMinRange;
        int rngHelper;
        float currTileUtil, highestTileUtil;

        //this unit's specific variables
        List<OverlayTile> tilesInRange = new List<OverlayTile>();
        List<OverlayTile> tilesInRangePhasing = new List<OverlayTile>();
        List<OverlayTile> tilesInSight = new List<OverlayTile>();
        List<OverlayTile> tilesInSightPhasing = new List<OverlayTile>();
        List<OverlayTile> possibleAttackLocations = new List<OverlayTile>();
        Skill currSelectedSkill;
        int skillAmount;

        public OverlayTile thisUnitTile, targetTileToMove, aoeTargetTile;
        OverlayTile bestMoveTile;

        Dictionary<Entity, int> distancesDictionary = new Dictionary<Entity, int>();
        Dictionary<string, float> utilityDictionary = new Dictionary<string, float>();
        Dictionary<Entity, float> aggroDictionary = new Dictionary<Entity, float>();

        float healthPercent;

        #region Class Attributes
        public List<OverlayTile> TilesInRange
        {
            get => tilesInRange;
            set => tilesInRange = value;
        }

        public List<Entity> TotalHeroList
        {
            get => totalHeroList;
            set => totalHeroList = value;
        }

        public List<Entity> TotalPropList
        {
            get => totalPropList;
            set => totalPropList = value;
        }
        #endregion


        private void Awake()
        {
            thisUnit = GetComponent<Entity>();
        }

        //Main Action
        public void MakeDecision(Entity thisUnit)
        {
            //reset values
            thisUnit.HasAttacked = false;
            thisUnit.HasMoved = false;
            detectedStealthHero = false;

            //sort heros by distance and find tiles in range
            SortHeroesByDistance(thisUnit);
            //AddHeroesAndObstacles();

            if (totalHeroList.Count > 0)
            {
                thisUnitTile = thisUnit.ActiveTile;

                tilesInRange = Pathfinding.Pathfinding.FindTilesInRange(thisUnitTile, thisUnit.stats.MoveRange, includeProps: false);
                tilesInRangePhasing = Pathfinding.Pathfinding.FindTilesInRange(thisUnitTile, thisUnit.stats.MoveRange, includeProps: true);
                tilesInSight = Pathfinding.Pathfinding.FindTilesInRange(thisUnitTile, 99, includeProps: false);
                tilesInSightPhasing = Pathfinding.Pathfinding.FindTilesInRange(thisUnitTile, 99, includeProps: true);

                PathfindingManager.Instance.ShowTilesInRange(tilesInRange);

                healthPercent = 100 * thisUnit.stats.Health / thisUnit.stats.MaxHealth;
                //Debug.Log(healthPercent);

                //weight calculations
                utilityDictionary.Clear();
                utilityDictionary.Add("Attack", healthPercent);
                //utilityDictionary.Add("Retreat", 40);

                //sort by most utility score
                var SortedOptions = utilityDictionary.OrderByDescending(utilityDictionary => utilityDictionary.Value);

                switch (SortedOptions.ToList()[0].Key)
                {
                    //example list
                    case "Attack":
                        AggressiveAction(thisUnit);
                        break;
                    case "Retreat":
                        //Debug.Log("Retreat Triggered");
                        break;
                }
            }
        }

        #region Types of Actions
        void AggressiveAction(Entity thisUnit)
        {
            //setting the amount of skills that is attached to the unit
            skillAmount = 1;

            if (thisUnit.Skill1Skill != null)
            {
                skillAmount += 1;
            }
            if (thisUnit.Skill2Skill != null)
            {
                skillAmount += 2;
            }
            if (thisUnit.Skill3Skill != null)
            {
                skillAmount += 3;
            }

            //randomising between which skill to use
            switch (Random.Range(0, skillAmount))
            {
                case 1:
                    currSelectedSkill = thisUnit.Skill1Skill;
                    break;
                case 2:
                    currSelectedSkill = thisUnit.Skill2Skill;
                    break;
                case 3:
                    currSelectedSkill = thisUnit.Skill3Skill;
                    break;
                default:
                    currSelectedSkill = thisUnit.BasicAttackSkill;
                    //currSelectedSkill = thisUnit.Skill1Skill;
                    //uncomment above to force unit to try Skill1 first 
                    break;
            }

            //setting the values based on the closest hero
            
            targetHero = closestHero;
            rangeToTarget = rangeToClosestHero;
            targetTileToMove = targetHero.ActiveTile;

            InMoveAtkRangeCheck();

            #region Flowchart
            //resetting util values
            currTileUtil = 0;
            highestTileUtil = 0;

            if (currSelectedSkill.TargetType == TargetType.AOE)
            {
                IfTargetTypeAOE();
            }
            else if (inAtkRange)
            {
                //Debug.Log("A");
                IfInAtkRange();
            }
            else if (inMoveAndAttackRange)
            {
                //Debug.Log("B");
                IfInMoveAtkRange();
            }
            else
            {
                //Debug.Log("C");
                IfOutMoveAtkRange();
            }

            //after assigning the totalPathList, check if immobilized
            if (thisUnit.CheckImmobilize())
            {
                totalPathList.Clear();
            }
            #endregion
        }
        #endregion

        #region Destination Calculation Methods
        void IfInAtkRange()
        {
            if (possibleAttackLocations.Count > 0)
            {
                bestMoveTile = possibleAttackLocations[0];
            }
            rngHelper = 1;

            for (int i = 0; i < possibleAttackLocations.Count; i++)
            {
                currTileUtil = FindDistanceBetweenTile(targetTileToMove, possibleAttackLocations[i]);
                switch (targetHero.Direction)
                {
                    case Direction.NORTH:
                        if ((possibleAttackLocations[i].gridLocation.x < targetTileToMove.gridLocation.x) && (possibleAttackLocations[i].gridLocation.y == targetTileToMove.gridLocation.y))
                        {
                            currTileUtil = currTileUtil + 20;
                        }
                        break;
                    case Direction.SOUTH:
                        if ((possibleAttackLocations[i].gridLocation.x > targetTileToMove.gridLocation.x) && (possibleAttackLocations[i].gridLocation.y == targetTileToMove.gridLocation.y))
                        {
                            currTileUtil = currTileUtil + 20;
                        }
                        break;
                    case Direction.EAST:
                        if ((possibleAttackLocations[i].gridLocation.x == targetTileToMove.gridLocation.x) && (possibleAttackLocations[i].gridLocation.y < targetTileToMove.gridLocation.y))
                        {
                            currTileUtil = currTileUtil + 20;
                        }
                        break;
                    case Direction.WEST:
                        if ((possibleAttackLocations[i].gridLocation.x == targetTileToMove.gridLocation.x) && (possibleAttackLocations[i].gridLocation.y > targetTileToMove.gridLocation.y))
                        {
                            currTileUtil = currTileUtil + 20;
                        }
                        break;
                }

                //Debug.Log(currTileUtil);
                if (!possibleAttackLocations[i].CheckEntityGameObjectOnTile())
                {
                    if (currTileUtil > highestTileUtil)
                    {
                        bestMoveTile = possibleAttackLocations[i];
                        highestTileUtil = currTileUtil;
                    }
                    else if (currTileUtil == highestTileUtil)
                    {
                        rngHelper++;
                        if (Random.Range(0.0f, 1.0f) < (1.0f / rngHelper))
                        {
                            bestMoveTile = possibleAttackLocations[i];
                            highestTileUtil = currTileUtil;
                        }
                    }
                }
            }

            if (bestMoveTile == null)
            {
                totalPathList.Clear();
            }
            else
            {
                totalPathList = Pathfinding.Pathfinding.FindPath(thisUnitTile, bestMoveTile, tilesInRange);
            }
        }
        
        void IfInMoveAtkRange()
        {
            if (possibleAttackLocations.Count > 0)
            {
                bestMoveTile = possibleAttackLocations[0];
            }
            rngHelper = 1;

            for (int i = 0; i < possibleAttackLocations.Count; i++)
            {
                currTileUtil = FindDistanceBetweenTile(targetTileToMove, possibleAttackLocations[i]);

                switch (targetHero.Direction)
                {
                    case Direction.NORTH:
                        if ((possibleAttackLocations[i].gridLocation.x < targetTileToMove.gridLocation.x) && (possibleAttackLocations[i].gridLocation.y == targetTileToMove.gridLocation.y))
                        {
                            currTileUtil = currTileUtil + 20;
                            //Debug.Log("North MoveATK");
                        }
                        break;
                    case Direction.SOUTH:
                        if ((possibleAttackLocations[i].gridLocation.x > targetTileToMove.gridLocation.x) && (possibleAttackLocations[i].gridLocation.y == targetTileToMove.gridLocation.y))
                        {
                            currTileUtil = currTileUtil + 20;
                            //Debug.Log("South MoveATK");
                        }
                        break;
                    case Direction.EAST:
                        if ((possibleAttackLocations[i].gridLocation.x == targetTileToMove.gridLocation.x) && (possibleAttackLocations[i].gridLocation.y < targetTileToMove.gridLocation.y))
                        {
                            currTileUtil = currTileUtil + 20;
                            //Debug.Log("East MoveATK");
                        }
                        break;
                    case Direction.WEST:
                        if ((possibleAttackLocations[i].gridLocation.x == targetTileToMove.gridLocation.x) && (possibleAttackLocations[i].gridLocation.y > targetTileToMove.gridLocation.y))
                        {
                            currTileUtil = currTileUtil + 20;
                            //Debug.Log("West MoveATK");
                        }
                        break;
                }
                //Debug.Log(currTileUtil);
                if (!possibleAttackLocations[i].CheckEntityGameObjectOnTile())
                {
                    if (currTileUtil > highestTileUtil)
                    {
                        bestMoveTile = possibleAttackLocations[i];
                        highestTileUtil = currTileUtil;
                    }
                    else if (currTileUtil == highestTileUtil)
                    {
                        rngHelper++;
                        if (Random.Range(0.0f, 1.0f) < (1.0f / rngHelper))
                        {
                            bestMoveTile = possibleAttackLocations[i];
                            highestTileUtil = currTileUtil;
                        }
                    }
                }
            }

            if (bestMoveTile == null)
            {
                totalPathList.Clear();
            }
            else
            {
                totalPathList = Pathfinding.Pathfinding.FindPath(thisUnitTile, bestMoveTile, tilesInRange);
            }
        }

        void IfOutMoveAtkRange()
        {
            if (currSelectedSkill != thisUnit.BasicAttackSkill)
            {
                //reinitializing skill
                currSelectedSkill = thisUnit.BasicAttackSkill;

                InMoveAtkRangeCheck();

                if (inAtkRange)
                {
                    IfInAtkRange();
                }
                else if (inMoveAndAttackRange)
                {
                    IfInMoveAtkRange();
                }
                else
                {
                    IfOutMoveAtkRange();
                }
                return;
            }

            bestPath = null;

            pathA = Pathfinding.Pathfinding.FindPath(thisUnitTile, OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(targetTileToMove.gridLocation.x + 1, targetTileToMove.gridLocation.y)), tilesInSight);
            pathB = Pathfinding.Pathfinding.FindPath(thisUnitTile, OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(targetTileToMove.gridLocation.x, targetTileToMove.gridLocation.y + 1)), tilesInSight);
            pathC = Pathfinding.Pathfinding.FindPath(thisUnitTile, OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(targetTileToMove.gridLocation.x - 1, targetTileToMove.gridLocation.y)), tilesInSight);
            pathD = Pathfinding.Pathfinding.FindPath(thisUnitTile, OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(targetTileToMove.gridLocation.x, targetTileToMove.gridLocation.y - 1)), tilesInSight);

            if (pathA.Count != 0)
            {
                bestPath = pathA;
            }
            if (pathB.Count != 0)
            {
                if (bestPath != null)
                {
                    if (bestPath.Count > pathB.Count)
                    {
                        bestPath = pathB;
                    }
                }
                else
                {
                    bestPath = pathB;
                }
            }
            if (pathC.Count != 0)
            {
                if (bestPath != null)
                {
                    if (bestPath.Count > pathC.Count)
                    {
                        bestPath = pathC;
                    }
                }
                else
                {
                    bestPath = pathC;
                }
            }
            if (pathD.Count != 0)
            {
                if (bestPath != null)
                {
                    if (bestPath.Count > pathD.Count)
                    {
                        bestPath = pathD;
                    }
                }
                else
                {
                    bestPath = pathD;
                }
            }

            if (bestPath == null)
            {
                return;
            }
            else
            {
                if (bestPath == pathA)
                {
                    bestMoveTile = OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(targetTileToMove.gridLocation.x + 1, targetTileToMove.gridLocation.y));
                }
                else if (bestPath == pathB)
                {
                    bestMoveTile = OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(targetTileToMove.gridLocation.x, targetTileToMove.gridLocation.y + 1));
                }
                else if (bestPath == pathC)
                {
                    bestMoveTile = OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(targetTileToMove.gridLocation.x - 1, targetTileToMove.gridLocation.y));
                }
                else if (bestPath == pathD)
                {
                    bestMoveTile = OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(targetTileToMove.gridLocation.x, targetTileToMove.gridLocation.y - 1));
                }
            }

            pathThinker = Pathfinding.Pathfinding.FindPath(thisUnitTile, bestMoveTile, tilesInSight);
            pathThinkerPhase = Pathfinding.Pathfinding.FindPath(thisUnitTile, bestMoveTile, tilesInSightPhasing);
            pathThinkerPhase.Insert(0, thisUnitTile);

            int noPhaseUtil = pathThinker.Count();
            int PhaseUtil = pathThinkerPhase.Count();

            int wastedMoveCounter = thisUnit.stats.MoveRange;
            
            for (int i = 0; i < pathThinkerPhase.Count; i++)
            {
                if (pathThinkerPhase[i].CheckEntityGameObjectOnTile())
                {
                    if (pathThinkerPhase[i].CheckEntityGameObjectOnTile().GetComponent<Entity>().IsProp)
                    {
                        PhaseUtil = wastedMoveCounter + PhaseUtil;
                        wastedMoveCounter = thisUnit.stats.MoveRange - 1;
                    }
                    else
                    {
                        wastedMoveCounter--;
                        if (wastedMoveCounter <= 0)
                        {
                            wastedMoveCounter = thisUnit.stats.MoveRange;
                        }
                    }
                }
            }

            if (PhaseUtil < noPhaseUtil)
            {
                phaseThinking = true;
                for (int i = 0; i < thisUnit.stats.MoveRange - 1; i++) {
                    if (pathThinkerPhase[i].CheckEntityGameObjectOnTile())
                    {
                        if (pathThinkerPhase[i].CheckEntityGameObjectOnTile().GetComponent<Entity>().IsProp)
                        {
                            targetTileToMove = pathThinkerPhase[i];
                            if (i == 0)
                            {
                                totalPathList = Pathfinding.Pathfinding.FindPath(thisUnitTile, pathThinkerPhase[0], tilesInRange);
                                targetTileToMove = pathThinkerPhase[1];
                                inAtkRange = true;
                                break;
                            }
                            else
                            {
                                totalPathList = Pathfinding.Pathfinding.FindPath(thisUnitTile, pathThinkerPhase[i - 1], tilesInRange);
                                targetTileToMove = pathThinkerPhase[i];
                                inAtkRange = true;
                                break;
                            }
                        }
                    }
                }
                totalPathList = Pathfinding.Pathfinding.FindPath(thisUnitTile, pathThinkerPhase[thisUnit.stats.MoveRange - 1], tilesInRange);
            }
            else
            {
                phaseThinking = false;
                Debug.Log(pathThinker.Count + " vs " + (thisUnit.stats.MoveRange - 1));
                totalPathList = Pathfinding.Pathfinding.FindPath(thisUnitTile, pathThinker[thisUnit.stats.MoveRange - 1], tilesInRange);
            }
        }

        void IfTargetTypeAOE()
        {
            bestMoveTile = thisUnitTile;

            rngHelper = 1;

            for (int i = 0; i < tilesInRange.Count; i++)
            {
                if (!tilesInRange[i].CheckEntityGameObjectOnTile())
                {
                    if (FindDistanceBetweenTile(targetTileToMove, tilesInRange[i]) < FindDistanceBetweenTile(targetTileToMove, bestMoveTile))
                    {
                        bestMoveTile = tilesInRange[i];
                    }
                    else if (FindDistanceBetweenTile(targetTileToMove, tilesInRange[i]) == FindDistanceBetweenTile(targetTileToMove, bestMoveTile))
                    {
                        rngHelper++;
                        if (Random.Range(0.0f, 1.0f) < (1.0f / rngHelper))
                        {
                            bestMoveTile = tilesInRange[i];
                        }
                    }
                }
            }

            if (bestMoveTile == thisUnitTile)
            {
                totalPathList.Clear();

                float xDist = thisUnitTile.gridLocation.x - targetTileToMove.gridLocation.x;
                float yDist = thisUnitTile.gridLocation.y - targetTileToMove.gridLocation.y;

                if (Mathf.Abs(xDist) > Mathf.Abs(yDist))
                {
                    if (xDist < 0)
                    {
                        //north
                        aoeTargetTile = OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(thisUnitTile.gridLocation.x + 1, thisUnitTile.gridLocation.y));
                    }
                    else
                    {
                        //south
                        aoeTargetTile = OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(thisUnitTile.gridLocation.x + 1, thisUnitTile.gridLocation.y));
                    }
                }
                else
                {
                    if (yDist > 0)
                    {
                        //east
                        aoeTargetTile = OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(thisUnitTile.gridLocation.x, thisUnitTile.gridLocation.y - 1));
                    }
                    else
                    {
                        //west
                        aoeTargetTile = OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(thisUnitTile.gridLocation.x, thisUnitTile.gridLocation.y + 1));
                    }
                }
            }
            else
            {
                totalPathList = Pathfinding.Pathfinding.FindPath(thisUnitTile, bestMoveTile, tilesInRange);

                float xDist = bestMoveTile.gridLocation.x - targetTileToMove.gridLocation.x;
                float yDist = bestMoveTile.gridLocation.y - targetTileToMove.gridLocation.y;

                if (Mathf.Abs(xDist) > Mathf.Abs(yDist))
                {
                    if (xDist < 0)
                    {
                        //north
                        aoeTargetTile = OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(bestMoveTile.gridLocation.x + 1, bestMoveTile.gridLocation.y));
                    }
                    else
                    {
                        //south
                        aoeTargetTile = OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(bestMoveTile.gridLocation.x + 1, bestMoveTile.gridLocation.y));
                    }
                }
                else
                {
                    if (yDist > 0)
                    {
                        //east
                        aoeTargetTile = OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(bestMoveTile.gridLocation.x, bestMoveTile.gridLocation.y - 1));
                    }
                    else
                    {
                        //west
                        aoeTargetTile = OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(bestMoveTile.gridLocation.x, bestMoveTile.gridLocation.y + 1));
                    }
                }
            }
            
            
        }
        #endregion

        void InMoveAtkRangeCheck()
        {
            selectedAttackRange = currSelectedSkill.Range;
            selectedAttackMinRange = currSelectedSkill.MinRange;

            //RESETS BOOLS
            possibleAttackLocations.Clear();
            inAtkRange = false;
            inMoveAndAttackRange = false;

            #region checks if unit is inAtkRange/inMoveAndAttackRange
            switch (currSelectedSkill.TargetArea)
            {
                case TargetArea.Line:
                    if (IsTileAttackableFromCross(thisUnitTile, targetTileToMove))
                    {
                        inAtkRange = true;
                    }
                    for (int i = 0; i < tilesInRange.Count; i++)
                    {
                        if (IsTileAttackableFromCross(tilesInRange[i], targetTileToMove))
                        {
                            if (!tilesInRange[i].CheckEntityGameObjectOnTile())
                            {
                                possibleAttackLocations.Add(tilesInRange[i]);
                                inMoveAndAttackRange = true;
                            }
                        }
                    }
                    break;
                case TargetArea.Diamond:
                    if (IsTileAttackableFromDiamond(thisUnitTile, targetTileToMove))
                    {
                        inAtkRange = true;
                    }
                    for (int i = 0; i < tilesInRange.Count; i++)
                    {
                        if (IsTileAttackableFromDiamond(tilesInRange[i], targetTileToMove))
                        {
                            if (!tilesInRange[i].CheckEntityGameObjectOnTile())
                            {
                                possibleAttackLocations.Add(tilesInRange[i]);
                                inMoveAndAttackRange = true;
                            }
                        }
                    }
                    break;

                default:
                    if (IsTileAttackableFromDiamond(thisUnitTile, targetTileToMove))
                    {
                        inAtkRange = true;
                    }
                    for (int i = 0; i < tilesInRange.Count; i++)
                    {
                        if (IsTileAttackableFromDiamond(tilesInRange[i], targetTileToMove))
                        {
                            if (!tilesInRange[i].CheckEntityGameObjectOnTile())
                            {
                                possibleAttackLocations.Add(tilesInRange[i]);
                                inMoveAndAttackRange = true;
                            }
                        }
                    }
                    break;
            }
            

            //Checks tiles in range for possibleAttackableLocations if they do not have a unit on it
            
            #endregion
        }



        #region Public Calls for Enemy Phase
        public void MoveProcess(Entity thisUnit)
        {
            if (totalPathList.Count > 0)
            {
                //render arrow, pan camera
                PathfindingManager.Instance.RenderArrow(tilesInRange, totalPathList, thisUnit);
                CameraControl.Instance.UpdateCameraPan(thisUnit.gameObject);

                //only if you havent detected hero
                if (!detectedStealthHero)
                {
                    thisUnit.HasMoved = true;
                    PathfindingManager.Instance.MoveAlongPath(thisUnit, totalPathList, tilesInRange);
                }
            }

            //if you find a stealth unit in view, and you havent attack
            if (CombatManager.Instance.IsStealthUnitInViewRange(thisUnit, 1).Count > 0 && !thisUnit.HasAttacked && !detectedStealthHero)
            {
                //resetting values
                detectedStealthHero = true;
                OverlayTile redirectTile = null;
                PathfindingManager.Instance.ClearArrow(totalPathList);

                //set the targets based on the range (defaulted to 1)
                List<Entity> targets = CombatManager.Instance.IsStealthUnitInViewRange(thisUnit, 1);

                if (targets.Count > 0)
                {
                    //based on the amount, randomize the targets
                    switch (Random.Range(0, targets.Count))
                    {
                        case 0:
                            targetTileToMove = targets[0].ActiveTile;
                            break;

                        case 1:
                            targetTileToMove = targets[1].ActiveTile;
                            break;

                        case 2:
                            targetTileToMove = targets[2].ActiveTile;
                            break;
                    }

                    List<OverlayTile> tilesAroundTarget = new List<OverlayTile>(Pathfinding.Pathfinding.FindTilesInRange(targetTileToMove, 1, includeProps:false));
                    List<OverlayTile> possibleRedirectTiles = new List<OverlayTile>();

                    if(tilesAroundTarget.Count > 0) 
                    {
                        for(int i= 0; i < tilesAroundTarget.Count; i++)
                        {
                            if (!tilesAroundTarget[i].CheckEntityGameObjectOnTile() && !tilesAroundTarget[i].CheckObstacleOnTile())
                            {
                                if (FindDistanceBetweenTile(tilesAroundTarget[i], thisUnit.ActiveTile) <= 1)
                                {
                                    possibleRedirectTiles.Add(tilesAroundTarget[i]);
                                }
                            }
                        }

                        if(possibleRedirectTiles.Count > 1)
                        {
                            switch(Random.Range(0, possibleRedirectTiles.Count))
                            {
                                case 0:
                                    redirectTile = possibleRedirectTiles.First();
                                    break;
                                case 1:
                                    redirectTile = possibleRedirectTiles.Last();
                                    break;

                                default:
                                    Debug.LogWarning("Not Suppose to have 3 possible tiles");
                                    break;
                            }
                        }
                        else
                        {
                            redirectTile = possibleRedirectTiles.First();
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                thisUnit.ShowPopUpText("Detected Stealth Hero!!", Color.red);
                targetTileToMove.CheckEntityGameObjectOnTile()?.GetComponent<Entity>().UpdateTokenLifeTime(STATUS_EFFECT.STEALTH_TOKEN);

                if (thisUnit.TypeOfUnit == TypeOfUnit.MELEE_UNIT)
                {
                    if(!thisUnit.ImmobilizeToken)
                    {
                        StartCoroutine(DetectedStealthUnit(redirectTile));
                    }
                }
                else
                {
                    if (currSelectedSkill.TargetArea != TargetArea.Line)
                    {
                        AttackProcess(thisUnit, targetTileToMove);
                    }
                    else
                    {
                        StartCoroutine(DetectedStealthUnit(redirectTile));
                    }
                }
            }
        }

        public void AttackProcess(Entity thisUnit, OverlayTile targetTile)
        {
            if (targetTile.CheckEntityGameObjectOnTile()?.GetComponent<Entity>() != null)
            {
                targetTile.ShowEnemyTile();
                thisUnit.HasAttacked = true;

                StartCoroutine(AttackAction(thisUnit));
            }
        }

        public IEnumerator DetectedStealthUnit(OverlayTile redirectTile)
        {
            currSelectedSkill = thisUnit.BasicAttackSkill;

            yield return new WaitForSeconds(1f);

            StartCoroutine(PathfindingManager.Instance.MoveTowardsTile(thisUnit, redirectTile, 0.25f));
            yield return new WaitUntil(() => Vector2.Distance(thisUnit.transform.position, redirectTile.transform.position) < 0.01f);

            AttackProcess(thisUnit, targetTileToMove);
        }

        #endregion

        #region Calculations/Utility
        //delay used for attacks
        IEnumerator AttackAction(Entity thisUnit)
        {
            yield return new WaitForSeconds(attackDelay);

            if (currSelectedSkill.TargetType == TargetType.AOE)
            {
                CombatManager.Instance.EnemyTargetGround(aoeTargetTile, currSelectedSkill);
                targetTileToMove.HideTile();
                totalPathList.Clear();
            }
            else
            {
                CombatManager.Instance.EnemyTargetUnit(targetTileToMove.CheckEntityGameObjectOnTile().GetComponent<Entity>(), currSelectedSkill);
                targetTileToMove.HideTile();
                totalPathList.Clear();
            }
        }

        void SortHeroesByDistance(Entity thisUnit)
        {
            UpdateHeroList();

            if(totalHeroList.Count > 0)
            {
                //creating/updating distancesDict
                distancesDictionary.Clear();
                for (int i = 0; i < totalHeroList.Count; i++)
                {
                    int distTemp = (int)FindDistanceBetweenUnit(thisUnit, totalHeroList[i]);
                    distancesDictionary.Add(totalHeroList[i], distTemp);
                }
                var sortedHeroes = distancesDictionary.OrderBy(distancesDict => distancesDict.Value);

                closestHero = sortedHeroes.ToList()[0].Key;
                rangeToClosestHero = sortedHeroes.ToList()[0].Value;
            }
            else
            {
                distancesDictionary.Clear();
                closestHero = null;
                rangeToClosestHero = 0;
            }
        }

        void AddHeroesAndObstacles()
        {
            UpdatePropList();

            aggroDictionary.Clear();
            
            for (int i = 0; i < distancesDictionary.Count; i++)
            {
                aggroDictionary.Add(distancesDictionary.ToList()[i].Key, (distancesDictionary.ToList()[i].Value) - (thisUnit.BasicAttackSkill.Range + thisUnit.stats.MoveRange));
            }
            for (int i = 0; i < totalPropList.Count; i++)
            {
                int distTemp = (int)FindDistanceBetweenUnit(thisUnit, totalPropList[i]);
                aggroDictionary.Add(totalPropList[i], distTemp);
            }

            var sortedAggros = aggroDictionary.OrderBy(aggroDictionary => aggroDictionary.Value);

            targetHero = sortedAggros.ToList()[0].Key;
            if (!targetHero.IsHostile && !targetHero.StealthToken && !targetHero.IsProp)
            {
                //Debug.Log("hero aggro");
                rangeToTarget = sortedAggros.ToList()[0].Value + (thisUnit.BasicAttackSkill.Range + thisUnit.stats.MoveRange);
            }
            else
            {
                //Debug.Log("prop aggro");
                rangeToTarget = sortedAggros.ToList()[0].Value;
            }
        }

        void UpdateHeroList()
        {
            //populating unitList
            totalHeroList.Clear();
            totalUnitList = FindObjectsOfType<Entity>().ToList();

            //filter by heroes
            foreach (var unit in totalUnitList)
            {
                if (!unit.IsHostile && !unit.StealthToken && !unit.IsProp)
                {
                    totalHeroList.Add(unit);
                }
            }
        }

        void UpdatePropList()
        {
            totalPropList.Clear();
            totalUnitList = FindObjectsOfType<Entity>().ToList();

            foreach (var unit in totalUnitList)
            {
                if (unit.IsProp)
                {
                    totalPropList.Add(unit);
                }
            }
        }

        public float FindDistanceBetweenUnit(Entity target1, Entity target2)
        {
            float dist;
            Vector3Int t1v, t2v;

            t1v = target1.ActiveTile.gridLocation;
            t2v = target2.ActiveTile.gridLocation;

            dist = (Mathf.Abs((t1v.x)-(t2v.x)) + Mathf.Abs((t1v.y) - (t2v.y)));

            return dist;
        }

        public float FindDistanceBetweenTile(OverlayTile target1, OverlayTile target2)
        {
            float dist;
            Vector3Int t1v, t2v;

            t1v = target1.gridLocation;
            t2v = target2.gridLocation;

            dist = (Mathf.Abs((t1v.x) - (t2v.x)) + Mathf.Abs((t1v.y) - (t2v.y)));

            return dist;
        }

        bool IsTileAttackableFromDiamond(OverlayTile target1, OverlayTile target2)
        {
            rangeToTarget = Mathf.Abs(target1.gridLocation.x - target2.gridLocation.x) + Mathf.Abs(target1.gridLocation.y - target2.gridLocation.y);
            if (rangeToTarget <= selectedAttackRange)
            {
                if (rangeToTarget >= selectedAttackMinRange)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else return false;
        }
        bool IsTileAttackableFromCross(OverlayTile target1, OverlayTile target2)
        {
            if (target1.CheckEntityGameObjectOnTile())
            {
                return false;
            }
            else if (target1.gridLocation.x == target2.gridLocation.x)
            {
                //same row/x
                if ((target1.gridLocation.y + selectedAttackRange >= target2.gridLocation.y) && (target1.gridLocation.y - selectedAttackRange <= target2.gridLocation.y))
                {
                    if (selectedAttackMinRange != 0)
                    {
                        if ((target1.gridLocation.y + selectedAttackMinRange > target2.gridLocation.y) && (target1.gridLocation.y - selectedAttackMinRange < target2.gridLocation.y))
                        {
                            return false; //too close, minimum range
                        }
                        else return true; //fulfils min range
                    }
                    else return true; //no min range to check, in range
                } 
                else return false; //out of range
            }
            else if (target1.gridLocation.y == target2.gridLocation.y)
            {
                //same column/y
                if ((target1.gridLocation.x + selectedAttackRange >= target2.gridLocation.x) && (target1.gridLocation.x - selectedAttackRange <= target2.gridLocation.x))
                {
                    //within range
                    if (selectedAttackMinRange != 0)
                    {
                        if ((target1.gridLocation.x + selectedAttackMinRange > target2.gridLocation.x) && (target1.gridLocation.x - selectedAttackMinRange < target2.gridLocation.x))
                        {
                            return false; //too close, minimum range
                        }
                        else return true; //fulfils min range
                    }
                    else return true; //no min range to check, in range
                }
                else return false; //out of range
            }
            else return false; //out of cross
        }
        #endregion
        
    }

}
