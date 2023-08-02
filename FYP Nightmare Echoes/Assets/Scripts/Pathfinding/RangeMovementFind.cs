using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NightmareEchoes.Pathfinding
{
    public static  class RangeMovementFind
    {
        public static List<OverlayTile> TileMovementRange(OverlayTile startTile, int range)
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

            return inRangeTiles.Distinct().ToList();
        }
    }
}
