using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NightmareEchoes.Unit;
using System.Linq;

namespace NightmareEchoes.AI
{
    public class BaseAI : MonoBehaviour
    {
        [SerializeField] List<BaseUnit> heroList;
        BaseUnit closestHero;
        bool inAtkRange;
        bool inMoveAtkRange;

        private void Start()
        {

        }
        public void MakeDecision()
        {
            if (inAtkRange)
            {
                //retreat if ranged, attack closestHero
            }
            else if (inMoveAtkRange)
            {
                //move, attack hero
            }
            else
            {
                //move towards closest hero
            }
        }
        void SortHeroes()
        {
            heroList = FindObjectsOfType<BaseUnit>().ToList();
            //filter by heroes


        }
    }

}
