using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NightmareEchoes.Grid;

namespace NightmareEchoes.Unit.Combat
{
    public class CombatManager : MonoBehaviour
    {
        public static CombatManager Instance;
        
        private Skill activeSkill;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;
        }

        //Init
        void OnBattleStart()
        {
            //Get all units on field
            //Categorize into friendly / hostile
            //Log locations
        }
        
        //Player Calls
        public void SelectSkill(BaseUnit unit, Skill skill)
        {
            RenderOverlayTile.Instance.ClearRenders();
            
            if (activeSkill != null && activeSkill == skill)
            {
                //RenderOverlayTile.ClearRenders();
                activeSkill = null;
                return;
            }
            
            activeSkill = skill;

            string targetRange = null;

            if (skill.targetType == TargetType.Single)
            {
                targetRange = "Line";
            }
            RenderOverlayTile.Instance.RenderTiles(unit.ActiveTile, targetRange, skill.range);
        }


        //Validate
        
        //Logic
        
        //Apply
    }
    
}
