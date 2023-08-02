using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace NightmareEchoes.Pathfinding
{
    public class OverlayTileManager : MonoBehaviour
    {
        private int childCount = 0;
        public static OverlayTileManager Instance;

        public OverlayTile overlaytilePrefab;
        public GameObject overlayContainer;

        //Dicitonary to store overlay tile position from grid pos
        public Dictionary<Vector2Int, OverlayTile> map;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }

            InitOverlayTiles();
        }

        void Start()    
        {

        }


        void InitOverlayTiles()
        {
            //generating the grid
            var tileMap = gameObject.GetComponentInChildren<Tilemap>();
            map = new Dictionary<Vector2Int, OverlayTile>();
            //getting the bounds of the map
            BoundsInt bounds = tileMap.cellBounds;

            //For looping to get the bounds of our map (Starting with Z first as its height then x or y either does not matter) (Z-- since its going down)
            for (int z = bounds.max.z; z >= bounds.min.z; z--)
            {
                for (int y = bounds.min.y; y < bounds.max.y; y++)
                {
                    for (int x = bounds.min.x; x < bounds.max.x; x++)
                    {
                        //Capturing tile location
                        var tileLocation = new Vector3Int(x, y, z);
                        var TileKey = new Vector2Int(x, y);

                        //Make sure the tilemap has the tile we are looking for
                        if (tileMap.HasTile(tileLocation) && !map.ContainsKey(TileKey))
                        {
                            //Instantiating the tileprefab over the grid and storing the position in the girdContainer
                            var overlayTile = Instantiate(overlaytilePrefab, overlayContainer.transform);
                            overlayTile.name = $"{childCount} {tileLocation}";
                            //Getting WorldPos (Vector 3) to Tile Position (Vector3int)
                            var cellWorldPos = tileMap.GetCellCenterWorld(tileLocation);

                            //Placing overlay tile to where they are supposed to be
                            //Instead of setting the position straigh tto the cellWorldPos we are offsetting it to one cell higher on the z axis so it can be seen on screen
                            overlayTile.transform.position = new Vector3(cellWorldPos.x, cellWorldPos.y, cellWorldPos.z);
                            overlayTile.GetComponent<SpriteRenderer>().sortingOrder = tileMap.GetComponent<TilemapRenderer>().sortingOrder;
                            overlayTile.gridLocation = tileLocation;
                            childCount++;
                            map.Add(TileKey, overlayTile);
                        }
                    }
                }
            }
        }


        public List<OverlayTile> GetNeighbourTiles(OverlayTile currentOverlayTile)
        {
            var map = OverlayTileManager.Instance.map;

            List<OverlayTile> neighbours = new List<OverlayTile>();

            //Top
            Vector2Int LocToCheck = new Vector2Int(currentOverlayTile.gridLocation.x, currentOverlayTile.gridLocation.y + 1);

            if (map.ContainsKey(LocToCheck))
            {
                neighbours.Add(map[LocToCheck]);
            }

            //Bottom
            LocToCheck = new Vector2Int(currentOverlayTile.gridLocation.x, currentOverlayTile.gridLocation.y - 1);

            if (map.ContainsKey(LocToCheck))
            {
                neighbours.Add(map[LocToCheck]);
            }

            //Right
            LocToCheck = new Vector2Int(currentOverlayTile.gridLocation.x + 1, currentOverlayTile.gridLocation.y);

            if (map.ContainsKey(LocToCheck))
            {
                neighbours.Add(map[LocToCheck]);
            }

            //Left
            LocToCheck = new Vector2Int(currentOverlayTile.gridLocation.x - 1, currentOverlayTile.gridLocation.y);

            if (map.ContainsKey(LocToCheck))
            {
                neighbours.Add(map[LocToCheck]);
            }

            return neighbours;
        }
    }
}

