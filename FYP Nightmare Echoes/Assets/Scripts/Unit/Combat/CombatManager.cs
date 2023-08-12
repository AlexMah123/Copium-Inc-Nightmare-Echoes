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

        public List<Units> unitsInvolved;
        public List<Units> aliveUnits;
        public List<Units> deadUnits;
        
        public List<Units> friendlyUnits;
        public List<Units> aliveFriendlyUnits;
        public List<Units> deadFriendlyUnits;
        
        public List<Units> hostileUnits;
        public List<Units> aliveHostileUnits;
        public List<Units> deadHostileUnits;

        private Skill activeSkill;
        private List<OverlayTile> skillRangeTiles;

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

        private void Update()
        {
            TargetUnit();
        }

        //Init
        void OnBattleStart()
        {
            unitsInvolved = FindObjectsOfType<Units>().ToList();
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
        public void SelectSkill(Units unit, Skill skill)
        {
            //Clear Active Renders
            RenderOverlayTile.Instance.ClearRenders();
            
            if (activeSkill != null && activeSkill == skill)
            {
                activeSkill = null;
                return;
            }
            
            activeSkill = skill;
            
            //Getting Attack Ranges
            var tileRange = new List<OverlayTile> { unit.ActiveTile };
            var possibleTileCoords = new List<Vector2Int>();
            
            //Select Shape
            switch (skill.TargetArea.ToString())
            {
                case "Line":
                    possibleTileCoords = LineRange(unit.ActiveTile, skill.Range);
                    break;
                case "Square":
                    possibleTileCoords = SquareRange(unit.ActiveTile, skill.Range);
                    break;
                default:
                    Debug.LogWarning("ERROR");
                    break;
            }
            
            //Trim Out of Bounds
            var map = OverlayTileManager.Instance.map;
            foreach (var coord in possibleTileCoords.Where(coord => map.ContainsKey(coord)))
            {
                if (OverlayTileManager.Instance.map.TryGetValue(coord, out var tile))
                    tileRange.Add(tile);
            }
            
            skillRangeTiles = tileRange;
            
            //Render Range and Units in Range
            RenderOverlayTile.Instance.RenderTiles(tileRange);
            var list = aliveHostileUnits.Select(enemy => enemy.ActiveTile).ToList();
            RenderOverlayTile.Instance.RenderEnemyTiles(list);
        }

        private void TargetUnit()
        {
            if (!activeSkill) return;
            if (!Input.GetMouseButtonDown(0)) return;
            
            var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Unit"));
            if (!hit) return;
            var target = hit.collider.gameObject.GetComponent<Units>();
            if (!target) return;
            
            //Check if the enemy selected is in range
            //Originally it was a ForEach loop but Rider recommended this LINQ expression instead lmao
            if (skillRangeTiles.All(tile => tile != target.ActiveTile)) return;
            activeSkill.Cast(target);
            activeSkill = null;
            
            RenderOverlayTile.Instance.ClearRenders();
        }
        
        //==Cast Ranges==
        private List<Vector2Int> LineRange(OverlayTile startTile, int range)
        {
            var possibleTileCoords = new List<Vector2Int>();
            for (var i = 1; i <= range; i++)
            {
                possibleTileCoords.Add(new Vector2Int(startTile.gridLocation.x + i, startTile.gridLocation.y));
                possibleTileCoords.Add(new Vector2Int(startTile.gridLocation.x - i, startTile.gridLocation.y)); 
                possibleTileCoords.Add(new Vector2Int(startTile.gridLocation.x, startTile.gridLocation.y + i)); 
                possibleTileCoords.Add(new Vector2Int(startTile.gridLocation.x, startTile.gridLocation.y - i)); 
            }
            
            return possibleTileCoords;
        }
        
        private List<Vector2Int> SquareRange(OverlayTile startTile, int range)
        {
            var possibleTileCoords = new List<Vector2Int>();

            for (var i = -range; i <= range; i++)
            {
                for (var j = -range; j <= range; j++)
                {
                    possibleTileCoords.Add(new Vector2Int(startTile.gridLocation.x + i, startTile.gridLocation.y + j));
                }
            }
            return possibleTileCoords;
        }
        
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
