using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//created by Jian Hua, editted by Vinn
namespace NightmareEchoes.Grid
{
    public class TileMapManager : MonoBehaviour
    {
        [SerializeField] public int width;
        [SerializeField] public int length;
        
        [SerializeField] public TileBase testTile;
        
        public Tilemap tilemap;
        public Vector3Int TileLocation;
        private TilemapRenderer tilemapRenderer;
        
        private void Awake()
        {
            tilemap = GetComponent<Tilemap>();
            tilemapRenderer = GetComponent<TilemapRenderer>();
        }

        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            { 
                GetMouseTilePos();
            }
        }

        //Source: https://blog.unity.com/engine-platform/procedural-patterns-you-can-use-with-tilemaps-part-1
        public int[,] GenerateArray(int width, int length, bool empty)
        {
            var map = new int[width, length];
            for (var x = 0; x < map.GetUpperBound(0); x++)
            {
                for (var y = 0; y < map.GetUpperBound(1); y++)
                {
                    if (empty)
                    {
                        map[x, y] = 0;
                    }
                    else
                    {
                        map[x, y] = 1;
                    }
                }
            }
            return map;
        }
        
        public void RenderMap(int[,] map, Tilemap tilemap, TileBase tile)
        {
            //Clear the map (ensures we dont overlap)
            tilemap.ClearAllTiles();
            //Loop through the width of the map
            for (int x = 0; x < map.GetUpperBound(0) ; x++)
            {
                //Loop through the height of the map
                for (int y = 0; y < map.GetUpperBound(1); y++)
                {
                    // 1 = tile, 0 = no tile
                    if (map[x, y] == 1)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                    }
                }
            }
        }

        public void UpdateMap(int[,] map, Tilemap tilemap) //Takes in our map and tilemap, setting null tiles where needed
        {
            for (int x = 0; x < map.GetUpperBound(0); x++)
            {
                for (int y = 0; y < map.GetUpperBound(1); y++)
                {
                    //We are only going to update the map, rather than rendering again
                    //This is because it uses less resources to update tiles to null
                    //As opposed to re-drawing every single tile (and collision data)
                    if (map[x, y] == 0)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), null);
                    }
                }
            }
        }


        public void GetMouseTilePos()
        {
            Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            TileLocation = tilemap.WorldToCell(MousePos);

            if (tilemap.GetTile(TileLocation))
            {
                Debug.Log("Tile at" + MousePos);
            }
            else
            {
                Debug.Log("No tile at " + MousePos);
            }
        }
    }
}
