using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace NightmareEchoes.Grid
{
    public class PathfindingScript
    {
/*

        public List<TileData> FindPath(TileData startNode, TileData endNode)
        { 
            List<TileData> openList = new List<TileData>(); 
            List<TileData> closeList = new List<TileData>();

            openList.Add(startNode);

            while (openList.Count > 0)
            { 
                TileData currentTile = openList.OrderBy(x => x.Fcost).First();

                openList.Remove(currentTile);
                closeList.Add(currentTile);

                if (currentTile == endNode)
                {
                    return GetFinishedList(startNode,endNode);
                }

                var neighbourTiles = GetNeighBourTiles(currentTile);

                foreach (var neighbour in neighbourTiles)
                {
                    //The mathf.abs is for jumpheight and if its above 1 then it cannot jump (Adding it for now incase we have height maps in game)
                    if (neighbour.isBlocked || closeList.Contains(neighbour) || Mathf.Abs(currentTile.GridLocation.z - neighbour.GridLocation.z) > 1)
                    {
                        continue;
                    }

                    neighbour._Gcost = GetManhattenDistance(startNode,neighbour);
                    neighbour._Hcost = GetManhattenDistance(endNode, neighbour);

                    neighbour.prevTile = currentTile;

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }

            }

            return new List<TileData>();
        }

        private List<TileData> GetFinishedList(TileData startNode, TileData endNode)
        {
            List<TileData> finishedList = new List<TileData>();

            TileData curTile = endNode;
            
            while (curTile != startNode) 
            {
                finishedList.Add(curTile);
                curTile = curTile.prevTile;
            }

            finishedList.Reverse();

            return finishedList;
        }

        private int GetManhattenDistance(TileData start, TileData neighbour)
        {
            return Mathf.Abs(start.GridLocation.x - neighbour.GridLocation.x) + Mathf.Abs(start.GridLocation.y - neighbour.GridLocation.y);
        }

        private List<TileData> GetNeighBourTiles(TileData currentTile)
        {
            //TileMapManager mapManager;
            var map = TileMapManager.Instance.MAP;

            List<TileData> neighbours = new List<TileData>();

            //top
            Vector3Int locationToCheck = new Vector3Int(currentTile.GridLocation.x, currentTile.GridLocation.y + 1,0);

            if (map.ContainsKey(locationToCheck))
            {
                neighbours.Add(map[locationToCheck]);  
            }
            //bottom
             locationToCheck = new Vector3Int(currentTile.GridLocation.x, currentTile.GridLocation.y - 1, 0);

            if (map.ContainsKey(locationToCheck))
            {
                neighbours.Add(map[locationToCheck]);
            }
            //right
            locationToCheck = new Vector3Int(currentTile.GridLocation.x +1, currentTile.GridLocation.y, 0);

            if (map.ContainsKey(locationToCheck))
            {
                neighbours.Add(map[locationToCheck]);
            }
            //left
            locationToCheck = new Vector3Int(currentTile.GridLocation.x-1, currentTile.GridLocation.y, 0);

            if (map.ContainsKey(locationToCheck))
            {
                neighbours.Add(map[locationToCheck]);
            }

            return neighbours;
        }
        */
    }
}
