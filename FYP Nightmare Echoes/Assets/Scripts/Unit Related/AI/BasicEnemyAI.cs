using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NightmareEchoes.Grid;
using NightmareEchoes.Unit.Pathfinding;
using UnityEngine.Tilemaps;
using System.Linq;
using NightmareEchoes.Inputs;
using NightmareEchoes.Unit.Combat;
using System;

//by Terrence, editted by alex
namespace NightmareEchoes.Unit.AI
{
    public class BasicEnemyAI : MonoBehaviour
    {
        OverlayTileManager overlayTileManager;

        [Header("Hero List + Unit List")]
        public List<Entity> totalHeroList, totalUnitList;

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
        public List<OverlayTile> pathWithoutProps = new List<OverlayTile>();
        public List<OverlayTile> pathWithProps = new List<OverlayTile>();
        public OverlayTile frontTileTemp;

        //checking if this unit can attk, move & attk and has attacked.
        public bool inAtkRange, inMoveAndAttackRange,detectedStealthHero;
        public int selectedAttackRange;
        int selectedAttackMinRange;
        int rngHelper;
        float currTileUtil, highestTileUtil;

        //this unit's specific variables
        List<OverlayTile> tilesInRangeWithoutProps = new List<OverlayTile>();
        List<OverlayTile> tilesInRangeWithProps = new List<OverlayTile>();
        public List<OverlayTile> tilesInSightWithoutProps = new List<OverlayTile>();
        public List<OverlayTile> tilesInSightWithProps = new List<OverlayTile>();
        List<OverlayTile> possibleAttackLocations = new List<OverlayTile>();
        Skill currSelectedSkill;
        int skillAmount;

        public OverlayTile thisUnitTile, tileToAttack, aoeTargetTile;
        OverlayTile targetTileToMove;
        OverlayTile bestMoveTile;

        Dictionary<Entity, int> distancesDictionary = new Dictionary<Entity, int>();
        Dictionary<string, float> utilityDictionary = new Dictionary<string, float>();
        Dictionary<Entity, float> aggroDictionary = new Dictionary<Entity, float>();

        float healthPercent;

        #region Class Attributes
        public List<OverlayTile> TilesInRange
        {
            get => tilesInRangeWithoutProps;
            set => tilesInRangeWithoutProps = value;
        }

        public List<Entity> TotalHeroList
        {
            get => totalHeroList;
            set => totalHeroList = value;
        }

        #endregion


        private void Awake()
        {
            thisUnit = GetComponent<Entity>();

        }

        private void Start()
        {
            overlayTileManager = OverlayTileManager.Instance;
        }

