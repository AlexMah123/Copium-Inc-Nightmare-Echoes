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
        int rangePlaceholder;
        OverlayTile overlayTile;
        List<OverlayTile> moveableTiles;
        
        OverlayTile currTile, targetTile;
        Vector3Int V3Int;
        Vector2 cellThis, cellTarget, cellTemp;
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
        }
        public void MakeDecision()
        {
            SortHeroesByDistance();

            currTile = thisUnit.ActiveTile;
            //moveableTiles = RangeMovementFind.TileMovementRange(thisOverlay, thisUnit.stats.MoveRange, !overlayTile.PlayerOnTile);
            //PathFind.FindPath(three overloads);
            //PathfindingManager.moveAlongPath();

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
            Debug.Log("Aggressive Action Triggered");

            //placeholder targeting, range placeholder
            rangePlaceholder = 1;
            target = closestHero;
            targetRange = closestRange;
            targetTile = target.ActiveTile;

            //range check FIXED
            if (((targetTile.gridLocation.x + rangePlaceholder >= currTile.gridLocation.x || targetTile.gridLocation.x - rangePlaceholder <= currTile.gridLocation.x) && targetTile.gridLocation.y == currTile.gridLocation.y) || ((targetTile.gridLocation.y + rangePlaceholder >= currTile.gridLocation.y || targetTile.gridLocation.y - rangePlaceholder <= currTile.gridLocation.y) && targetTile.gridLocation.x == currTile.gridLocation.x))
            {
                inAtkRange = true;
            }
            
            if (targetRange <= rangePlaceholder + thisUnit.stats.MoveRange)
            {
                inMoveAtkRange = true;
            }

            if (inAtkRange)
            {
                //retreat if ranged, attack target
            }
            else if (inMoveAtkRange)
            {
                //move, attack target
            }
            else
            {
                //move towards target
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
    }

}
