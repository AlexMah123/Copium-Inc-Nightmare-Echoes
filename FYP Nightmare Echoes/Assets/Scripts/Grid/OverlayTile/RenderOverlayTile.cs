using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NightmareEchoes.Grid
{
    public class RenderOverlayTile : MonoBehaviour
    {
        public static RenderOverlayTile Instance;
        [SerializeField] private List<OverlayTile> activeRenders;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;
        }

        public void RenderTiles(OverlayTile startTile, string type, int range)
        {
            var tileRange = new List<OverlayTile> { startTile };
            var possibleTileCoords = new List<Vector2Int>();
            
            //Select Shape
            switch (type)
            {
                case "Line":
                    possibleTileCoords = RenderLine(startTile, range);
                    break;
                case "Square":
                    possibleTileCoords = RenderSquare(startTile, range);
                    break;
                default:
                    Debug.LogWarning("ERROR");
                    break;
            }
            
            //Trim Out of Bounds
            var map = OverlayTileManager.Instance.map;
            foreach (var coord in possibleTileCoords.Where(coord => map.ContainsKey(coord)))
            {
                if (OverlayTileManager.Instance.map.TryGetValue(coord, out var tile))
                    tileRange.Add(tile);
            }
            
            //Show valid
            foreach (var tile in tileRange)
            {
                tile.ShowAttackTile();
            }

            activeRenders = tileRange;
        }
        
        public void ClearRenders()
        {
            foreach (var tile in activeRenders)
            {
                tile.HideTile();
            }
            activeRenders.Clear();
        }
        
        private List<Vector2Int> RenderLine(OverlayTile startTile, int range)
        {
            var possibleTileCoords = new List<Vector2Int>();
            for (var i = 1; i <= range; i++)
            {
                possibleTileCoords.Add(new Vector2Int(startTile.gridLocation.x + i, startTile.gridLocation.y));
                possibleTileCoords.Add(new Vector2Int(startTile.gridLocation.x - i, startTile.gridLocation.y)); 
                possibleTileCoords.Add(new Vector2Int(startTile.gridLocation.x, startTile.gridLocation.y + i)); 
                possibleTileCoords.Add(new Vector2Int(startTile.gridLocation.x, startTile.gridLocation.y - i)); 
            }
            
            return possibleTileCoords;
        }
        
        private List<Vector2Int> RenderSquare(OverlayTile startTile, int range)
        {
            var possibleTileCoords = new List<Vector2Int>();

            for (var i = -range; i <= range; i++)
            {
                for (var j = -range; j <= range; j++)
                {
                    possibleTileCoords.Add(new Vector2Int(startTile.gridLocation.x + i, startTile.gridLocation.y + j));
                }
            }
            
            return possibleTileCoords;
        }
    }
}