        //Main Action
        public void MakeDecision(Entity thisUnit)
        {
            //reset values
            thisUnit.HasAttacked = false;
            thisUnit.HasMoved = false;
            detectedStealthHero = false;
            bestPath = new List<OverlayTile>();

            //sort heros by distance and find tiles in range
            SortHeroesByDistance(thisUnit);

            if (totalHeroList.Count > 0)
            {
                thisUnitTile = thisUnit.ActiveTile;
                tilesInRangeWithoutProps = Pathfinding.Pathfinding.FindTilesInRange(thisUnitTile, thisUnit.stats.MoveRange, ignoreProps: false);
                tilesInRangeWithProps = Pathfinding.Pathfinding.FindTilesInRange(thisUnitTile, thisUnit.stats.MoveRange, ignoreProps: true);

                //tilesInSightWithoutProps = Pathfinding.Pathfinding.FindTilesInRange(thisUnitTile, (TileMapManager.Instance.length + TileMapManager.Instance.width), includeProps: false);
                //tilesInSightWithProps = Pathfinding.Pathfinding.FindTilesInRange(thisUnitTile, (TileMapManager.Instance.length + TileMapManager.Instance.width), includeProps: true);

                tilesInSightWithoutProps = Pathfinding.Pathfinding.FindTilesInRange(thisUnitTile, FindDistanceBetweenTile(thisUnitTile, closestHero.ActiveTile) + 2, ignoreProps: false);
                tilesInSightWithProps = Pathfinding.Pathfinding.FindTilesInRange(thisUnitTile, FindDistanceBetweenTile(thisUnitTile, closestHero.ActiveTile) + 2, ignoreProps: true);

                PathfindingManager.Instance.ShowTilesInRange(tilesInRangeWithoutProps);

                /*foreach(var tile in tilesInSightWithProps)
                {
                    tile.ShowCustomColor(Color.blue);
                }*/

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
            skillAmount = 0;

            if (thisUnit.Skill1Skill != null)
            {
                skillAmount++;
            }
            if (thisUnit.Skill2Skill != null)
            {
                skillAmount++;
            }
            if (thisUnit.Skill3Skill != null)
            {
                skillAmount++;
            }

            switch (UnityEngine.Random.Range(0, skillAmount))
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
                    break;
            }

            //setting the values based on the closest hero
            targetHero = closestHero;
            rangeToTarget = rangeToClosestHero;
            tileToAttack = targetHero.ActiveTile;
            rngHelper = 1;


            #region Deciding To attack through obstacles or not
            //setting the different end points around the target.
            List<List<OverlayTile>> pathOptions = new List<List<OverlayTile>>();

            Vector2Int[] directions = {
                new Vector2Int(1, 0),  // Right
                new Vector2Int(0, 1),  // Up
                new Vector2Int(-1, 0), // Left
                new Vector2Int(0, -1)  // Down
            };

            for (int i = 0; i < directions.Length; i++)
            {
                Vector2Int targetLocation = new Vector2Int(tileToAttack.gridLocation.x, tileToAttack.gridLocation.y) + directions[i];
                targetTileToMove = overlayTileManager.GetOverlayTile(targetLocation);

                // Check if the target location is valid (not blocked by entities or obstacles)
                if (targetTileToMove != null && !targetTileToMove.CheckEntityGameObjectOnTile() && !targetTileToMove.CheckObstacleOnTile())
                {
                    List<OverlayTile> path = Pathfinding.Pathfinding.FindPath(thisUnitTile, targetTileToMove, tilesInSightWithProps);

                    if (path.Count > 0)
                    {
                        pathOptions.Add(path);
                    }

                }
            }

            #region Assigning best path
            for (int i = 0; i < pathOptions.Count; i++)
            {
                if (bestPath.Count == 0 || bestPath.Count > pathOptions[i].Count)
                {
                    bestPath = pathOptions[i];

                    if (bestPath.Count > 0)
                    {
                        bestMoveTile = bestPath.Last();
                    }
                }
            }

            if (bestPath.Count > 0)
            {
                pathWithoutProps = Pathfinding.Pathfinding.FindPath(thisUnitTile, bestMoveTile, tilesInSightWithoutProps);
                pathWithProps = Pathfinding.Pathfinding.FindPath(thisUnitTile, bestMoveTile, tilesInSightWithProps);

                #endregion

                int withoutPropsUtil = pathWithoutProps.Count();
                int withPropsUtil = 0;

                int wastedMoveCounter = thisUnit.stats.MoveRange;

                for (int i = 0; i < pathWithProps.Count; i++)
                {
                    var checkEntity = pathWithProps[i].CheckEntityGameObjectOnTile()?.GetComponent<Entity>();

                    if (!checkEntity)
                    {
                        withPropsUtil++;
                        wastedMoveCounter--;
                        if (wastedMoveCounter <= 0)
                        {
                            wastedMoveCounter = thisUnit.stats.MoveRange;
                        }
                        continue;
                    }

                    if (checkEntity.IsProp)
                    {
                        withPropsUtil += wastedMoveCounter;
                        wastedMoveCounter = thisUnit.stats.MoveRange - 1;
                    }
                }

                if (withPropsUtil < withoutPropsUtil || withoutPropsUtil == 0)
                {
                    //if there is no obstacles
                    totalPathList = thisUnit.stats.MoveRange >= pathWithProps.Count ?
                        totalPathList = Pathfinding.Pathfinding.FindPath(thisUnitTile, pathWithProps[pathWithProps.Count - 1], tilesInRangeWithProps) :
                        totalPathList = Pathfinding.Pathfinding.FindPath(thisUnitTile, pathWithProps[thisUnit.stats.MoveRange - 1], tilesInRangeWithProps);

                    for (int i = 0; i < thisUnit.stats.MoveRange + currSelectedSkill.Range; i++)
                    {
                        if (i >= pathWithProps.Count)
                        {
                            break;
                        }

                        var checkEntity = pathWithProps[i].CheckEntityGameObjectOnTile()?.GetComponent<Entity>();

                        if (!checkEntity)
                        {
                            continue;
                        }

                        if (checkEntity.IsProp)
                        {
                            if (i == 0)
                            {
                                totalPathList.Clear();
                                tileToAttack = pathWithProps[i];
                                targetHero = checkEntity;
                                break;
                            }
                            else
                            {
                                totalPathList = Pathfinding.Pathfinding.FindPath(thisUnitTile, pathWithProps[i - 1], tilesInRangeWithProps);
                                tileToAttack = pathWithProps[i];
                                targetHero = checkEntity;
                                break;
                            }
                        }
                    }
                }
                else if (withPropsUtil > withoutPropsUtil)
                {
                    if (pathWithoutProps.Count > 0)
                    {
                        totalPathList = thisUnit.stats.MoveRange >= pathWithProps.Count ?
                        totalPathList = Pathfinding.Pathfinding.FindPath(thisUnitTile, pathWithProps[pathWithoutProps.Count - 1], tilesInRangeWithoutProps) :
                        totalPathList = Pathfinding.Pathfinding.FindPath(thisUnitTile, pathWithoutProps[thisUnit.stats.MoveRange - 1], tilesInRangeWithoutProps);
                    }
                }
            }
             
            #endregion

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
            FindBestMoveTile(possibleAttackLocations);
        }
        
