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
            List<OverlayTile> openSet = new List<OverlayTile> { start };
            List<OverlayTile> closedSet = new List<OverlayTile>();

            start.G = 0;
            start.H = GetManhattanDistance(start, end);
            start.prevTile = null;

            while (openSet.Count > 0)
            {
                //find the node with the lowest F value
                OverlayTile currentOverlayTile = openSet.OrderBy(x => x.F).First();

                if (currentOverlayTile == end)
                {
                    //finalize our path
                    return GetFinishedList(start, end);
                }

                openSet.Remove(currentOverlayTile);
                closedSet.Add(currentOverlayTile);

                var neighbourTiles = overLayTileManager.GetNeighbourTiles(currentOverlayTile, tilesInRange);

                for(int i = 0; i < neighbourTiles.Count; i++) 
                {
                    if (neighbourTiles[i].isBlocked || closedSet.Contains(neighbourTiles[i]))
                    {
                        continue;
                    }

                    int tentativeG = currentOverlayTile.G + GetManhattanDistance(currentOverlayTile, neighbourTiles[i]);

                    if (!openSet.Contains(neighbourTiles[i]) || tentativeG < neighbourTiles[i].G)
                    {
                        neighbourTiles[i].prevTile = currentOverlayTile;
                        neighbourTiles[i].G = tentativeG;
                        neighbourTiles[i].H = GetManhattanDistance(neighbourTiles[i], end);

                        if (!openSet.Contains(neighbourTiles[i]))
                        {
                            openSet.Add(neighbourTiles[i]);
                        }
                    }
                }
            }

            return new List<OverlayTile>();
        }

        public static List<OverlayTile> FindTilesInRange(OverlayTile startTile, int range, bool ignoreProps = false, bool ignoreObstacles = false)
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

                //if there is no entity and no obstacle
                if (!inRangeTiles[i].CheckEntityGameObjectOnTile() && !obstacleOnTile)
                {
                    filteredTiles.Add(inRangeTiles[i]);
                }
                else if (entityOnTile != null)
                {
                    //if there is an entity but they are the same type, do not have stealth, and is not a prop
                    if (entityOnTile.IsHostile == unitAlignment && !entityOnTile.StealthToken && !entityOnTile.IsProp)
                    {
                        filteredTiles.Add(inRangeTiles[i]);
                    }

                    //if ignore props and there is a prop
                    if (ignoreProps && entityOnTile.IsProp)
                    {
                        filteredTiles.Add(inRangeTiles[i]);
                    }
                }

                //if ignore obstacle and there is an obstacle
                if (ignoreObstacles && obstacleOnTile)
                {
                    filteredTiles.Add(inRangeTiles[i]);
                }
            }

            return GetFilteredTilesInRange(startTile, filteredTiles, range);
        }
        
        public static List<OverlayTile> FindTilesInRangeToDestination(OverlayTile startTile, OverlayTile endTile, bool ignoreProps = false, bool ignoreObstacles = false)
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

                //if there is no entity and no obstacle
                if (!inRangeTiles[i].CheckEntityGameObjectOnTile() && !obstacleOnTile)
                {
                    filteredTiles.Add(inRangeTiles[i]);
                }
                else if (entityOnTile != null)
                {
                    //if there is an entity but they are the same type, do not have stealth, and is not a prop
                    if (entityOnTile.IsHostile == unitAlignment && !entityOnTile.StealthToken && !entityOnTile.IsProp)
                    {
                        filteredTiles.Add(inRangeTiles[i]);
                    }

                    //add back if it is a prop
                    if (ignoreProps && entityOnTile.IsProp)
                    {
                        filteredTiles.Add(inRangeTiles[i]);
                    }
                }

                //if ignore obstacle and there is a obstacle
                if (ignoreObstacles && obstacleOnTile)
                {
                    filteredTiles.Add(inRangeTiles[i]);
                }
            }

            return filteredTiles;
        }

        private static int GetManhattanDistance(OverlayTile start, OverlayTile end)
        {
            return Mathf.Abs(start.gridLocation.x - end.gridLocation.x) + Mathf.Abs(start.gridLocation.y - end.gridLocation.y);
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

        private static List<OverlayTile> GetFilteredTilesInRange(OverlayTile startTile, List<OverlayTile> tiles, int range)
        {
            var filteredTiles = new List<OverlayTile>();

            for(int i = 0; i < tiles.Count; i++)
            {
                var path = FindPath(startTile, tiles[i], tiles);

                if (path.Count <= range && path.Count > 0)
                {
                    filteredTiles.Add(tiles[i]);
                }
            }

            return filteredTiles;
        }
    }

}
