using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NightmareEchoes.Grid;


//created by Vinn
namespace NightmareEchoes.Unit.Pathfinding
{
    public static class RangeMovementFind
    {
        public static List<OverlayTile> TileMovementRange(OverlayTile startTile, int range)
        { 
            var inRangeTiles = new List<OverlayTile>();
            int stepCount = 0;

            inRangeTiles.Add(startTile);

            var TileForPreviousStep = new List<OverlayTile>();
            TileForPreviousStep.Add(startTile);

            while (stepCount < range )
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

            var removedTilesWithObstacles = inRangeTiles.Distinct().ToList().Where(tile => !tile.CheckUnitOnTile()).ToList();

            var cleanedTiles = new List<OverlayTile>();
            foreach (var tile in removedTilesWithObstacles)
            { 
                var path = PathFind.FindPath(startTile, tile, removedTilesWithObstacles);
                if (path.Count <= range)
                    cleanedTiles.Add(tile);
            }
            
            return cleanedTiles;
        }
    }
}
