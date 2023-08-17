using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//created by Jian Hua, editted by Vinn and Terrence and Alex
namespace NightmareEchoes.Grid
{
    public class TileMapManager : MonoBehaviour
    {
        [Header("Singleton stuff")]
        public static TileMapManager Instance;

        [Header("Grid")]
        public int width;
        public int length;
        public TileBase testTile;
        public Tilemap tilemap;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {

        }

        public void Update()
        {
            
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
                    else if (!empty )
                    {
                        map[x, y] = 1;
                    }
                }
            }
            return map;
        }
        
        public void RenderMap(int[,] map, Tilemap tilemap, TileBase tile)
        {
            //Clear the map
            tilemap.ClearAllTiles();
            
            //Loop through the width of the map
            for (int x = 0; x < map.GetUpperBound(0) ; x++)
            {
                //Loop through the height of the map
                for (int y = 0; y < map.GetUpperBound(1); y++)
                {
                    var tilelocation = new Vector3Int(x,y,0);
                    // 1 = tile, 0 = no tile
                    if (map[x, y] == 1)
                    {
                        tilemap.SetTile(tilelocation, tile);
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
    }


}
