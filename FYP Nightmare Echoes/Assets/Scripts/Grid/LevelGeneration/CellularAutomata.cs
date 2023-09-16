using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Grid
{
    public class CellularAutomata : MonoBehaviour
    {
        int[,] GenerateNoiseGrid(int density, int x, int z)
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
    }
}
