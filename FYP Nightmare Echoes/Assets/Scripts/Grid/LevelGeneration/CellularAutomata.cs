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
        public int iterations;
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

        public int[,] ApplyCellularAutomata(int[,] map)
        {
            for (var i = 1; i <= iterations; i++)
            {
                var tempMap = map;
                for (var j = 0; j < this.x; j++)
                {
                    for (var k = 0; k < this.z; k++)
                    {
                        var neighbourWallCount = 0;
                        for (var x = j - 1; x <= j + 1; x++)
                        {
                            for (var z = k - 1; z <= k + 1; z++)
                            {
                                if (x >= 0 && x < this.x && z >= 0 && z < this.z)
                                {
                                    if (x == j && z == k) continue;
                                    if (tempMap[x, z] == 0)
                                        neighbourWallCount++;
                                }
                                else
                                    neighbourWallCount++;
                            }
                        }
                        
                        if (neighbourWallCount > 4)
                            map[j, k] = 0;
                        else
                            map[j, k] = 1;
                    }
                }
            }
            return map;
        }
        
        public void GenerateMap(int[,] map)
        {
            tilemap.ClearAllTiles();
            
            for (int x = 0; x <= map.GetUpperBound(0) ; x++)
            {
                for (int z = 0; z <= map.GetUpperBound(1); z++)
                {
                    var tilelocation = new Vector3Int(x, z, 0);
                    if (map[x, z] == 1)
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
                var map = generator.GenerateNoiseGrid();
                map = generator.ApplyCellularAutomata(map);
                generator.GenerateMap(map);
            }
        }
    }
}

