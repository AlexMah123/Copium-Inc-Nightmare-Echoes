using System.Collections;
using System.Collections.Generic;
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

        List<int[]> FindLowestEntropy(int[,] matrix, int numberOfPatterns)
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
            List<int[]> lowestEntropyCells = new List<int[]>();
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
        
        void Main(int x, int z)
        {
            //Output dimensions' valid patterns (T/F)
            //False = pattern is no longer valid
            bool[] wave = new bool[10];
        
            //Output dimensions' entropy value
            //Higher entropy = more patterns
            int[,] entropy = new int[x, z];
 
            //##Init##
            //Define which tileset to use
            //Set all waves to True
            //Set all entropies to highest

            //##Core##
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
