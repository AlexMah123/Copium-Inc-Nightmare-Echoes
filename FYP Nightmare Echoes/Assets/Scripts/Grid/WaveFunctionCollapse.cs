using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NightmareEchoes.Grid
{
    public class WaveFunctionCollapse : MonoBehaviour
    {
        void Observe()
        {
            //Find lowest nonzero entropy
            //Select random valid (True) pattern
        }

        void Collapse()
        {
            //Assign selected pattern
            //Set all patterns to false
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
            var wave = new int[x, z, n];
            
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
            var entropy = new int[x, z];
            
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
