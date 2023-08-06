using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NightmareEchoes.Unit.Pathfinding;

namespace NightmareEchoes.Unit.Combat
{
    public class CombatManager : MonoBehaviour
    {
        public PathfindingManager cursor;

        private Skill activeSkill;

        //Init
        void OnBattleStart()
        {
            //Get all units on field
            //Categorize into friendly / hostile
            //Log locations
        }
        
        //Player Calls
        public void SelectSkill(Skill skill)
        {
            activeSkill = skill;
        }


        //Validate
        
        //Logic
        
        //Apply
    }
    
}
