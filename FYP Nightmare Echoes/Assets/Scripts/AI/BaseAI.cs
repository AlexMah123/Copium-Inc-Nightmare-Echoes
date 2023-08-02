using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NightmareEchoes.Unit;
using NightmareEchoes.Grid;
using UnityEngine.Tilemaps;
using System.Linq;

//by Terrence
namespace NightmareEchoes.AI
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
            

        }

        void AggressiveAction()
        {
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
            int i, j;
            BaseUnit temp;
            int temp2;
            bool swapped;

            //creating/updating distancesDict
            distancesDict.Clear();
            for (i = 0; i < heroList.Count; i++)
            {
                V3Int = new Vector3Int((int)heroList[i].transform.position.x, (int)heroList[i].transform.position.y, (int)heroList[i].transform.position.z);
                cellTemp = new Vector2(TileMapManager.Instance.tilemap.WorldToCell(V3Int).x, TileMapManager.Instance.tilemap.WorldToCell(V3Int).y);
                temp2 = ((int)cellThis.x - (int)cellTemp.x) + ((int)cellThis.y - (int)cellTemp.y); //rough distance
                distancesDict.Add(heroList[i], temp2);
            }
            distancesDict.OrderBy(distancesDict => distancesDict.Value);

            closestHero = distancesDict.ToList()[0].Key;
            closestRange = distancesDict.ToList()[0].Value;

            //sorting heroList based on distancesDict
            /*for (i = 0; i < heroList.Count - 1; i++)
            {
                swapped = false;
                for (j = 0; j < heroList.Count - i - 1; j++)
                {
                    if (distancesDict[heroList[i]] > distancesDict[heroList[i + 1]]) 
                    {
                        temp = heroList[j];
                        heroList[j] = heroList[j + 1];
                        heroList[j + 1] = temp;

                        swapped = true;
                    }
                }
                if (swapped == false)
                    break;
            }*/


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
