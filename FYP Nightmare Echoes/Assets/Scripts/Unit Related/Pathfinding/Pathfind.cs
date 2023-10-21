using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NightmareEchoes.Grid;
using UnityEngine.Tilemaps;

//created by Vinn, editted by Alex
namespace NightmareEchoes.Unit.Pathfinding
{
    public static class Pathfind
    {
        public static List<OverlayTile> FindPath(OverlayTile start, OverlayTile end, List<OverlayTile> tilesInRange)
        {
            var overLayTileManager = OverlayTileManager.Instance;
            List<OverlayTile> currTilesToCheck = new List<OverlayTile>();
            List<OverlayTile> prevTilesToCheck = new List<OverlayTile>();

            currTilesToCheck.Add(start);

            while (currTilesToCheck.Count > 0)
            {
                OverlayTile currentOverlayTile = currTilesToCheck.OrderBy(x => x.F).First();

                currTilesToCheck.Remove(currentOverlayTile);
                prevTilesToCheck.Add(currentOverlayTile);

                if (currentOverlayTile == end)
                {
                    //finalize our path
                    return GetFinishedList(start, end);
                }

                var neighbourTiles = overLayTileManager.GetNeighbourTiles(currentOverlayTile, tilesInRange);

                for(int i = 0; i < neighbourTiles.Count; i++) 
                {
                    if (neighbourTiles[i].isBlocked || prevTilesToCheck.Contains(neighbourTiles[i]))
                    {
                        continue;
                    }

                    neighbourTiles[i].G = GetManhattanDistance(start, neighbourTiles[i]);
                    neighbourTiles[i].H = GetManhattanDistance(end, neighbourTiles[i]);

                    neighbourTiles[i].prevTile = currentOverlayTile;

                    if (!currTilesToCheck.Contains(neighbourTiles[i]))
                    {
                        currTilesToCheck.Add(neighbourTiles[i]);
                    }
                }
            }

            return new List<OverlayTile>();
        }

        public static List<OverlayTile> FindTilesInRange(OverlayTile startTile, int range, bool ignoreProps)
        {
            var overLayTileManager = OverlayTileManager.Instance;
            var inRangeTiles = new List<OverlayTile> { startTile };
            var tileForPreviousStep = new List<OverlayTile> { startTile };

            int stepCount = 0;
            bool unitAlignment = false;

            while (stepCount < range)
            {
                var surroundingTiles = new List<OverlayTile>();

                for(int i = 0; i < tileForPreviousStep.Count; i++)
                {
                    var neighbourTiles = overLayTileManager.GetNeighbourTiles(tileForPreviousStep[i], new List<OverlayTile>());

                    for(int j = 0; j < neighbourTiles.Count; j++)
                    {
                        if (!inRangeTiles.Contains(neighbourTiles[j]))
                        {
                            surroundingTiles.Add(neighbourTiles[j]);
                            inRangeTiles.Add(neighbourTiles[j]);

                        }
                    }
                }

                tileForPreviousStep = surroundingTiles;
                stepCount++;
            }

            if (startTile.CheckEntityGameObjectOnTile())
            {
                unitAlignment = startTile.CheckEntityGameObjectOnTile().GetComponent<Entity>().IsHostile;
            }

            var filteredTiles = new List<OverlayTile>();

            for(int i = 0; i < inRangeTiles.Count; i++)
            {
                var entityOnTile = inRangeTiles[i].CheckEntityGameObjectOnTile()?.GetComponent<Entity>();
                var obstacleOnTile = inRangeTiles[i].CheckObstacleOnTile();

                if (!inRangeTiles[i].CheckEntityGameObjectOnTile() && !obstacleOnTile)
                {
                    filteredTiles.Add(inRangeTiles[i]);
                }
                else if (entityOnTile != null)
                {
                    if (entityOnTile.IsHostile == unitAlignment && !entityOnTile.StealthToken && !entityOnTile.IsProp)
                    {
                        filteredTiles.Add(inRangeTiles[i]);
                    }

                    if (ignoreProps && entityOnTile.IsProp)
                    {
                        filteredTiles.Add(inRangeTiles[i]);
                    }
                }
            }

            return filteredTiles;
        }
        
        public static List<OverlayTile> FindTilesInRangeToDestination(OverlayTile startTile, OverlayTile endTile, bool ignoreProps)
        {
            var overLayTileManager = OverlayTileManager.Instance;
            var inRangeTiles = new List<OverlayTile> { startTile };
            var tileForPreviousStep = new List<OverlayTile> { startTile };
            
            bool unitAlignment = false;
            bool destinationFound = false;

            while (!destinationFound)
            {
                var surroundingTiles = new List<OverlayTile>();

                for (int i = 0; i < tileForPreviousStep.Count; i++)
                {
                    if (tileForPreviousStep[i] == endTile)
                        destinationFound = true;

                    var neighbourTiles = overLayTileManager.GetNeighbourTiles(tileForPreviousStep[i], new List<OverlayTile>());

                    for (int j = 0; j < neighbourTiles.Count; j++)
                    {
                        if (!inRangeTiles.Contains(neighbourTiles[j]))
                        {
                            surroundingTiles.Add(neighbourTiles[j]);
                            inRangeTiles.Add(neighbourTiles[j]);

                        }
                    }
                }

                tileForPreviousStep = surroundingTiles;

                if(inRangeTiles.Count >= TileMapManager.Instance.width * TileMapManager.Instance.length)
                {
                    Debug.LogWarning("CANNOT FIND DESTINATION");
                    destinationFound = true;
                    break;
                }
            }

            if (startTile.CheckEntityGameObjectOnTile())
            {
                unitAlignment = startTile.CheckEntityGameObjectOnTile().GetComponent<Entity>().IsHostile;
            }

            var filteredTiles = new List<OverlayTile>();

            for (int i = 0; i < inRangeTiles.Count; i++)
            {
                var entityOnTile = inRangeTiles[i].CheckEntityGameObjectOnTile()?.GetComponent<Entity>();
                var obstacleOnTile = inRangeTiles[i].CheckObstacleOnTile();

                if (!inRangeTiles[i].CheckEntityGameObjectOnTile() && !obstacleOnTile)
                {
                    filteredTiles.Add(inRangeTiles[i]);
                }
                else if (entityOnTile != null)
                {
                    if (entityOnTile.IsHostile == unitAlignment && !entityOnTile.StealthToken && !entityOnTile.IsProp)
                    {
                        filteredTiles.Add(inRangeTiles[i]);
                    }

                    if (ignoreProps && entityOnTile.IsProp)
                    {
                        filteredTiles.Add(inRangeTiles[i]);
                    }
                }
            }

            return filteredTiles;
        }

        private static int GetManhattanDistance(OverlayTile start, OverlayTile neighbour)
        {
            return Mathf.Abs(start.gridLocation.x - neighbour.gridLocation.x) + Mathf.Abs(start.gridLocation.y - neighbour.gridLocation.y);
        }

        private static List<OverlayTile> GetFinishedList(OverlayTile start, OverlayTile end)
        {
            var finishedList = new List<OverlayTile>();

            OverlayTile currentTile = end;

            while (currentTile != start)
            {
                finishedList.Add(currentTile);
                currentTile = currentTile.prevTile;
            }

            finishedList.Reverse();

            return finishedList;
        }
    }

}
