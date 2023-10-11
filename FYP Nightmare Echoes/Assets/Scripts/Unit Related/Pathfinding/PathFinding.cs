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
                    //if the tile is marked blocked, or already is in the list, ignore
                    if (neighbour.isBlocked || endList.Contains(neighbour))
                    {
                        continue;
                    }

                    //ManhattenDistance calculates the distance between the start and the neighbours using the G,F and H cost
                    neighbour.G = GetManHattenDistance(start , neighbour);
                    neighbour.H =  GetManHattenDistance(end, neighbour);

                    neighbour.prevTile = currentOverlayTile;

                    //if there is no duplicate of the tile, add it
                    if (!openList.Contains(neighbour))
                    { 
                        openList.Add(neighbour);
                    }
                }
            }

            return new List<OverlayTile>();    
        }

        public static List<OverlayTile> FindTilesInRange(OverlayTile startTile, int range, bool includeProps)
        {
            var inRangeTiles = new List<OverlayTile>();
            int stepCount = 0;
            bool UnitAlignment = false;
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


            //cache the unit's type based on the start tile's unit
            if(startTile.CheckEntityGameObjectOnTile())
            {
                UnitAlignment = startTile.CheckEntityGameObjectOnTile().GetComponent<Entity>().IsHostile;

            }

            var filteredTiles = new List<OverlayTile>();

            //foreach tile in a filtered list with no duplicates
            foreach (var tiles in inRangeTiles.Distinct().ToList())
            {
                //if tile does not have a unit and have an obstacle on it
                if (!tiles.CheckEntityGameObjectOnTile() && !tiles.CheckObstacleOnTile())
                {
                    filteredTiles.Add(tiles);
                }
                //else if there is a unit
                else if (tiles.CheckEntityGameObjectOnTile()?.GetComponent<Entity>() != null)
                {
                    //check if that unit is the same type as the UnitAlignment, if so, add it.
                    if (tiles.CheckEntityGameObjectOnTile().GetComponent<Entity>().IsHostile == UnitAlignment && !tiles.CheckEntityGameObjectOnTile().GetComponent<Entity>().StealthToken && !tiles.CheckEntityGameObjectOnTile().GetComponent<Entity>().IsProp)
                        filteredTiles.Add(tiles);

                    //if do include props (for enemy)
                    if (includeProps)
                    {
                        if(tiles.CheckEntityGameObjectOnTile().GetComponent<Entity>().IsProp)
                        {
                            filteredTiles.Add(tiles);
                        }
                    }                    
                }
            }

            return (from tile in filteredTiles let path = FindPath(startTile, tile, filteredTiles) where path.Count <= range && path.Count > 0 select tile).ToList();
        }


        #region Find Pathfind Calculation
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
