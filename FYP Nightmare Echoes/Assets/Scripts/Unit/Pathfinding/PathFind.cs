using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NightmareEchoes.Grid;
using UnityEngine.Tilemaps;


//created by Vinn
namespace NightmareEchoes.Unit.Pathfinding
{
    public static class PathFind
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

    }
}
