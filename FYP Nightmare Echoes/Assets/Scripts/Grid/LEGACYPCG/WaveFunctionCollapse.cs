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
        private int xSize, zSize;
        private bool[,] collapsed;
        private int[,,] wave;
        private int[,] entropy;

        private List<TileData> tileList;

        //Stack to check for neighbouring cells
        Stack<int[]> neighbours = new Stack<int[]>();
        
        private static readonly int[,] cardinals = 
        {   { 1, 0 },       //N 
            { 0, 1 },       //E
            { -1, 0 },      //S
            { 0, -1 } };    //W
        
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
            collapsed[cellX, cellZ] = true;
            
            Propagate(cell);
        }

        void Propagate(int[] cell)
        {
            //Add valid neighbours to stack
            for (var i = 0; i < cardinals.GetLength(0); i++)
            {
                var neighbour = new[] {cell[0] + cardinals[i,0], cell[1] + cardinals[i, 1]};
                //Check for out of bounds
                if (neighbour[0] < 0 || neighbour[1] < 0 || neighbour[0] >= xSize || neighbour[1] >= zSize)
                    continue;
                
                //Add to stack if not collapsed
                if (!collapsed[neighbour[0],neighbour[1]])
                    neighbours.Push(neighbour);
            }

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
            xSize = x;
            zSize = z;
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
            
            //Output dimensions' collapsed state
            //True => collapsed
            collapsed = new bool[x, z];

            //Set all entropies to highest
            for (var a = 0; a < x; a++)
            {
                for (var b = 0; b < z; b++)
                {
                    entropy[a, b] = n;
                    collapsed[a, b] = false;
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
