using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace NightmareEchoes.Grid
{
    public class CellularAutomata : MonoBehaviour
    {
        [Header("Grid")]
        public int x;
        public int z;
        public int density;
        public TileBase testTile;
        public Tilemap tilemap;
        
        public int[,] GenerateNoiseGrid()
        {
            var grid = new int[x, z];
            for (var i = 0; i < x; i++)
            {
                for (var j = 0; j < z; j++)
                {
                    if (Random.Range(1, 101) > density)
                        grid[i, j] = 1;
                    else 
                        grid[i, j] = 0;
                }
            }
            return grid;
        }
        
        public void GenerateMap(int[,] map)
        {
            tilemap.ClearAllTiles();
            
            for (int x = 0; x <= map.GetUpperBound(0) ; x++)
            {
                for (int y = 0; y <= map.GetUpperBound(1); y++)
                {
                    var tilelocation = new Vector3Int(x, y, 0);
                    if (map[x, y] == 1)
                    {
                        tilemap.SetTile(tilelocation, testTile);
                    }
                }
            }
        }
    }
    
    [CustomEditor(typeof(CellularAutomata))]
    public class CellularAutomataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var generator = (CellularAutomata)target;

            if (GUILayout.Button("CreateMap"))
            {
                generator.GenerateMap(generator.GenerateNoiseGrid());
            }
        }
    }
}

