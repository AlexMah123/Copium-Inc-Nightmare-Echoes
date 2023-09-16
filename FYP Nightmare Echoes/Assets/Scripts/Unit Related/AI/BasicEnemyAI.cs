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
        public float attackDelay = 1f;

        Units targetHero, closestHero;
        float rangeToTarget, rangeToClosest;
        public bool inAtkRange, inMoveAndAttackRange, hasAttacked;
        int rangePlaceholder;
        int rngHelper;
        float currTileUtil, highestTileUtil;
        List<OverlayTile> tilesInRange;
        List<OverlayTile> possibleAttackLocations = new List<OverlayTile>();

        OverlayTile unitCurrentTile, targetTile;
        OverlayTile moveToTile, bestMoveTile;

        Color pathClr = Color.yellow;
        Color moveableClr = new Color(1, 0.8f, 0.8f);

        Dictionary<Units, int> distancesDictionary = new Dictionary<Units, int>();
        Dictionary<string, float> utilityDictionary = new Dictionary<string, float>();
        Dictionary<Units, float> aggroDictionary = new Dictionary<Units, float>();
        float healthPercent;

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
            
        }

        public void MakeDecision(Units thisUnit)
        {
            //reset values
            hasAttacked = false;

            //sort heros by distance and find tiles in range
            SortHeroesByDistance(thisUnit);

            if (totalHeroList.Count > 0)
            {
                unitCurrentTile = thisUnit.ActiveTile;

                tilesInRange = Pathfinding.Pathfinding.FindTilesInRange(unitCurrentTile, thisUnit.stats.MoveRange);
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

        void AggressiveAction(Units thisUnit)
        {
            rangePlaceholder = thisUnit.gameObject.GetComponent<Skill>().Range;

            targetHero = closestHero;
            rangeToTarget = rangeToClosest;
            targetTile = targetHero.ActiveTile;

            //RESETS BOOLS
            possibleAttackLocations.Clear();
            inAtkRange = false;
            inMoveAndAttackRange = false;

            #region checks if unit is inAtkRange/inMoveAndAttackRagne
            if (IsTileAttackableFrom(unitCurrentTile, targetTile, rangePlaceholder))
            {
                inAtkRange = true;
            }
          
            //Checks tiles in range for possibleAttackableLocations if they do not have a unit on it
            for (int i = 0; i < tilesInRange.Count; i++)
            {
                if (IsTileAttackableFrom(tilesInRange[i], targetTile, rangePlaceholder))
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

            if (inAtkRange)
            {
                //Debug.Log("Attackin");
                bestMoveTile = possibleAttackLocations[0];
                rngHelper = 1;
                if (thisUnit.TypeOfUnit == TypeOfUnit.RANGED_UNIT)
                {
                    bestMoveTile = possibleAttackLocations[0];

                    for (int i = 0; i < possibleAttackLocations.Count; i++)
                    {
                        currTileUtil = FindDistanceBetweenTile(targetTile, possibleAttackLocations[i]);

                        if (currTileUtil > FindDistanceBetweenTile(targetTile, bestMoveTile))
                        {
                            bestMoveTile = possibleAttackLocations[i];
                        }
                        else if (FindDistanceBetweenTile(targetTile, possibleAttackLocations[i]) == FindDistanceBetweenTile(targetTile, bestMoveTile))
                        {
                            rngHelper++;
                            if (Random.Range(0.0f, 1.0f) < (1.0f / rngHelper))
                            {
                                bestMoveTile = possibleAttackLocations[i];
                            }
                        }
                    }

                    totalPathList = Pathfinding.Pathfinding.FindPath(unitCurrentTile, bestMoveTile, tilesInRange);
                }
            }
            else if (inMoveAndAttackRange)
            {
                //Debug.Log("MoveAttackin");
                bestMoveTile = possibleAttackLocations[0];
                rngHelper = 1;

                for (int i = 0; i < possibleAttackLocations.Count; i++)
                {
                    currTileUtil = FindDistanceBetweenTile(targetTile, possibleAttackLocations[i]);

                    switch (targetHero.Direction)
                    {
                        case Direction.North:
                            if ((possibleAttackLocations[i].gridLocation.x > targetTile.gridLocation.x) && (possibleAttackLocations[i].gridLocation.y == targetTile.gridLocation.y))
                            {
                                currTileUtil += 10;
                            }
                            break;
                        case Direction.South:
                            if ((possibleAttackLocations[i].gridLocation.x < targetTile.gridLocation.x) && (possibleAttackLocations[i].gridLocation.y == targetTile.gridLocation.y))
                            {
                                currTileUtil += 10;
                            }
                            break;
                        case Direction.East:
                            if ((possibleAttackLocations[i].gridLocation.x == targetTile.gridLocation.x) && (possibleAttackLocations[i].gridLocation.y < targetTile.gridLocation.y))
                            {
                                currTileUtil += 10;
                            }
                            break;
                        case Direction.West:
                            if ((possibleAttackLocations[i].gridLocation.x == targetTile.gridLocation.x) && (possibleAttackLocations[i].gridLocation.y > targetTile.gridLocation.y))
                            {
                                currTileUtil += 10;
                            }
                            break;
                    }
                    if (currTileUtil > FindDistanceBetweenTile(targetTile, bestMoveTile))
                    {
                        bestMoveTile = possibleAttackLocations[i];
                    }
                    else if (FindDistanceBetweenTile(targetTile, possibleAttackLocations[i]) == FindDistanceBetweenTile(targetTile, bestMoveTile))
                    {
                        rngHelper++;
                        if (Random.Range(0.0f, 1.0f) < (1.0f / rngHelper))
                        {
                            bestMoveTile = possibleAttackLocations[i];
                        }
                    }
                }

                totalPathList = Pathfinding.Pathfinding.FindPath(unitCurrentTile, bestMoveTile, tilesInRange);
            }
            else
            {
                //Debug.Log("Movin");
                //move towards target
                bestMoveTile = tilesInRange[0];
                rngHelper = 1;

                for (int i = 0; i < tilesInRange.Count; i++)
                {
                    if (!tilesInRange[i].CheckUnitOnTile())
                    {
                        if (FindDistanceBetweenTile(targetTile, tilesInRange[i]) < FindDistanceBetweenTile(targetTile, bestMoveTile))
                        {
                            bestMoveTile = tilesInRange[i];
                        }
                        else if (FindDistanceBetweenTile(targetTile, tilesInRange[i]) == FindDistanceBetweenTile(targetTile, bestMoveTile))
                        {
                            rngHelper++;
                            if (Random.Range(0.0f, 1.0f) < (1.0f / rngHelper))
                            {
                                //Debug.Log(1.0f / rngHelper);
                                bestMoveTile = tilesInRange[i];
                            }
                        }
                    }
                }

                totalPathList = Pathfinding.Pathfinding.FindPath(unitCurrentTile, bestMoveTile, tilesInRange);

            }

            #endregion
        }

        public void MoveProcess(Units thisUnit)
        {
            if (totalPathList.Count > 0)
            {
                PathfindingManager.Instance.MoveAlongPath(thisUnit, totalPathList, tilesInRange);
                CameraControl.Instance.UpdateCameraPan(thisUnit.gameObject);
            }

            if (totalPathList.Count == 0 && (inAtkRange || inMoveAndAttackRange) && !hasAttacked && totalHeroList.Count > 0)
            {
                AttackProcess(thisUnit);
            }
        }

        public void AttackProcess(Units thisUnit)
        {
            if (targetTile.CheckUnitOnTile()?.GetComponent<Units>() != null)
            {
                targetTile.ShowEnemyTile();
                CombatManager.Instance.EnemyTargetUnit(targetTile.CheckUnitOnTile().GetComponent<Units>(), thisUnit.BasicAttackSkill);

                StartCoroutine(Delay());
                hasAttacked = true;
            }
        }

        IEnumerator Delay()
        {
            yield return new WaitForSeconds(attackDelay);
            targetTile.HideTile();
        }

        #region Calculations
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
                //Debug.Log("Closest Hero: " + closestHero.Name + ", " + rangeToClosest + " tiles away");
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
            foreach (var Unit in totalUnitList)
            {
                if (!Unit.IsHostile)
                {
                    totalHeroList.Add(Unit);
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

        bool IsTileAttackableFrom(OverlayTile target1, OverlayTile target2, int range)
        {
            if (target1.gridLocation.x == target2.gridLocation.x)
            {
                //same row/x
                if ((target1.gridLocation.y + rangePlaceholder >= target2.gridLocation.y) && (target1.gridLocation.y - rangePlaceholder <= target2.gridLocation.y))
                {
                    return true;
                } 
                else return false;
            }
            else if (target1.gridLocation.y == target2.gridLocation.y)
            {
                //same column/y
                if ((target1.gridLocation.x + rangePlaceholder >= target2.gridLocation.x) && (target1.gridLocation.x - rangePlaceholder <= target2.gridLocation.x))
                {
                    return true;
                }
                else return false;
            }
            else return false;
        }

        #endregion
        
    }

}
