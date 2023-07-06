using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Grid
{
    public class WaveFunctionCollapse : MonoBehaviour
    {
        //Output dimensions' valid patterns (T/F)
        //False = pattern is no longer valid
        private bool[,] wave;
        
        //Output dimensions' entropy value
        //Higher entropy = more patterns
        private int[,] entropy;
        
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

        void Main()
        {
            //##Init##
            //Define which tileset to use
            //Define size of the map

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
