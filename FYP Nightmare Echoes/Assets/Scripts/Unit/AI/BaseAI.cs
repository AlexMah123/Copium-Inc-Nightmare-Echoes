using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NightmareEchoes.Grid;
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
        
        Tile currTile, targetTile;
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
            V3Int = new Vector3Int((int)thisUnit.transform.position.x, (int)thisUnit.transform.position.y, (int)thisUnit.transform.position.z);
            currTile = (Tile)TileMapManager.Instance.tilemap.GetTile(V3Int);
            cellThis = new Vector2(TileMapManager.Instance.tilemap.WorldToCell(V3Int).x, TileMapManager.Instance.tilemap.WorldToCell(V3Int).y);
            SortHeroesByDistance();

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

            //update the bools below
            
            if (targetRange <= rangePlaceholder + thisUnit.stats.MoveRange)
            {
                inMoveAtkRange = true;
                if (targetRange <= rangePlaceholder)
                {
                    inAtkRange = true;
                }
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
                V3Int = new Vector3Int((int)heroList[i].transform.position.x, (int)heroList[i].transform.position.y, (int)heroList[i].transform.position.z);
                cellTemp = new Vector2(TileMapManager.Instance.tilemap.WorldToCell(V3Int).x, TileMapManager.Instance.tilemap.WorldToCell(V3Int).y);
                Debug.Log(cellTemp.x + ", " + cellTemp.y);
                distTemp = (Mathf.Abs((int)cellThis.x - (int)cellTemp.x)) + Mathf.Abs(((int)cellThis.y - (int)cellTemp.y)); //rough distance
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
    }

}