        void IfInMoveAtkRange()
        {
            FindBestMoveTile(possibleAttackLocations);
        }

        void IfOutMoveAtkRange()
        {
            if (currSelectedSkill != thisUnit.BasicAttackSkill)
            {
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

            bestMoveTile = thisUnitTile;
            int shortestPathLength = 0;
            bool runOnce = false;

            for (int i = 0; i < tilesInRangeWithoutProps.Count; i++)
            {
                if (!tilesInRangeWithoutProps[i].CheckEntityGameObjectOnTile())
                {
                    //set distance between tile without prop and the target tile to move.
                    var distanceBetweenTileAndTarget = FindDistanceBetweenTile(targetTileToMove, tilesInRangeWithoutProps[i]) + thisUnit.stats.MoveRange;
                    var tilesInRangeFromTileWithoutEntity = new List<OverlayTile>(Pathfinding.Pathfinding.FindTilesInRange(tilesInRangeWithoutProps[i], distanceBetweenTileAndTarget, ignoreProps: false));
                    var pathFromPossibleTile = new List<OverlayTile>(Pathfinding.Pathfinding.FindPath(tilesInRangeWithoutProps[i], targetTileToMove, tilesInRangeFromTileWithoutEntity));

                    if(pathFromPossibleTile.Count == 0)
                    {
                        continue;
                    }
                    
                    if(!runOnce)
                    {
                        shortestPathLength = pathFromPossibleTile.Count;
                        bestMoveTile = tilesInRangeWithoutProps[i];
                        runOnce = true;
                    }

                    if (pathFromPossibleTile.Count < shortestPathLength)
                    {
                        shortestPathLength = pathFromPossibleTile.Count;
                        bestMoveTile = tilesInRangeWithoutProps[i];
                    }
                }
            }

            if (bestMoveTile == thisUnitTile)
            {
                //Defaulted
                for (int i = 0; i < tilesInRangeWithoutProps.Count; i++)
                {
                    if (!tilesInRangeWithoutProps[i].CheckEntityGameObjectOnTile())
                    {
                        if (FindDistanceBetweenTile(targetTileToMove, tilesInRangeWithoutProps[i]) < FindDistanceBetweenTile(targetTileToMove, bestMoveTile))
                        {
                            bestMoveTile = tilesInRangeWithoutProps[i];
                        }
                        else if (FindDistanceBetweenTile(targetTileToMove, tilesInRangeWithoutProps[i]) == FindDistanceBetweenTile(targetTileToMove, bestMoveTile))
                        {
                            rngHelper++;
                            if (UnityEngine.Random.Range(0.0f, 1.0f) < (1.0f / rngHelper))
                            {
                                bestMoveTile = tilesInRangeWithoutProps[i];
                            }
                        }
                    }
                }

                totalPathList = Pathfinding.Pathfinding.FindPath(thisUnitTile, bestMoveTile, tilesInRangeWithoutProps);

                /*frontTileTemp = returnFrontTile(thisUnitTile, thisUnit.Direction);
                if (frontTileTemp.CheckEntityGameObjectOnTile())
                {
                    if (frontTileTemp.CheckEntityGameObjectOnTile().GetComponent<Entity>().IsProp)
                    {
                        tileToAttack = frontTileTemp;
                        inAtkRange = true;
                    }
                }
                frontTileTemp = null;*/
            }
            else
            {
                totalPathList = Pathfinding.Pathfinding.FindPath(thisUnitTile, bestMoveTile, tilesInRangeWithoutProps);

                /*frontTileTemp = returnFrontTile(thisUnitTile, thisUnit.Direction);
                if (frontTileTemp.CheckEntityGameObjectOnTile())
                {
                    if (frontTileTemp.CheckEntityGameObjectOnTile().GetComponent<Entity>().IsProp)
                    {
                        tileToAttack = frontTileTemp;
                        inAtkRange = true;
                    }
                }
                frontTileTemp = null;*/
            }
        }

        void IfTargetTypeAOE()
        {
            FindBestMoveTile(tilesInRangeWithoutProps);

            if (bestMoveTile == thisUnitTile)
            {
                totalPathList.Clear();
                SetAOETargetTile(thisUnitTile);
            }
            else
            {
                totalPathList = Pathfinding.Pathfinding.FindPath(thisUnitTile, bestMoveTile, tilesInRangeWithoutProps);
                SetAOETargetTile(bestMoveTile);
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
                    if (IsTileAttackableFromCross(thisUnitTile, tileToAttack))
                    {
                        inAtkRange = true;
                    }
                    else
                    {
                        for (int i = 0; i < tilesInRangeWithoutProps.Count; i++)
                        {
                            if (IsTileAttackableFromCross(tilesInRangeWithoutProps[i], tileToAttack))
                            {
                                if (!tilesInRangeWithoutProps[i].CheckEntityGameObjectOnTile())
                                {
                                    possibleAttackLocations.Add(tilesInRangeWithoutProps[i]);
                                    inMoveAndAttackRange = true;
                                }
                            }
                        }
                    }
                    
                    break;
                case TargetArea.Diamond:
                    if (IsTileAttackableFromDiamond(thisUnitTile, tileToAttack))
                    {
                        inAtkRange = true;
                    }
                    else
                    {
                        for (int i = 0; i < tilesInRangeWithoutProps.Count; i++)
                        {
                            if (IsTileAttackableFromDiamond(tilesInRangeWithoutProps[i], tileToAttack))
                            {
                                if (!tilesInRangeWithoutProps[i].CheckEntityGameObjectOnTile())
                                {
                                    possibleAttackLocations.Add(tilesInRangeWithoutProps[i]);
                                    inMoveAndAttackRange = true;
                                }
                            }
                        }
                    }
                    
                    break;

                default:
                    if (IsTileAttackableFromDiamond(thisUnitTile, tileToAttack))
                    {
                        inAtkRange = true;
                    }
                    else
                    {
                        for (int i = 0; i < tilesInRangeWithoutProps.Count; i++)
                        {
                            if (IsTileAttackableFromDiamond(tilesInRangeWithoutProps[i], tileToAttack))
                            {
                                if (!tilesInRangeWithoutProps[i].CheckEntityGameObjectOnTile())
                                {
                                    possibleAttackLocations.Add(tilesInRangeWithoutProps[i]);
                                    inMoveAndAttackRange = true;
                                }
                            }
                        }
                    }
                    break;
            }
            #endregion
        }



        #region Public Calls for Enemy Phase
        public void MoveProcess(Entity thisUnit)
        {
            if (totalPathList.Count > 0)
            {
                //render arrow, pan camera
                PathfindingManager.Instance.RenderArrow(tilesInRangeWithoutProps, totalPathList, thisUnit);
                CameraControl.Instance.UpdateCameraPan(thisUnit.gameObject);

                //only if you havent detected hero
                if (!detectedStealthHero)
                {
                    thisUnit.HasMoved = true;
                    PathfindingManager.Instance.MoveAlongPath(thisUnit, totalPathList, tilesInRangeWithoutProps);
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
                    switch (UnityEngine.Random.Range(0, targets.Count))
                    {
                        case 0:
                            tileToAttack = targets[0].ActiveTile;
                            break;

                        case 1:
                            tileToAttack = targets[1].ActiveTile;
                            break;

                        case 2:
                            tileToAttack = targets[2].ActiveTile;
                            break;
                    }

                    List<OverlayTile> tilesAroundTarget = new List<OverlayTile>(Pathfinding.Pathfinding.FindTilesInRange(tileToAttack, 1, ignoreProps:false));
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
                            switch(UnityEngine.Random.Range(0, possibleRedirectTiles.Count))
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
                tileToAttack.CheckEntityGameObjectOnTile()?.GetComponent<Entity>().UpdateTokenLifeTime(STATUS_EFFECT.STEALTH_TOKEN);

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
                        AttackProcess(thisUnit, tileToAttack);
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

            AttackProcess(thisUnit, tileToAttack);
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
                tileToAttack.HideTile();
                totalPathList.Clear();
            }
            else
            {
                CombatManager.Instance.EnemyTargetUnit(tileToAttack.CheckEntityGameObjectOnTile().GetComponent<Entity>(), currSelectedSkill);
                tileToAttack.HideTile();
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

        void UpdateHeroList()
        {
            //populating unitList
            totalHeroList.Clear();
            totalUnitList = FindObjectsOfType<Entity>().ToList();

            //filter by heroes
            for(int i = 0; i < totalUnitList.Count; i++)
            {
                if (!totalUnitList[i].IsHostile && !totalUnitList[i].StealthToken && !totalUnitList[i].IsProp)
                {
                    totalHeroList.Add(totalUnitList[i]);
                }
            }
        }

        void FindBestMoveTile(List<OverlayTile> tileList)
        {
            if (tileList.Count > 0)
            {
                bestMoveTile = tileList[0];
            }
            rngHelper = 1;

            for (int i = 0; i < tileList.Count; i++)
            {
                currTileUtil = FindDistanceBetweenTile(tileToAttack, tileList[i]);
                switch (targetHero.Direction)
                {
                    case Direction.NORTH:
                        if ((tileList[i].gridLocation.x < tileToAttack.gridLocation.x) && (tileList[i].gridLocation.y == tileToAttack.gridLocation.y))
                        {
                            currTileUtil = currTileUtil + 20;
                        }
                        break;
                    case Direction.SOUTH:
                        if ((tileList[i].gridLocation.x > tileToAttack.gridLocation.x) && (tileList[i].gridLocation.y == tileToAttack.gridLocation.y))
                        {
                            currTileUtil = currTileUtil + 20;
                        }
                        break;
                    case Direction.EAST:
                        if ((tileList[i].gridLocation.x == tileToAttack.gridLocation.x) && (tileList[i].gridLocation.y < tileToAttack.gridLocation.y))
                        {
                            currTileUtil = currTileUtil + 20;
                        }
                        break;
                    case Direction.WEST:
                        if ((tileList[i].gridLocation.x == tileToAttack.gridLocation.x) && (tileList[i].gridLocation.y > tileToAttack.gridLocation.y))
                        {
                            currTileUtil = currTileUtil + 20;
                        }
                        break;
                }

                if (!tileList[i].CheckEntityGameObjectOnTile())
                {
                    if (currTileUtil > highestTileUtil)
                    {
                        bestMoveTile = tileList[i];
                        highestTileUtil = currTileUtil;
                    }
                    else if (currTileUtil == highestTileUtil)
                    {
                        rngHelper++;
                        if (UnityEngine.Random.Range(0.0f, 1.0f) < (1.0f / rngHelper))
                        {
                            bestMoveTile = tileList[i];
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
                totalPathList = Pathfinding.Pathfinding.FindPath(thisUnitTile, bestMoveTile, tilesInRangeWithoutProps);
            }
        }

        void SetAOETargetTile(OverlayTile sourceTile)
        {
            float xDist = sourceTile.gridLocation.x - sourceTile.gridLocation.x;
            float yDist = sourceTile.gridLocation.y - sourceTile.gridLocation.y;

            if (Mathf.Abs(xDist) > Mathf.Abs(yDist))
            {
                if (xDist < 0)
                {
                    //north
                    aoeTargetTile = OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(sourceTile.gridLocation.x + 1, sourceTile.gridLocation.y));
                }
                else
                {
                    //south
                    aoeTargetTile = OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(sourceTile.gridLocation.x + 1, sourceTile.gridLocation.y));
                }
            }
            else
            {
                if (yDist > 0)
                {
                    //east
                    aoeTargetTile = OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(sourceTile.gridLocation.x, sourceTile.gridLocation.y - 1));
                }
                else
                {
                    //west
                    aoeTargetTile = OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(sourceTile.gridLocation.x, sourceTile.gridLocation.y + 1));
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

        public int FindDistanceBetweenTile(OverlayTile target1, OverlayTile target2)
        {
            int dist;
            Vector3Int t1v, t2v;

            t1v = target1.gridLocation;
            t2v = target2.gridLocation;

            dist = (Mathf.Abs((t1v.x) - (t2v.x)) + Mathf.Abs((t1v.y) - (t2v.y)));

            return dist;
        }

        void AdjustTileUtilityBasedOnDirection(OverlayTile tile)
        {
            switch (targetHero.Direction)
            {
                case Direction.NORTH:
                    if (tile.gridLocation.x < tileToAttack.gridLocation.x && tile.gridLocation.y == tileToAttack.gridLocation.y)
                    {
                        currTileUtil += 20;
                    }
                    break;
                case Direction.SOUTH:
                    if (tile.gridLocation.x > tileToAttack.gridLocation.x && tile.gridLocation.y == tileToAttack.gridLocation.y)
                    {
                        currTileUtil += 20;
                    }
                    break;
                case Direction.EAST:
                    if (tile.gridLocation.x == tileToAttack.gridLocation.x && tile.gridLocation.y < tileToAttack.gridLocation.y)
                    {
                        currTileUtil += 20;
                    }
                    break;
                case Direction.WEST:
                    if (tile.gridLocation.x == tileToAttack.gridLocation.x && tile.gridLocation.y > tileToAttack.gridLocation.y)
                    {
                        currTileUtil += 20;
                    }
                    break;
            }
        }

        bool IsTileAttackableFromDiamond(OverlayTile target1, OverlayTile target2)
        {
            rangeToTarget = Mathf.Abs(target1.gridLocation.x - target2.gridLocation.x) + Mathf.Abs(target1.gridLocation.y - target2.gridLocation.y);
            return rangeToTarget <= selectedAttackRange && rangeToTarget >= selectedAttackMinRange;
        }

        OverlayTile returnFrontTile(OverlayTile checkFrom, Direction facing)
        {
            OverlayTile frontTile;
            switch (facing)
            {
                case Direction.NORTH:
                    frontTile = OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(checkFrom.gridLocation.x + 1, checkFrom.gridLocation.y));
                    break;

                case Direction.SOUTH:
                    frontTile = OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(checkFrom.gridLocation.x - 1, checkFrom.gridLocation.y));
                    break;

                case Direction.EAST:
                    frontTile = OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(checkFrom.gridLocation.x, checkFrom.gridLocation.y + 1));
                    break;

                case Direction.WEST:
                    frontTile = OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(checkFrom.gridLocation.x, checkFrom.gridLocation.y - 1));
                    break;

                default: //treat as if facing north
                    frontTile = OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(checkFrom.gridLocation.x + 1, checkFrom.gridLocation.y));
                    break;
            }
            return frontTile;
        }

        bool IsTileAttackableFromCross(OverlayTile target1, OverlayTile target2)
        {
            if (target1.gridLocation.x == target2.gridLocation.x)
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
