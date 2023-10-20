using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NightmareEchoes.Grid;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;
using static UnityEditor.Progress;


//created by Vinn, editted by Alex
namespace NightmareEchoes.Unit.Pathfinding
{
    public static class Pathfinding
    {
        public static List<OverlayTile> FindPath(OverlayTile start, OverlayTile end, List<OverlayTile> LimitTiles)
        {
            var overLayTileManager = OverlayTileManager.Instance;
            List<OverlayTile> openList = new List<OverlayTile>();
            List<OverlayTile> endList = new List<OverlayTile>();

            openList.Add(start);

            while (openList.Count > 0)
            {
                OverlayTile currentOverlayTile = GetLowestFTile(openList);

                openList.Remove(currentOverlayTile);
                endList.Add(currentOverlayTile);

                if (currentOverlayTile == end)
                {
                    //finalize our path
                    return GetFinishedList(start, end);
                }

                var neighbourTiles = overLayTileManager.GetNeighbourTiles(currentOverlayTile, LimitTiles);

                for(int i = 0; i < neighbourTiles.Count; i++) 
                {
                    if (neighbourTiles[i].isBlocked || endList.Contains(neighbourTiles[i]))
                    {
                        continue;
                    }

                    neighbourTiles[i].G = GetManhattanDistance(start, neighbourTiles[i]);
                    neighbourTiles[i].H = GetManhattanDistance(end, neighbourTiles[i]);

                    neighbourTiles[i].prevTile = currentOverlayTile;

                    if (!openList.Contains(neighbourTiles[i]))
                    {
                        openList.Add(neighbourTiles[i]);
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
                    surroundingTiles.AddRange(overLayTileManager.GetNeighbourTiles(tileForPreviousStep[i], new List<OverlayTile>()));
                }

                inRangeTiles.AddRange(surroundingTiles);
                tileForPreviousStep = surroundingTiles.Distinct().ToList();
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
                    surroundingTiles.AddRange(overLayTileManager.GetNeighbourTiles(tileForPreviousStep[i], new List<OverlayTile>()));
                }

                inRangeTiles.AddRange(surroundingTiles);
                tileForPreviousStep = surroundingTiles.Distinct().ToList();
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

        private static OverlayTile GetLowestFTile(List<OverlayTile> tiles)
        {
            OverlayTile lowestF = tiles[0];

            for(int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i].F < lowestF.F)
                {
                    lowestF = tiles[i];
                }
            }
 
            return lowestF;
        }

        private static int GetManhattanDistance(OverlayTile start, OverlayTile neighbour)
        {
            return Mathf.Abs(start.gridLocation.x - neighbour.gridLocation.x) + Mathf.Abs(start.gridLocation.y - neighbour.gridLocation.y);
        }

        private static List<OverlayTile> GetFinishedList(OverlayTile start, OverlayTile end)
        {
            var finishedList = new List<OverlayTile>();

            OverlayTile currentOverlayTile = end;

            while (currentOverlayTile != start)
            {
                finishedList.Add(currentOverlayTile);
                currentOverlayTile = currentOverlayTile.prevTile;
            }

            finishedList.Reverse();

            return finishedList;
        }
    }

}
