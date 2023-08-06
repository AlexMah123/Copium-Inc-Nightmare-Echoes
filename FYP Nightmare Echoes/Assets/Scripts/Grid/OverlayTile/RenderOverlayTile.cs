using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NightmareEchoes.Grid
{
    public class RenderOverlayTile : MonoBehaviour
    {
        public static void RenderTiles(OverlayTile startTile, string type, int range)
        {
            var tileRange = new List<OverlayTile>();

            switch (type)
            {
                case "Line":
                    RenderLine(startTile, range);
                    break;
                default:
                    Debug.LogWarning("ERROR");
                    break;
            }
            
            foreach (var tile in tileRange)
            {
                tile.ShowTile();
            }
        }

        public static List<OverlayTile> RenderLine(OverlayTile startTile, int range)
        {
            var tileRange = new List<OverlayTile> { startTile };

            var possibleTileCoords = new List<Vector2Int>();
            for (var i = 1; i <= range; i++)
            {
                possibleTileCoords.Add(new Vector2Int(startTile.gridLocation.x + i, startTile.gridLocation.y));
                possibleTileCoords.Add(new Vector2Int(startTile.gridLocation.x - i, startTile.gridLocation.y)); 
                possibleTileCoords.Add(new Vector2Int(startTile.gridLocation.x, startTile.gridLocation.y + i)); 
                possibleTileCoords.Add(new Vector2Int(startTile.gridLocation.x, startTile.gridLocation.y - i)); 
            }
            
            var map = OverlayTileManager.Instance.map;
            foreach (var coord in possibleTileCoords.Where(coord => map.ContainsKey(coord)))
            {
                if (map.TryGetValue(coord, out var tile))
                    tileRange.Add(tile);
            }

            return tileRange;
        }
        
    }
}
