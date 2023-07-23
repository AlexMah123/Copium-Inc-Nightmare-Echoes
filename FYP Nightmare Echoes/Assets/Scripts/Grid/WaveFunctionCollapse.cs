using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NightmareEchoes.Grid
{
    public class WaveFunctionCollapse : MonoBehaviour
    {
        private int[,,] wave;
        private int[,] entropy;

        private List<TileData> tileList;

        void ObserveAndCollapse()
        {
            //Find lowest nonzero entropy
            var lowestEntropyCells = FindLowestEntropy(entropy, tileList.Count);
            
            //Pick random cell
            var cell = lowestEntropyCells[Random.Range(0, lowestEntropyCells.Count)];
            var cellX = cell[0];
            var cellZ = cell[1];
            
            //List valid patterns of cell
            List<int> validPatterns = new List<int>();
            for (var i = 0; i < wave.GetLength(2); i++)
            {
                if (wave[cellX, cellZ, i] == 1)
                    validPatterns.Add(i);
            }
            
            //Pick random valid (True) pattern
            var result = Random.Range(0, validPatterns.Count);
            
            //Set all patterns to false
            for (var j = 0; j < wave.GetLength(2); j++)
            {
                wave[cellX, cellZ, j] = 0;
            }
            
            //Set selected pattern to true
            wave[cellX, cellZ, result] = 1;

            //Update entropy
            entropy[cellX, cellZ] = 0;
        }

        void Propagate()
        {
            //Check neighbouring cells
            //Set false to non-valid patterns
            //Repeat until all neighbouring cells accounted for
        }

        private List<int[]> FindLowestEntropy(int[,] matrix, int numberOfPatterns)
        {
            //Find lowest entropy value;
            int lowestEntropy = numberOfPatterns;
            for (var x = 0; x < matrix.Rank; x++)
            {
                for (var z = 0; z < matrix.GetUpperBound(x); z++)
                {
                    if (matrix[x, z] >= lowestEntropy || matrix[x, z] == 0) continue;
                    lowestEntropy = matrix[x, z];
                }
            }
            
            //Return null if lowest is 0
            if (lowestEntropy == 0) return null;
            
            //Return all cells with lowest entropy
            var lowestEntropyCells = new List<int[]>();
            for (var x = 0; x < matrix.Rank; x++)
            {
                for (var z = 0; z < matrix.GetUpperBound(x); z++)
                {
                    if (matrix[x, z] != lowestEntropy) continue;
                    lowestEntropyCells.Add(new []{x,z});
                }
            }

            return lowestEntropyCells;
        }

        private static int CalculateTotalEntropy(int[,] matrix)
        {
            var entropySum = matrix.Cast<int>().Sum();

            return entropySum;
        }
        
        void Main(int x, int z, int n)
        {
            //Output dimensions' valid patterns (1=T/0=F)
            //False = pattern is no longer valid
            //n = number of patterns (placeholder rn)
            wave = new int[x, z, n];
            
            //Set all patterns to true
            for (var i = 0; i < x; i++)
            {
                for (var j = 0; j < z; j++)
                {
                    for (var k = 0; k < n; k++)
                    {
                        wave[i, j, k] = 1;
                    }
                }
            }
                
            //Output dimensions' entropy value
            //Higher entropy = more patterns
            entropy = new int[x, z];
            
            //Set all entropies to highest
            for (var a = 0; a < x; a++)
            {
                for (var b = 0; b < z; b++)
                {
                    entropy[a, b] = n;
                }
            }
            //##Init##
            //Define which tileset to use

            //##Core##

            while (CalculateTotalEntropy(entropy) > 0)
            {
                
            }
            /* While entropy > 0
             * Observe
             * Collapse
             * Propagate
             * Update entropies
             * Repeat
             */

        }


    }
}
