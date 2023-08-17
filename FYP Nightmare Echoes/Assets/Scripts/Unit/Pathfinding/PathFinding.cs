using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NightmareEchoes.Grid;
using UnityEngine.Tilemaps;


//created by Vinn, editted by Alex
namespace NightmareEchoes.Unit.Pathfinding
{
    public static class PathFinding
    {
        //The list at the end creates a limitor to how far the player can move which works for our player boundary range
        public static List<OverlayTile> FindPath(OverlayTile start, OverlayTile end, List<OverlayTile> LimitTiles)
        {
            List<OverlayTile> openList = new List<OverlayTile>();
            List<OverlayTile> endList = new List<OverlayTile>();

            openList.Add(start);

            while (openList.Count > 0)
            {
                OverlayTile currentOverlayTile = openList.OrderBy(x => x.F).First();

                openList.Remove(currentOverlayTile);
                endList.Add(currentOverlayTile);

                if (currentOverlayTile == end)
                {
                    //finalize our path
                    return GetFinishedList(start,end);

                }

                //function to get neighbourTiles
                var neighbourTiles = OverlayTileManager.Instance.GetNeighbourTiles(currentOverlayTile, LimitTiles);

                foreach (var neighbour in neighbourTiles) 
                {
                    if (neighbour.isBlocked || endList.Contains(neighbour))
                    {

                        continue;
                    }

                    //ManhattenDistance calculates the distance between the start and the neighbours using the G,F and H cost
                    neighbour.G = GetManHattenDistance(start , neighbour);
                    neighbour.H =  GetManHattenDistance(end, neighbour);

                    neighbour.prevTile = currentOverlayTile;

                    if (!openList.Contains(neighbour))
                    { 
                        openList.Add(neighbour);
                    }
                }
            }

            return new List<OverlayTile>();    
        }

        public static List<OverlayTile> FindTilesInRange(OverlayTile startTile, int range)
        {
            var inRangeTiles = new List<OverlayTile>();
            int stepCount = 0;

            inRangeTiles.Add(startTile);

            var TileForPreviousStep = new List<OverlayTile>();
            TileForPreviousStep.Add(startTile);

            while (stepCount < range)
            {
                var surroundingTiles = new List<OverlayTile>();

                foreach (var item in TileForPreviousStep)
                {
                    surroundingTiles.AddRange(OverlayTileManager.Instance.GetNeighbourTiles(item, new List<OverlayTile>()));
                }

                inRangeTiles.AddRange(surroundingTiles);
                TileForPreviousStep = surroundingTiles.Distinct().ToList();
                stepCount++;
            }



            var UnitAlignment = startTile.CheckUnitOnTile().GetComponent<Units>().IsHostile;

            var RemovedTileWithObstacles = new List<OverlayTile>();

            foreach (var tiles in inRangeTiles.Distinct().ToList())
            {
                if (!tiles.CheckUnitOnTile())
                    RemovedTileWithObstacles.Add(tiles);

                else if (tiles.CheckUnitOnTile().GetComponent<Units>().IsHostile == UnitAlignment)
                    RemovedTileWithObstacles.Add(tiles);
            }


            return (from tile in RemovedTileWithObstacles let path = FindPath(startTile, tile, RemovedTileWithObstacles) where path.Count <= range && path.Count > 0 select tile).ToList();
        }


        #region Pathfind Calculation
        private static int GetManHattenDistance(OverlayTile start , OverlayTile neighbour)
        {
            return Mathf.Abs(start.gridLocation.x - neighbour.gridLocation.x) + Mathf.Abs(start.gridLocation.y - neighbour.gridLocation.y);
        }

        private static List<OverlayTile> GetFinishedList(OverlayTile start, OverlayTile end)
        { 
            List<OverlayTile> finishedList = new List<OverlayTile>();

            OverlayTile currentOverlayTile = end;

            while (currentOverlayTile != start)
            { 
                finishedList.Add(currentOverlayTile);
                currentOverlayTile = currentOverlayTile.prevTile;
            }

            finishedList.Reverse();

            return finishedList;
        }
        #endregion
    }
}
