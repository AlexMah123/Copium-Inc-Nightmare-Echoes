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
        [SerializeField] List<BaseUnit> heroList, unitList;
        [SerializeField] BaseUnit thisUnit, target;
        BaseUnit closestHero;
        float closestRange;
        bool inAtkRange;
        bool inMoveAtkRange;
        Tile currTile, targetTile;
        Vector3Int V3Int;
        Vector2 cellThis, cellTarget, cellTemp;
        Dictionary<BaseUnit, int> distancesDict = new Dictionary<BaseUnit, int>();
        Dictionary<string, float> utilityDictionary = new Dictionary<string, float>();
        Dictionary<BaseUnit, float> aggroDictionary = new Dictionary<BaseUnit, float>();
        float healthPercent;

        private void Start()
        {
            
        }

        public void MakeDecision()
        {
            V3Int = new Vector3Int((int)thisUnit.transform.position.x, (int)thisUnit.transform.position.y, (int)thisUnit.transform.position.z);
            currTile = (Tile)TileMapManager.Instance.tilemap.GetTile(V3Int);
            cellThis = new Vector2(TileMapManager.Instance.tilemap.WorldToCell(V3Int).x, TileMapManager.Instance.tilemap.WorldToCell(V3Int).y);
            SortHeroesByDistance();

            healthPercent = 100 * thisUnit.Health / thisUnit.MaxHealth;
            
            //weight calculations
            utilityDictionary.Clear();
            utilityDictionary.Add("Attack", healthPercent);
            utilityDictionary.Add("Retreat", 0.4f);
            
            //sort by most utility score
            utilityDictionary.OrderByDescending(utilityDictionary => utilityDictionary.Value);

            switch (utilityDictionary.ToList()[0].Key)
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

            //update the bools below

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
                distTemp = ((int)cellThis.x - (int)cellTemp.x) + ((int)cellThis.y - (int)cellTemp.y); //rough distance
                distancesDict.Add(heroList[i], distTemp);
            }
            distancesDict.OrderBy(distancesDict => distancesDict.Value);

            closestHero = distancesDict.ToList()[0].Key;
            closestRange = distancesDict.ToList()[0].Value;
            Debug.Log("Closest Hero: " + closestHero.Name + ", " + closestRange + " tiles away");
        }

        void UpdateLists()
        {
            //populating unitList
            heroList.Clear();
            unitList = FindObjectsOfType<BaseUnit>().ToList();

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
