using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using NightmareEchoes.TurnOrder;

//created by Jian Hua, editted by Vinn and Terrence and Alex
namespace NightmareEchoes.Grid
{
    public class TileMapManager : MonoBehaviour
    {
        [Header("Singleton stuff")]
        public static TileMapManager Instance;

        [Header("Grid")]
        [SerializeField] public int width;
        [SerializeField] public int length;
        [SerializeField] public TileBase testTile;

        
        public Tilemap tilemap;
        public Vector3Int TilePos;
        private TilemapRenderer tilemapRenderer;
        private Vector3 spawnPos;
        private Vector3 endPos;
        
        Vector3Int prevTilePos;

        [Header("Pathfinding")]
        [SerializeField] private GameObject StartPos;
        [SerializeField] private GameObject EndPos;
        [SerializeField] public GameObject spawnTest;

        public Vector3 SpawnPos
        {
            get => spawnPos;
            private set => spawnPos = value;
        }
      

        private void Awake()
        {
            tilemap = GetComponent<Tilemap>();
            tilemapRenderer = GetComponent<TilemapRenderer>();

            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
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
            if (UIManager.Instance.gameIsPaused)
            {
                return;
            }

            if (Input.GetMouseButtonDown(1))
            {
                //GetMouseTilePos2();
            }

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
            //Clear the map (ensures we dont overlap)
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

        public void GetMouseTilePos()
        {
            Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            TilePos = tilemap.WorldToCell(MousePos);


            if (tilemap.GetTile(TilePos))
            {
                if (prevTilePos != null)
                {
                    tilemap.SetTileFlags(prevTilePos, TileFlags.None);
                    tilemap.SetColor(prevTilePos, Color.white);
                }

                tilemap.SetTileFlags(TilePos, TileFlags.None);
                tilemap.SetColor(TilePos, Color.red);

                prevTilePos = TilePos;

                if (StartPos.activeSelf == true)
                {
                    spawnPos = new Vector3(tilemap.CellToLocal(TilePos).x, tilemap.CellToLocal(TilePos).y - 2, 1);
                    //StartPos.transform.position = spawnPos;
                    Debug.Log(" Mouse Click is on" + spawnPos);
                }
                else
                {
                    endPos = new Vector3(tilemap.CellToLocal(TilePos).x, tilemap.CellToLocal(TilePos).y - 2, 1);
                    /*StartPos.transform.position = spawnPos;*/
                    Debug.Log("Second Mouse Click is on" + spawnPos);
                }
            }
        }

        //this section by Terrence, spawning on tiles proof of concept
        public void GetMouseTilePos2()
        {

        }

    }


}
