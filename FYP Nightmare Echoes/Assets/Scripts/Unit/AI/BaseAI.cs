using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NightmareEchoes.Grid;
using NightmareEchoes.Unit.Pathfinding;
using UnityEngine.Tilemaps;
using System.Linq;

//by Terrence
namespace NightmareEchoes.Unit.AI
{
    public class BaseAI : MonoBehaviour
    {
        [SerializeField] List<Units> heroList, unitList;
        [SerializeField] Units thisUnit, target;
        Units closestHero;
        float closestRange, targetRange;
        bool inAtkRange;
        bool inMoveAtkRange;
        int rangePlaceholder = 1;
        List<OverlayTile> moveableTiles;
        List<OverlayTile> attackFromLocations = new List<OverlayTile>();

        OverlayTile currTile, targetTile;
        OverlayTile moveToTile, bestMoveTile;
        List<OverlayTile> pathList = new List<OverlayTile>();
        Dictionary<Units, int> distancesDict = new Dictionary<Units, int>();
        Dictionary<string, float> utilityDictionary = new Dictionary<string, float>();
        
        Dictionary<Units, float> aggroDictionary = new Dictionary<Units, float>();
        float healthPercent;

        private void Start()
        {
            
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("============= New Action! =============");
                MakeDecision();
            }

            StartCoroutine(MoveProcess());
        }
        public void MakeDecision()
        {
            SortHeroesByDistance();
            currTile = thisUnit.ActiveTile;
            moveableTiles = RangeMovementFind.TileMovementRange(currTile, thisUnit.stats.MoveRange);

            healthPercent = 100 * thisUnit.stats.Health / thisUnit.stats.MaxHealth;
            Debug.Log(healthPercent);

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
                    AggressiveAction();
                    break;
                case "Retreat":
                    Debug.Log("Retreat Triggered");
                    break;
            }
        }

        void AggressiveAction()
        {
            //Range hardcoded as placeholder
            if (thisUnit.TypeOfUnit == TypeOfUnit.RANGED_UNIT)
            {
                rangePlaceholder = 4;
            }
            else if (thisUnit.TypeOfUnit == TypeOfUnit.MELEE_UNIT)
            {
                rangePlaceholder = 1;
            }

            target = closestHero;
            targetRange = closestRange;
            targetTile = target.ActiveTile;

            //range check FIXED, does not factor obstacles
            attackFromLocations.Clear();
            
            inAtkRange = false;
            inMoveAtkRange = false;
            /*if (((currTile.gridLocation.x + rangePlaceholder >= targetTile.gridLocation.x || currTile.gridLocation.x - rangePlaceholder <= targetTile.gridLocation.x) && targetTile.gridLocation.y == currTile.gridLocation.y) || ((currTile.gridLocation.y + rangePlaceholder >= targetTile.gridLocation.y || currTile.gridLocation.y - rangePlaceholder <= targetTile.gridLocation.y) && targetTile.gridLocation.x == currTile.gridLocation.x))
            {
                inAtkRange = true;
            }*/
            if (rangeFinder(currTile, targetTile, rangePlaceholder))
            {
                inAtkRange = true;
            }
          
            for (int i = 0; i < moveableTiles.Count; i++)
            {
                if (rangeFinder(moveableTiles[i], targetTile, rangePlaceholder))
                {
                    attackFromLocations.Add(moveableTiles[i]);
                    inMoveAtkRange = true;
                }
            }


            if (inAtkRange)
            {
                Debug.Log("Attackin");
                bestMoveTile = attackFromLocations[0];
                if (thisUnit.TypeOfUnit == TypeOfUnit.RANGED_UNIT)
                {
                    bestMoveTile = attackFromLocations[0];
                    for (int i = 0; i < attackFromLocations.Count; i++)
                    {
                        if (findDist(targetTile, attackFromLocations[i]) > findDist(targetTile, bestMoveTile))
                        {
                            bestMoveTile = attackFromLocations[i];
                        }
                    }
                    pathList = PathFind.FindPath(currTile, bestMoveTile, moveableTiles);
                }
                //wait until in position, then attack
            }
            else if (inMoveAtkRange)
            {
                Debug.Log("MoveAttackin");
                bestMoveTile = attackFromLocations[0];
                for (int i = 0; i < attackFromLocations.Count; i++)
                {
                    if (findDist(targetTile, attackFromLocations[i]) > findDist(targetTile, bestMoveTile))
                    {
                        bestMoveTile = attackFromLocations[i];
                    }
                }

                pathList = PathFind.FindPath(currTile, bestMoveTile, moveableTiles);
                //wait until in position, then attack

            }
            else
            {
                Debug.Log("Movin");
                //move towards target
                bestMoveTile = moveableTiles[0];
                for (int i = 0; i < moveableTiles.Count; i++)
                {
                    if (findDist(targetTile, moveableTiles[i]) < findDist(targetTile, bestMoveTile))
                    {
                        bestMoveTile = moveableTiles[i];
                    }
                }

                pathList = PathFind.FindPath(currTile, bestMoveTile, moveableTiles);
                //wait until in position, then pass turn
            }
        }
        void SortHeroesByDistance()
        {
            UpdateLists();

            //sorting
            int i, distTemp;

            //creating/updating distancesDict
            distancesDict.Clear();
            for (i = 0; i < heroList.Count; i++)
            {
                distTemp = (int)findDist(thisUnit, heroList[i]);
                distancesDict.Add(heroList[i], distTemp);
            }
            var sortedHeroes = distancesDict.OrderBy(distancesDict => distancesDict.Value);

            closestHero = sortedHeroes.ToList()[0].Key;
            closestRange = sortedHeroes.ToList()[0].Value;
            Debug.Log("Closest Hero: " + closestHero.Name + ", " + closestRange + " tiles away");
        }

        void UpdateLists()
        {
            //populating unitList
            heroList.Clear();
            unitList = FindObjectsOfType<Units>().ToList();

            //filter by heroes
            foreach (var Unit in unitList)
            {
                if (!Unit.IsHostile)
                {
                    heroList.Add(Unit);
                }
            }
        }

        float findDist(Units target1, Units target2)
        {
            float dist;
            Vector3Int t1v, t2v;

            t1v = target1.ActiveTile.gridLocation;
            t2v = target2.ActiveTile.gridLocation;

            dist = (Mathf.Abs((t1v.x)-(t2v.x)) + Mathf.Abs((t1v.y) - (t2v.y)));

            return dist;
        }

        float findDist(OverlayTile target1, OverlayTile target2)
        {
            float dist;
            Vector3Int t1v, t2v;

            t1v = target1.gridLocation;
            t2v = target2.gridLocation;

            dist = (Mathf.Abs((t1v.x) - (t2v.x)) + Mathf.Abs((t1v.y) - (t2v.y)));

            return dist;
        }

        bool rangeFinder(OverlayTile target1, OverlayTile target2, int range)
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
        IEnumerator MoveProcess()
        {
            if (pathList.Count > 0) 
            {
                PathfindingManager.Instance.MoveAlongPath(thisUnit.gameObject, pathList);
                yield return null;
            }
            else
            {

            }
        }
    }

}
