using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NightmareEchoes.Grid;
using UnityEngine.Tilemaps;


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

                foreach (var neighbour in neighbourTiles)
                {
                    if (neighbour.isBlocked || endList.Contains(neighbour))
                    {
                        continue;
                    }

                    neighbour.G = GetManhattanDistance(start, neighbour);
                    neighbour.H = GetManhattanDistance(end, neighbour);

                    neighbour.prevTile = currentOverlayTile;

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
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

            //inRangeTiles.Add(startTile);
            //tileForPreviousStep.Add(startTile);

            while (stepCount < range)
            {
                var surroundingTiles = new List<OverlayTile>();

                foreach (var item in tileForPreviousStep)
                {
                    surroundingTiles.AddRange(overLayTileManager.GetNeighbourTiles(item, new List<OverlayTile>()));
                }

                inRangeTiles.AddRange(surroundingTiles);
                tileForPreviousStep = GetDistinctTiles(surroundingTiles);
                stepCount++;
            }

            if (startTile.CheckEntityGameObjectOnTile())
            {
                unitAlignment = startTile.CheckEntityGameObjectOnTile().GetComponent<Entity>().IsHostile;
            }

            var filteredTiles = new List<OverlayTile>();

            foreach (var tile in inRangeTiles.Distinct())
            {
                var entityOnTile = tile.CheckEntityGameObjectOnTile()?.GetComponent<Entity>();
                var obstacleOnTile = tile.CheckObstacleOnTile();

                if (!tile.CheckEntityGameObjectOnTile() && !obstacleOnTile)
                {
                    filteredTiles.Add(tile);
                }
                else if (entityOnTile != null)
                {
                    if (entityOnTile.IsHostile == unitAlignment && !entityOnTile.StealthToken && !entityOnTile.IsProp)
                    {
                        filteredTiles.Add(tile);
                    }

                    if (ignoreProps && entityOnTile.IsProp)
                    {
                        filteredTiles.Add(tile);
                    }
                }
            }

            return GetFilteredTilesInRange(startTile, filteredTiles, range);
        }

        private static OverlayTile GetLowestFTile(List<OverlayTile> tiles)
        {
            OverlayTile lowestF = tiles[0];
            foreach (var tile in tiles)
            {
                if (tile.F < lowestF.F)
                {
                    lowestF = tile;
                }
            }
            return lowestF;
        }

        private static List<OverlayTile> GetDistinctTiles(List<OverlayTile> tiles)
        {
            var distinctTiles = new List<OverlayTile>();
            foreach (var tile in tiles)
            {
                if (!distinctTiles.Contains(tile))
                {
                    distinctTiles.Add(tile);
                }
            }
            return distinctTiles;
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

        private static List<OverlayTile> GetFilteredTilesInRange(OverlayTile startTile, List<OverlayTile> tiles, int range)
        {
            var filteredTiles = new List<OverlayTile>();
            foreach (var tile in tiles)
            {
                var path = FindPath(startTile, tile, tiles);
                if (path.Count <= range && path.Count > 0)
                {
                    filteredTiles.Add(tile);
                }
            }
            return filteredTiles;
        }
    }

}
