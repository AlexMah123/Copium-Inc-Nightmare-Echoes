using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NightmareEchoes.Grid;

//created by JH
namespace NightmareEchoes.Unit.Combat
{
    public class CombatManager : MonoBehaviour
    {
        public static CombatManager Instance;

        public List<BaseUnit> unitsInvolved;
        public List<BaseUnit> aliveUnits;
        public List<BaseUnit> deadUnits;
        
        public List<BaseUnit> friendlyUnits;
        public List<BaseUnit> aliveFriendlyUnits;
        public List<BaseUnit> deadFriendlyUnits;
        
        public List<BaseUnit> hostileUnits;
        public List<BaseUnit> aliveHostileUnits;
        public List<BaseUnit> deadHostileUnits;

        private Skill activeSkill;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;
        }

        private void Start()
        {
            OnBattleStart();
        }

        //Init
        void OnBattleStart()
        {
            unitsInvolved = FindObjectsOfType<BaseUnit>().ToList();
            aliveUnits = unitsInvolved;
            
            foreach (var unit in unitsInvolved)
            {
                if (unit.IsHostile)
                    hostileUnits.Add(unit);
                else
                    friendlyUnits.Add(unit);
            }

            aliveHostileUnits = hostileUnits;
            aliveFriendlyUnits = friendlyUnits;
            
            StartCoroutine(UpdateUnitPositionsAtStart());
        }
        
        //Player Calls
        public void SelectSkill(BaseUnit unit, Skill skill)
        {
            RenderOverlayTile.Instance.ClearRenders();
            
            if (activeSkill != null && activeSkill == skill)
            {
                activeSkill = null;
                return;
            }
            
            activeSkill = skill;

            RenderOverlayTile.Instance.RenderTiles(unit.ActiveTile, skill.targetArea.ToString(), skill.range);

            var list = aliveHostileUnits.Select(enemy => enemy.ActiveTile).ToList();
            RenderOverlayTile.Instance.RenderEnemyTiles(list);
        }


        //Validate
        
        //Logic
        
        //Apply
        
        //==Coroutines==
        IEnumerator UpdateUnitPositionsAtStart()
        {
            yield return new WaitForSeconds(1f);
            foreach (var unit in unitsInvolved)
            {
                unit.UpdateLocation();
            }
        }
    }
    
}
