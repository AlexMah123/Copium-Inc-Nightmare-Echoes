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

        public bool turnEnded;
        
        private Skill activeSkill;
        private List<OverlayTile> skillRangeTiles;
        private List<OverlayTile> aoePreviewTiles = new();
        
        //Active AOEs
        private Dictionary<Skill, List<OverlayTile>> activeAoes = new();
        private Dictionary<Skill, int> activeAoesCD = new();

        private Camera cam;

        private bool secondaryTargeting;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;
            
            cam = Camera.main;
        }

        private void Start()
        {
            OnBattleStart();
        }

        private void Update()
        {
            if (activeSkill && !secondaryTargeting)
            {
                switch (activeSkill.TargetType)
                {
                    case TargetType.Single:
                        TargetUnit();
                        break;
                    case TargetType.AOE:
                        TargetGround();
                        break;
                }
            }
            Render();
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

        #region Logic Checks
        private void OnTurnStart()
        {
            
        }

        private void CheckAoe()
        {
            
        }
        
        private void EndTurn()
        {
            activeSkill.GetComponent<Units>().ShowPopUpText(activeSkill.SkillName);
            activeSkill.Reset();
            activeSkill = null;

            secondaryTargeting = false;
            
            RenderOverlayTile.Instance.ClearTargetingRenders();

            turnEnded = true;
        }
        #endregion

        #region Public Calls

        public void SelectSkill(Units unit, Skill skill)
        {
            //Clear Active Renders 
            RenderOverlayTile.Instance.ClearTargetingRenders();
            
            //Stop Coroutines
            StopAllCoroutines();

            secondaryTargeting = false;

            if (activeSkill != null)
            {
                activeSkill.StopAllCoroutines();
                activeSkill.Reset();
                if (activeSkill == skill)
                {
                    activeSkill = null;
                    return;
                }
            }
            
            activeSkill = skill;

            skillRangeTiles = CalculateRange(unit, skill);
        }

        public void SetActiveAoe(Skill skill, List<OverlayTile> tiles)
        {
            activeAoes.Add(skill, tiles);
            Debug.Log(tiles);
            activeAoesCD.Add(skill, skill.AoeDuration);
        }

        #endregion

        #region Targeting
        private void TargetUnit()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            
            var hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Unit"));
            if (!hit) return;
            var target = hit.collider.gameObject.GetComponent<Units>();
            if (!target) return;
            
            //Check if the enemy selected is in range
            //Originally it was a ForEach loop but Rider recommended this LINQ expression instead lmao
            if (skillRangeTiles.All(tile => tile != target.ActiveTile)) return;

            StartCoroutine(WaitForSkill(target));
        }

        private void TargetGround()
        {
            var hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Overlay Tile"));
            if (!hit) return;
            var target = hit.collider.gameObject.GetComponent<OverlayTile>();
            if (!target) return;
            if (skillRangeTiles.All(tile => tile != target)) return;
            
            var aoeArea = activeSkill.AoeType switch
            {
                AOEType.Square => SquareRange(target, 1),
                AOEType.Cross => LineRange(target, 1, false),
                AOEType.NonAOE => SquareRange(target, 0)
            };
            
            aoePreviewTiles.Clear();
            aoePreviewTiles.Add(target);
            foreach (var coord in aoeArea.Where(coord => OverlayTileManager.Instance.map.ContainsKey(coord)))
            {
                if (OverlayTileManager.Instance.map.TryGetValue(coord, out var tile))
                    aoePreviewTiles.Add(tile);
            }
            
            if (!Input.GetMouseButtonDown(0)) return;
            StartCoroutine(WaitForSkill(target, aoePreviewTiles));
        }
        
        public void SecondaryTargeting()
        {
            secondaryTargeting = true;
        }
        
        #endregion

        #region Rendering Tile Colors

        private void Render()
        {
            if (activeSkill)
            {
                RenderRangeAndUnits();
                if (activeSkill.TargetType == TargetType.AOE)
                    RenderAOETarget();
            }
            RenderActiveAoe();
        }
        
        //Attack Range and Units in Range
        private void RenderRangeAndUnits()
        {
            RenderOverlayTile.Instance.RenderAttackRangeTiles(skillRangeTiles);

            switch (activeSkill.TargetUnitAlignment)
            {
                case TargetUnitAlignment.Hostile:
                    RenderOverlayTile.Instance.RenderEnemyTiles(aliveHostileUnits.Select(enemy => enemy.ActiveTile).ToList());
                    break;
                case TargetUnitAlignment.Friendly:
                    RenderOverlayTile.Instance.RenderFriendlyTiles(aliveFriendlyUnits.Select(friendly => friendly.ActiveTile).ToList());
                    break;
                case TargetUnitAlignment.Both:
                    RenderOverlayTile.Instance.RenderEnemyTiles(aliveHostileUnits.Select(enemy => enemy.ActiveTile).ToList());
                    RenderOverlayTile.Instance.RenderFriendlyTiles(aliveFriendlyUnits.Select(friendly => friendly.ActiveTile).ToList());
                    break;
            }
        }
        
        //AOE Previews
        private void RenderAOETarget()
        {
            RenderOverlayTile.Instance.RenderAoeTiles(aoePreviewTiles);
        }
        
        //Active AOEs
        private void RenderActiveAoe()
        {
            foreach (var kvp in activeAoes)
            {
                RenderOverlayTile.Instance.RenderCustomColor(kvp.Value, kvp.Key.AoeColor);
                Debug.Log($"{kvp.Key} {kvp.Value.Count}");
            }
        }
        
        //Set Custom Range
        public void SetCustomRange(List<OverlayTile> tiles)
        {
            RenderOverlayTile.Instance.ClearTargetingRenders();
            skillRangeTiles = tiles;
        }

        #endregion
        
        #region Casting Range Calculation

        private List<OverlayTile> CalculateRange(Units unit, Skill skill)
        {
            var tileRange = new List<OverlayTile>();
            var possibleTileCoords = new List<Vector2Int>();

            var range = skill.Range;

            //Select Shape
            switch (skill.TargetArea.ToString())
            {
                case "Line":
                    possibleTileCoords = LineRange(unit.ActiveTile, range, false);
                    break;
                case "Square":
                    possibleTileCoords = SquareRange(unit.ActiveTile, range);
                    break;
                case "Crosshair":
                    possibleTileCoords = LineRange(unit.ActiveTile, range, true);
                    break;
                default:
                    Debug.LogWarning("ERROR");
                    break;
            }
            
            //Trim Out of Bounds
            tileRange = OverlayTileManager.Instance.TrimOutOfBounds(possibleTileCoords);

            return tileRange;
        }
        
        public List<Vector2Int> LineRange(OverlayTile startTile, int range, bool isCrosshair)
        {
            var possibleTileCoords = new List<Vector2Int>();
            var i = 1;
            if (isCrosshair) i = 2;
            for (; i <= range; i++)
            {
                possibleTileCoords.Add(new Vector2Int(startTile.gridLocation.x + i, startTile.gridLocation.y));
                possibleTileCoords.Add(new Vector2Int(startTile.gridLocation.x - i, startTile.gridLocation.y)); 
                possibleTileCoords.Add(new Vector2Int(startTile.gridLocation.x, startTile.gridLocation.y + i)); 
                possibleTileCoords.Add(new Vector2Int(startTile.gridLocation.x, startTile.gridLocation.y - i)); 
            }
            
            return possibleTileCoords;
        }
        
        public List<Vector2Int> SquareRange(OverlayTile startTile, int range)
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
        
        #endregion
        
        //==Coroutines==
        IEnumerator UpdateUnitPositionsAtStart()
        {
            yield return new WaitForSeconds(1f);
            foreach (var unit in unitsInvolved)
            {
                unit.UpdateLocation();
            }
        }

        IEnumerator WaitForSkill(Units target)
        {
            yield return new WaitUntil(() => activeSkill.Cast(target));
            EndTurn();
        }
        
        IEnumerator WaitForSkill(OverlayTile target, List<OverlayTile> aoeTiles)
        {
            yield return new WaitUntil(() => activeSkill.Cast(target, aoeTiles));
            EndTurn();
        }
    }
}
