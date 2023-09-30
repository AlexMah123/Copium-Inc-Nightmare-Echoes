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
        [SerializeField] List<Units> totalHeroList, totalUnitList;

        [Space(20), Header("Path List to hero")]
        public List<OverlayTile> totalPathList = new List<OverlayTile>();

        [Space(20), Header("Enemy Specifics")]
        Coroutine redirect;
        public float attackDelay = 1f;
        Units thisUnit;

        //used for decision making
        Units targetHero, closestHero;
        float rangeToTarget, rangeToClosest;

        //checking if this unit can attk, move & attk and has attacked.
        public bool inAtkRange, inMoveAndAttackRange, hasAttacked, detectedStealthHero;
        int selectedAttackRange;
        int selectedAttackMinRange;
        int rngHelper;
        float currTileUtil, highestTileUtil;

        //this unit's specific variables
        List<OverlayTile> tilesInRange = new List<OverlayTile>();
        List<OverlayTile> possibleAttackLocations = new List<OverlayTile>();
        Skill currSelectedSkill;
        int skillAmount;

        OverlayTile thisUnitTile, targetTileToMove;
        OverlayTile bestMoveTile;

        Dictionary<Units, int> distancesDictionary = new Dictionary<Units, int>();
        Dictionary<string, float> utilityDictionary = new Dictionary<string, float>();
        Dictionary<Units, float> aggroDictionary = new Dictionary<Units, float>();
        float healthPercent;

        public bool hasMoved;

        #region Class Attributes
        public List<OverlayTile> TilesInRange
        {
            get => tilesInRange;
            set => tilesInRange = value;
        }

        public List<Units> TotalHeroList
        {
            get => totalHeroList;
            set => totalHeroList = value;
        }
        #endregion


        private void Awake()
        {
            thisUnit = GetComponent<Units>();
        }

        //Main Action
        public void MakeDecision(Units thisUnit)
        {
            //reset values
            hasAttacked = false;
            hasMoved = false;
            detectedStealthHero = false;

            //sort heros by distance and find tiles in range
            SortHeroesByDistance(thisUnit);

            if (totalHeroList.Count > 0)
            {
                thisUnitTile = thisUnit.ActiveTile;

                tilesInRange = Pathfinding.Pathfinding.FindTilesInRange(thisUnitTile, thisUnit.stats.MoveRange);
                PathfindingManager.Instance.ShowTilesInRange(tilesInRange);

                healthPercent = 100 * thisUnit.stats.Health / thisUnit.stats.MaxHealth;
                //Debug.Log(healthPercent);

                //weight calculations
                utilityDictionary.Clear();
                utilityDictionary.Add("Attack", healthPercent);
                utilityDictionary.Add("Retreat", 40);

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
        void AggressiveAction(Units thisUnit)
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
                    break;
            }

            selectedAttackRange = currSelectedSkill.Range;
            selectedAttackMinRange = currSelectedSkill.MinRange;

            //setting the values based on the closest hero
            targetHero = closestHero;
            rangeToTarget = rangeToClosest;
            targetTileToMove = targetHero.ActiveTile;

            //RESETS BOOLS
            possibleAttackLocations.Clear();
            inAtkRange = false;
            inMoveAndAttackRange = false;

            #region checks if unit is inAtkRange/inMoveAndAttackRange
            if (IsTileAttackableFrom(thisUnitTile, targetTileToMove))
            {
                inAtkRange = true;
            }
          
            //Checks tiles in range for possibleAttackableLocations if they do not have a unit on it
            for (int i = 0; i < tilesInRange.Count; i++)
            {
                if (IsTileAttackableFrom(tilesInRange[i], targetTileToMove))
                {
                    if (!tilesInRange[i].CheckUnitOnTile())
                    {
                        possibleAttackLocations.Add(tilesInRange[i]);
                        inMoveAndAttackRange = true;
                    }
                }
            }
            #endregion

            #region Flowchart
            //resetting util values
            currTileUtil = 0;
            highestTileUtil = 0;

            if (inAtkRange)
            {
                bestMoveTile = possibleAttackLocations[0];
                rngHelper = 1;
                //if (thisUnit.TypeOfUnit == TypeOfUnit.RANGED_UNIT)
                //{
                    bestMoveTile = possibleAttackLocations[0];
                    
                    for (int i = 0; i < possibleAttackLocations.Count; i++)
                    {
                        currTileUtil = FindDistanceBetweenTile(targetTileToMove, possibleAttackLocations[i]);
                        switch (targetHero.Direction)
                        {
                            case Direction.NORTH:
                                if ((possibleAttackLocations[i].gridLocation.x < targetTileToMove.gridLocation.x) && (possibleAttackLocations[i].gridLocation.y == targetTileToMove.gridLocation.y))
                                {
                                    currTileUtil = currTileUtil + 20;
                                    Debug.Log("North ATK");
                                }
                                break;
                            case Direction.SOUTH:
                                if ((possibleAttackLocations[i].gridLocation.x > targetTileToMove.gridLocation.x) && (possibleAttackLocations[i].gridLocation.y == targetTileToMove.gridLocation.y))
                                {
                                    currTileUtil = currTileUtil + 20;
                                    Debug.Log("South ATK");
                                }
                                break;
                            case Direction.EAST:
                                if ((possibleAttackLocations[i].gridLocation.x == targetTileToMove.gridLocation.x) && (possibleAttackLocations[i].gridLocation.y < targetTileToMove.gridLocation.y))
                                {
                                    currTileUtil = currTileUtil + 20;
                                    Debug.Log("East ATK");
                                }
                                break;
                            case Direction.WEST:
                                if ((possibleAttackLocations[i].gridLocation.x == targetTileToMove.gridLocation.x) && (possibleAttackLocations[i].gridLocation.y > targetTileToMove.gridLocation.y))
                                {
                                    currTileUtil = currTileUtil + 20;
                                    Debug.Log("West ATK");
                                }
                                break;
                        }
                        //Debug.Log(currTileUtil);
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
                    
                    totalPathList = Pathfinding.Pathfinding.FindPath(thisUnitTile, bestMoveTile, tilesInRange);
            }
            else if (inMoveAndAttackRange)
            {
                bestMoveTile = possibleAttackLocations[0];
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
                                Debug.Log("North MoveATK");
                            }
                            break;
                        case Direction.SOUTH:
                            if ((possibleAttackLocations[i].gridLocation.x > targetTileToMove.gridLocation.x) && (possibleAttackLocations[i].gridLocation.y == targetTileToMove.gridLocation.y))
                            {
                                currTileUtil = currTileUtil + 20;
                                Debug.Log("South MoveATK");
                            }
                            break;
                        case Direction.EAST:
                            if ((possibleAttackLocations[i].gridLocation.x == targetTileToMove.gridLocation.x) && (possibleAttackLocations[i].gridLocation.y < targetTileToMove.gridLocation.y))
                            {
                                currTileUtil = currTileUtil + 20;
                                Debug.Log("East MoveATK");
                            }
                            break;
                        case Direction.WEST:
                            if ((possibleAttackLocations[i].gridLocation.x == targetTileToMove.gridLocation.x) && (possibleAttackLocations[i].gridLocation.y > targetTileToMove.gridLocation.y))
                            {
                                currTileUtil = currTileUtil + 20;
                                Debug.Log("West MoveATK");
                            }
                            break;
                    }
                    //Debug.Log(currTileUtil);
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
                totalPathList = Pathfinding.Pathfinding.FindPath(thisUnitTile, bestMoveTile, tilesInRange);
            }
            else
            {
                bestMoveTile = tilesInRange[0];
                rngHelper = 1;

                for (int i = 0; i < tilesInRange.Count; i++)
                {
                    if (!tilesInRange[i].CheckUnitOnTile())
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

                totalPathList = Pathfinding.Pathfinding.FindPath(thisUnitTile, bestMoveTile, tilesInRange);

            }

            //Debug.Log(highestTileUtil);
            #endregion
        }

        #endregion


        #region Public Calls for Enemy Phase
        public void MoveProcess(Units thisUnit)
        {
            if (totalPathList.Count > 0)
            {
                //render arrow, pan camera
                PathfindingManager.Instance.RenderArrow(tilesInRange, totalPathList, thisUnit);
                CameraControl.Instance.UpdateCameraPan(thisUnit.gameObject);

                //only if you havent detected hero
                if (!detectedStealthHero)
                {
                    hasMoved = true;
                    PathfindingManager.Instance.MoveAlongPath(thisUnit, totalPathList, tilesInRange);
                }
            }

            //if you find a stealth unit in view, and you havent attack
            if (CombatManager.Instance.IsStealthUnitInViewRange(thisUnit, 1).Count > 0 && !hasAttacked && !detectedStealthHero)
            {
                //resetting values
                detectedStealthHero = true;
                OverlayTile redirectTile = null;
                PathfindingManager.Instance.ClearArrow(totalPathList);

                //set the targets based on the range (defaulted to 1)
                List<Units> targets = CombatManager.Instance.IsStealthUnitInViewRange(thisUnit, 1);


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

                    //then based on the units direction, move infront of the target
                    switch (thisUnit.Direction)
                    {
                        case Direction.NORTH:
                            redirectTile = OverlayTileManager.Instance.GetOverlayTile((Vector2Int)(targetTileToMove.gridLocation - new Vector3Int(1, 0, 0)));
                            break;

                        case Direction.SOUTH:
                            redirectTile = OverlayTileManager.Instance.GetOverlayTile((Vector2Int)(targetTileToMove.gridLocation + new Vector3Int(1, 0, 0)));
                            break;

                        case Direction.EAST:
                            redirectTile = OverlayTileManager.Instance.GetOverlayTile((Vector2Int)(targetTileToMove.gridLocation + new Vector3Int(0, 1, 0)));
                            break;

                        case Direction.WEST:
                            redirectTile = OverlayTileManager.Instance.GetOverlayTile((Vector2Int)(targetTileToMove.gridLocation - new Vector3Int(0, 1, 0)));
                            break;
                    }
                }

                thisUnit.ShowPopUpText("Detected Stealth Hero!!", Color.red);
                StartCoroutine(DetectedStealthUnit(redirectTile));
            }
            //if you have reached the end, and are suppose to attack, havent attacked, havent foundStealthHero and there is a target.
            else if (totalPathList.Count == 0 && (inAtkRange || inMoveAndAttackRange) && !hasAttacked && !detectedStealthHero && totalHeroList.Count > 0)
            {
                AttackProcess(thisUnit, targetTileToMove);
            }
        }

        public void AttackProcess(Units thisUnit, OverlayTile targetTile)
        {
            if (targetTile.CheckUnitOnTile()?.GetComponent<Units>() != null)
            {
                targetTile.ShowEnemyTile();
                hasAttacked = true;

                StartCoroutine(Delay(thisUnit));
            }
        }

        public IEnumerator DetectedStealthUnit(OverlayTile redirectTile)
        {
            yield return new WaitForSeconds(1f);

            StartCoroutine(PathfindingManager.Instance.MoveTowardsTile(thisUnit, redirectTile, 0.25f));

            yield return new WaitUntil(() => Vector2.Distance(thisUnit.transform.position, redirectTile.transform.position) < 0.01f);
            targetTileToMove.CheckUnitOnTile()?.GetComponent<Units>().UpdateTokenLifeTime(STATUS_EFFECT.STEALTH_TOKEN);
            AttackProcess(thisUnit, targetTileToMove);

        }

        #endregion

        #region Calculations/Utility
        //delay used for attacks
        IEnumerator Delay(Units thisUnit)
        {
            yield return new WaitForSeconds(attackDelay);

            CombatManager.Instance.EnemyTargetUnit(targetTileToMove.CheckUnitOnTile().GetComponent<Units>(), thisUnit.BasicAttackSkill);
            targetTileToMove.HideTile();
            totalPathList.Clear();
        }

        void SortHeroesByDistance(Units thisUnit)
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
                rangeToClosest = sortedHeroes.ToList()[0].Value;
            }
            else
            {
                distancesDictionary.Clear();
                closestHero = null;
                rangeToClosest = 0;
            }
        }

        void UpdateHeroList()
        {
            //populating unitList
            totalHeroList.Clear();
            totalUnitList = FindObjectsOfType<Units>().ToList();

            //filter by heroes
            foreach (var unit in totalUnitList)
            {
                if (!unit.IsHostile && !unit.StealthToken)
                {
                    totalHeroList.Add(unit);
                }
            }
        }


        float FindDistanceBetweenUnit(Units target1, Units target2)
        {
            float dist;
            Vector3Int t1v, t2v;

            t1v = target1.ActiveTile.gridLocation;
            t2v = target2.ActiveTile.gridLocation;

            dist = (Mathf.Abs((t1v.x)-(t2v.x)) + Mathf.Abs((t1v.y) - (t2v.y)));

            return dist;
        }

        float FindDistanceBetweenTile(OverlayTile target1, OverlayTile target2)
        {
            float dist;
            Vector3Int t1v, t2v;

            t1v = target1.gridLocation;
            t2v = target2.gridLocation;

            dist = (Mathf.Abs((t1v.x) - (t2v.x)) + Mathf.Abs((t1v.y) - (t2v.y)));

            return dist;
        }

        bool IsTileAttackableFrom(OverlayTile target1, OverlayTile target2)
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
                            return false;
                        }
                        else return true;
                    }
                    else return true;
                } 
                else return false;
            }
            else if (target1.gridLocation.y == target2.gridLocation.y)
            {
                //same column/y
                if ((target1.gridLocation.x + selectedAttackRange >= target2.gridLocation.x) && (target1.gridLocation.x - selectedAttackRange <= target2.gridLocation.x))
                {
                    if (selectedAttackMinRange != 0)
                    {
                        if ((target1.gridLocation.x + selectedAttackMinRange > target2.gridLocation.x) && (target1.gridLocation.x - selectedAttackMinRange < target2.gridLocation.x))
                        {
                            return false;
                        }
                        else return true;
                    }
                    else return true;
                }
                else return false;
            }
            else return false;
        }

        #endregion
        
    }

}
