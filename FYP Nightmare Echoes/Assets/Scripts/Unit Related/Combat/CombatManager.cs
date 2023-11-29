using NightmareEchoes.Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.VFX;

//created by JH, edited by Ter
namespace NightmareEchoes.Unit.Combat
{
    public class CombatManager : MonoBehaviour
    {
        public static CombatManager Instance;

        public List<Entity> unitsInvolved = new ();
        public List<Entity> friendlyUnits = new ();
        public List<Entity> hostileUnits = new();

        [SerializeField] private List<Skill> friendlySkills = new List<Skill>();

        public bool turnEnded;
        
        [SerializeField] private Skill activeSkill;
        private List<OverlayTile> skillRangeTiles = new();
        private List<OverlayTile> aoePreviewTiles = new();
        private OverlayTile mainTile;
        
        //Ghosts
        private List<GameObject> ghostSprites = new();
        private List<OverlayTile> ghostTiles = new();

        //Active AOEs
        private Dictionary<Skill, List<OverlayTile>> activeAoes = new();
        private Dictionary<Skill, int> activeAoesCD = new();
        
        //Active Traps
        private OrderedDictionary activeTraps = new();

        private Camera cam;

        private bool secondaryTargeting;

        //to check if skill is casting
        public bool skillIsCasting = false;
        public bool lockInput = false;

        //used for end of turn
        private Direction chosenDirection;

        //To prevent update() from casting multiple times
        //Not used at the moment
        private bool castGate = false;
        
        //Movement Tiles to Render
        public List<OverlayTile> movementTiles = new();

        #region Properties
        public Skill ActiveSkill
        {
            get => activeSkill;
            set => activeSkill = value;
        }

        public Dictionary<Skill, List<OverlayTile>> ActiveAoes
        {
            get => activeAoes;
            set => activeAoes = value;
        }
        #endregion

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
            //only render when not paused
            if (Time.timeScale == 0) return;


            if (activeSkill && !secondaryTargeting)
            {
                if (!activeSkill.Placable)
                {
                    switch (activeSkill.TargetType)
                    {
                        case TargetType.Single:
                            TargetUnit();
                            break;
                        case TargetType.AOE:
                            TargetGround();
                            break;
                        case TargetType.Self:
                            TargetSelf();
                            break;
                    }
                }
            }
            Render();
        }

        //Init
        public void OnBattleStart()
        {
            unitsInvolved = FindObjectsOfType<Entity>().ToList();

            var iterator = new List<Entity>(unitsInvolved);

            /*foreach (var unit in iterator.Where(unit => unit.IsProp))
            {
                unitsInvolved.Remove(unit);
            }*/

            foreach (var unit in unitsInvolved)
            {
                if (unit.IsHostile)
                    hostileUnits.Add(unit);
                else
                    friendlyUnits.Add(unit);
            }

            foreach (var entity in friendlyUnits)
            {
                var skills = entity.gameObject.GetComponents<Skill>();
                friendlySkills.AddRange(skills);
            }

            StartCoroutine(UpdateUnitPositionsAtStart());
        }

        #region Logic Checks
        public void OnTurnStart()
        {
            foreach (var kvp in activeAoesCD.ToList())
            {
                activeAoesCD[kvp.Key]--;
                if (kvp.Value <= 0)
                {
                    activeAoes.TryGetValue(kvp.Key, out var tilesToBeCleared);
                    activeAoesCD.Remove(kvp.Key);
                    activeAoes.Remove(kvp.Key);

                    RenderOverlayTile.Instance.ClearCustomRenders(tilesToBeCleared);
                }
                else if (activeAoes.TryGetValue(kvp.Key, out var list))
                {
                    foreach (var tile in list.Where(tile => tile.CheckEntityGameObjectOnTile()))
                    {
                        kvp.Key.Cast(tile.CheckEntityGameObjectOnTile().GetComponent<Entity>());
                    }
                }
            }
        }

        //Check which tiles the unit passes
        //If the tile is an AOE tile, return the skill it is associated with
        public Skill CheckAoe(Entity unit)
        {
            var hit = Physics2D.Raycast(unit.transform.position, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Overlay Tile"));
            if (!hit) return null;
            var target = hit.collider.gameObject.GetComponent<OverlayTile>();
            
            foreach (var kvp in activeAoes)
            {
                foreach (var tile in kvp.Value)
                {
                    if (target == tile)
                    {
                        return kvp.Key;
                    }
                }
            }

            return null;
        }

        public Skill CheckTrap(Entity unit)
        {
            if (!unit.IsHostile) 
                return null;

            var hit = Physics2D.Raycast(unit.transform.position, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Overlay Tile"));

            if (!hit) 
                return null;

            var target = hit.collider.gameObject.GetComponent<OverlayTile>();
            var trap = target.CheckTrapOnTile();

            if (!trap) 
                return null;

            var enumerator = activeTraps.GetEnumerator(); 

            while (enumerator.MoveNext())
            {
                if ((GameObject)enumerator.Key != trap) 
                    continue;

                var activatedTrap = enumerator.Key;
                activeTraps.Remove(enumerator.Key);
                Destroy((GameObject)activatedTrap);
                return enumerator.Value as Skill;
            }

            return null;
        }
        
        private void EndTurn()
        {
            activeSkill.CheckCooldown(true);
            activeSkill.Reset();
            activeSkill = null;
            skillIsCasting = false;

            secondaryTargeting = false;
            
            RenderOverlayTile.Instance.ClearTargetingRenders();
            ClearPreviews();
            RemoveDeadUnits();
            
            turnEnded = true;
        }

        public void IncrementCoolDowns()
        {
            foreach (var skill in friendlySkills)
            {
                skill.CheckCooldown(false);
            }
        }
        #endregion

        #region Public Calls

        public void SelectSkill(Entity unit, Skill skill)
        {
            if (skill.OnCooldown)
            {
                unit.ShowPopUpText("Skill on cooldown", Color.yellow);
                return;
            }
            
            //Clear Active Renders 
            RenderOverlayTile.Instance.ClearTargetingRenders();
            ClearPreviews();
            
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
            
            if (activeSkill.Placable)
            {
                StartCoroutine(PlaceTraps());
            }
        }

        public void SetActiveAoe(Skill skill, List<OverlayTile> tiles)
        {
            //Check for dupe (usually wont happen as skill cd is longer than aoe duration but just in case)
            if (activeAoes.TryGetValue(skill, out var list))
            {
                RenderOverlayTile.Instance.ClearCustomRenders(list);
                activeAoes[skill] = tiles;
                activeAoesCD[skill] = skill.AoeDuration;
            }
            else
            {
                activeAoes.Add(skill, tiles);
                activeAoesCD.Add(skill, skill.AoeDuration);
            }
        }

        public void ClearActiveAoe(Skill skill)
        {
            if (activeAoes.TryGetValue(skill, out var list))
            {
                RenderOverlayTile.Instance.ClearCustomRenders(list);
                activeAoes.Remove(skill);
                activeAoesCD.Remove(skill);
            }
        }

        #endregion

        #region Targeting
        private void TargetUnit()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            
            var hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Entity"));
            if (!hit) return;
            var target = hit.collider.gameObject.GetComponent<Entity>();
            if (!target) return;
            
            //Check if the enemy selected is in range
            //Originally it was a ForEach loop but Rider recommended this LINQ expression instead lmao
            if (skillRangeTiles.All(tile => tile != target.ActiveTile)) return;
            
            if (activeSkill.InflictKnockback)
            {
                PreviewKnockback();
            }

            StartCoroutine(WaitForSkill(target));
        }

        private void TargetGround()
        {
            aoePreviewTiles.Clear();
            ClearPreviews();

            var target = OverlayTileManager.Instance.GetOverlayTileOnMouseCursor();
            if (!target) return;
            
            mainTile = target;
            if (skillRangeTiles.All(tile => tile != target)) return;

            if (activeSkill.AoeOffset > 0)
            {
                var parent = activeSkill.GetComponentInParent<Entity>();
                var offsetVector = parent.ActiveTile.gridLocation + (target.gridLocation - parent.ActiveTile.gridLocation) * activeSkill.AoeOffset;
                target = OverlayTileManager.Instance.GetOverlayTile(new Vector2Int(offsetVector.x, offsetVector.y));
                if (target == null)
                {
                    target = GetOverlayTileClone();
                    var cellWorldPos = OverlayTileManager.Instance.GetComponentInChildren<Tilemap>().GetCellCenterWorld(offsetVector);
                    target.transform.position = new Vector3(cellWorldPos.x, cellWorldPos.y, cellWorldPos.z);
                    target.gridLocation = offsetVector;
                }
            }
            
            var aoeArea = activeSkill.AoeType switch
            {
                AOEType.Square => SquareRange(target, activeSkill.AoeSize),
                AOEType.Cross => LineRange(target, activeSkill.AoeSize, false),
                AOEType.NonAOE => SquareRange(target, 0)
            };
            
            foreach (var coord in aoeArea.Where(coord => OverlayTileManager.Instance.gridDictionary.ContainsKey(coord)))
            {
                if (OverlayTileManager.Instance.gridDictionary.TryGetValue(coord, out var tile))
                    aoePreviewTiles.Add(tile);
            }

            if (activeSkill.InflictKnockback)
            {
                PreviewKnockback();
            }

            if (!Input.GetMouseButtonDown(0)) return;

            StartCoroutine(WaitForSkill(target, aoePreviewTiles));
        }

        private void TargetSelf()
        {
            if (!Input.GetMouseButtonDown(0)) return;

            var hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Entity"));
            if (!hit) return;
            var target = hit.collider.gameObject.GetComponent<Entity>();
            if (!target) return;

            if (!target == activeSkill.gameObject.GetComponent<Entity>()) return;


            if (activeSkill.Cast())
                EndTurn();
        }
        public void SecondaryTargeting()
        {
            secondaryTargeting = true;
        }

        #endregion

        #region Enemy Targeting
        public IEnumerator EnemyTargetUnit(Entity target, Skill skill)
        {
            var targetPosition = target.transform.position;
            
            activeSkill = skill;
            activeSkill.Cast(target);
            
            if (activeSkill.VFXGraph)
            {
                StartCoroutine(PlayVFX(activeSkill.VFXGraph, targetPosition));
            }

            #region Animations
            RenderOverlayTile.Instance.ClearTargetingRenders();
            var activeUnit = activeSkill.gameObject.GetComponent<Entity>();

            yield return WaitForAnimationCompletion(activeUnit);
            #endregion

            EndTurn();
            yield break;
        }

        public IEnumerator EnemyTargetGround(OverlayTile targetTile, Skill skill)
        {
            activeSkill = skill;

            var aoeArea = activeSkill.AoeType switch
            {
                AOEType.Square => SquareRange(targetTile, 1, false),
                AOEType.Cross => LineRange(targetTile, 1, false),
                AOEType.NonAOE => SquareRange(targetTile, 0)
            };

            aoePreviewTiles.Add(targetTile);
            foreach (var coord in aoeArea.Where(coord => OverlayTileManager.Instance.gridDictionary.ContainsKey(coord)))
            {
                if (OverlayTileManager.Instance.gridDictionary.TryGetValue(coord, out var tile))
                    aoePreviewTiles.Add(tile);
            }

            activeSkill.Cast(targetTile, aoePreviewTiles);
            
            if (activeSkill.VFXGraph)
            {
                StartCoroutine(PlayVFX(activeSkill.VFXGraph, targetTile.transform.position));
            }

            #region Animations
            RenderOverlayTile.Instance.ClearTargetingRenders();
            var activeUnit = activeSkill.gameObject.GetComponent<Entity>();

            yield return WaitForAnimationCompletion(activeUnit);
            #endregion

            EndTurn();
            yield break;
        }

        #endregion

        #region Rendering Tile Colors and Previews

        private void Render()
        {
            RenderActiveAoe();
            if (activeSkill)
            {
                RenderRangeAndUnits();
                if (activeSkill.TargetType == TargetType.AOE)
                    RenderAOETarget();
            }
            RenderMovementTiles();
        }
        
        //Attack Range and Units in Range
        private void RenderRangeAndUnits()
        {
            RenderOverlayTile.Instance.RenderAttackRangeTiles(skillRangeTiles);

            switch (activeSkill.TargetUnitAlignment)
            {
                case TargetUnitAlignment.Hostile:
                    RenderOverlayTile.Instance.RenderEnemyTiles(hostileUnits.Select(enemy => enemy.ActiveTile).ToList());
                    break;
                case TargetUnitAlignment.Friendly:
                    RenderOverlayTile.Instance.RenderFriendlyTiles(friendlyUnits.Select(friendly => friendly.ActiveTile).ToList());
                    break;
                case TargetUnitAlignment.Both:
                    RenderOverlayTile.Instance.RenderEnemyTiles(hostileUnits.Select(enemy => enemy.ActiveTile).ToList());
                    RenderOverlayTile.Instance.RenderFriendlyTiles(friendlyUnits.Select(friendly => friendly.ActiveTile).ToList());
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
            }
        }

        public void RenderMovementTiles(bool clear = false)
        {
            if (clear)
            {
                foreach (var tile in movementTiles)
                {
                    tile.HideTile();
                }
                movementTiles.Clear();
                
                return;
            }
            
            foreach (var tile in movementTiles)
            {
                tile.ShowMoveTile();
            }
        }

        //Set Custom Range
        public void SetCustomRange(List<OverlayTile> tiles)
        {
            RenderOverlayTile.Instance.ClearTargetingRenders();
            skillRangeTiles = tiles;
        }
        
        //Previews
        public void ClearPreviews()
        {
            foreach (var go in ghostSprites)
            {
                go.SetActive(false);
            }

            foreach (var tile in ghostTiles)
            {
                tile.gameObject.SetActive(false);
            }
            ghostSprites.Clear();
            ghostTiles.Clear();
        }
        
        private void PreviewKnockback()
        {
            foreach (var tile in aoePreviewTiles)
            {
                if (tile == mainTile) continue;
                if (!tile.CheckEntityGameObjectOnTile()) continue;

                var direction = tile.transform.position - mainTile.transform.position;
                var destination = tile.transform.position + direction;
                var destinationTile = OverlayTileManager.Instance.GetOverlayTileInWorldPos(destination);
                
                if (!destinationTile) continue;
                if (destinationTile.CheckEntityGameObjectOnTile() || destinationTile.CheckObstacleOnTile()) continue;

                var entity = tile.CheckEntityGameObjectOnTile();
                var entitySr = entity.GetComponent<SpriteRenderer>();
                var clone = GetClone(tile.CheckEntityGameObjectOnTile());
                var cloneSr = clone.GetComponent<SpriteRenderer>();

                //only for models, re-enable all the child objects
                foreach (Transform transform in clone.transform)
                {
                    transform.gameObject.SetActive(true);
                }

                clone.transform.localScale = entity.transform.localScale;
                cloneSr.sprite = entitySr.sprite;
                var entityColor = entitySr.color;
                cloneSr.color = new Color(entityColor.r, entityColor.g, entityColor.b, 0.5f);
                cloneSr.sortingLayerID = SortingLayer.NameToID("Entity");
                ghostSprites.Add(clone);
                
                clone.SetActive(true);
                clone.transform.position = destinationTile.transform.position;
            }
        }

        #endregion 
        
        #region Casting Range Calculation

        private List<OverlayTile> CalculateRange(Entity unit, Skill skill)
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
                    possibleTileCoords = SquareRange(unit.ActiveTile, range, false);
                    break;
                case "SquareGap":
                    possibleTileCoords = SquareRange(unit.ActiveTile, range, true);
                    break;
                case "Crosshair":
                    possibleTileCoords = LineRange(unit.ActiveTile, range, true);
                    break;
                case "FrontalAttack":
                    possibleTileCoords = FrontalRange(unit.ActiveTile, range , unit);
                    break;
                case "Diamond":
                    possibleTileCoords = DiamondRange(unit.ActiveTile, range, false);
                    break;
                case "DiamondGap":
                    possibleTileCoords = DiamondRange(unit.ActiveTile, range, true);
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
        
        public List<Vector2Int> SquareRange(OverlayTile startTile, int range, bool gap = false)
        {
            var possibleTileCoords = new List<Vector2Int>();

            for (var i = -range; i <= range; i++)
            {
                for (var j = -range; j <= range; j++)
                {
                    possibleTileCoords.Add(new Vector2Int(startTile.gridLocation.x + i, startTile.gridLocation.y + j));
                }
            }

            if (!gap) return possibleTileCoords;

            var startCoord = (Vector2Int) startTile.gridLocation;
            var list = new List<Vector2Int>
            {
                startCoord + Vector2Int.right, startCoord + Vector2Int.left, startCoord + Vector2Int.up, startCoord + Vector2Int.down,
                startCoord + Vector2Int.right + Vector2Int.up, startCoord + Vector2Int.right + Vector2Int.down, startCoord + Vector2Int.left + Vector2Int.up, startCoord + Vector2Int.left + Vector2Int.down,
                startCoord
            };
            
            var copy = new List<Vector2Int>(possibleTileCoords);
            foreach (var coord in from coord in copy from c in list where coord == c select coord)
            {
                possibleTileCoords.Remove(coord);
            }
            
            return possibleTileCoords;
        }

        public List<Vector2Int> FrontalRange(OverlayTile startTile, int range , Entity unit)
        {
            var possibleTileCoords = new List<Vector2Int>();

            var direction = new Vector2Int();

            switch (unit.Direction)
            {
                case Direction.NORTH:
                    direction = new Vector2Int(1, 0);
                    break;
                case Direction.SOUTH:
                    direction = new Vector2Int(-1,0);
                    break;
                case Direction.EAST:
                    direction = new Vector2Int(0,-1);
                    break;
                case Direction.WEST:
                    direction = new Vector2Int(0,1);
                    break;  
            }

            var frontalTile = new Vector2Int(startTile.gridLocation.x + direction.x , startTile.gridLocation.y + direction.y);
            possibleTileCoords.Add(frontalTile);


            for (var i = 1; i <= range; i++)
            {
                if (unit.Direction == Direction.NORTH || unit.Direction == Direction.SOUTH)
                {
                    possibleTileCoords.Add(frontalTile + new Vector2Int(0, i));
                    possibleTileCoords.Add(frontalTile - new Vector2Int(0, i));
                }
                if (unit.Direction == Direction.EAST || unit.Direction == Direction.WEST)
                {
                    possibleTileCoords.Add(frontalTile + new Vector2Int(i, 0));
                    possibleTileCoords.Add(frontalTile - new Vector2Int(i, 0));
                }
            }
            return possibleTileCoords;
        }

        public List<Vector2Int> DiamondRange(OverlayTile startTile, int range, bool gap = false)
        {
            var possibleTileCoords = new List<Vector2Int>();
            var startCoord = (Vector2Int)startTile.gridLocation;
            possibleTileCoords.Add(startCoord);
            
            var copyList = new List<Vector2Int>(possibleTileCoords);
            var newCoords = new List<Vector2Int>();
            for (var i = 0; i < range; i++)
            {
                foreach (var v in copyList)
                {
                    newCoords.Add(v + Vector2Int.right);
                    newCoords.Add(v + Vector2Int.left); 
                    newCoords.Add(v + Vector2Int.up); 
                    newCoords.Add(v + Vector2Int.down); 
                }
                
                possibleTileCoords.AddRange(newCoords);
                copyList.Clear();
                
                copyList.AddRange(newCoords);
                newCoords.Clear();
            }

            if (!gap) return possibleTileCoords;
            
            var n = startCoord + Vector2Int.right;
            var s = startCoord + Vector2Int.left;
            var e = startCoord + Vector2Int.up;
            var w = startCoord + Vector2Int.down;

            var copy = new List<Vector2Int>(possibleTileCoords);
            foreach (var coord in copy.Where(coord => coord == n || coord == s || coord == e || coord == w || coord == startCoord))
            {
                possibleTileCoords.Remove(coord);
            }

            return possibleTileCoords;
        }

        #endregion

        #region Misc Functions
        public List<Entity> IsStealthUnitInViewRange(Entity thisUnit, int range)
        {
            //declare variables and reset
            List<Entity> herosInStealth = new List<Entity>();

            var overlayTileInFront = new List<OverlayTile>(OverlayTileManager.Instance.TrimOutOfBounds(FrontalRange(thisUnit.ActiveTile, range, thisUnit)));

            foreach (var tile in overlayTileInFront)
            {
                //for each overlayTile in front, check if the tiles have units that are not hostile (hero)
                if (!tile.CheckEntityGameObjectOnTile()) 
                    continue;

                var entity = tile.CheckEntityGameObjectOnTile().GetComponent<Entity>();

                if (entity.IsHostile || entity.IsProp) 
                    continue;
                
                if (entity.DoesModifierExist(STATUS_EFFECT.STEALTH_TOKEN))
                    herosInStealth.Add(entity);
            }

            return herosInStealth;
        }

        public void RemoveDeadUnits()
        {
            unitsInvolved.RemoveAll(unit => unit == null);
            friendlyUnits.RemoveAll(unit => unit == null);
            hostileUnits.RemoveAll(unit => unit == null);

            friendlySkills.RemoveAll(skill => skill == null);
        }

        private void ChooseDirection(Direction direction)
        {
            chosenDirection = direction;
        }
        #endregion

        #region Coroutine
        public IEnumerator UpdateUnitPositionsAtStart()
        {
            yield return new WaitForSeconds(1f);

            for(int i = unitsInvolved.Count - 1; i >= 0; i--)
            {
                unitsInvolved[i].UpdateLocation();
            }
        }
        
        IEnumerator PlaceTraps()
        {
            var trapCount = 0;
            var trapList = new List<Vector3>();
            var trapSr = activeSkill?.PlacableGameObject.GetComponent<SpriteRenderer>();
            var trapSprite = trapSr.sprite;

            var thisUnit = activeSkill.gameObject.GetComponent<Entity>();

            while (trapCount < activeSkill?.PlacableCount)
            {
                ClearPreviews();
                
                foreach (var placedTrapPos in trapList)
                {
                    var placedTrapPreview = GetClone(activeSkill.PlacableGameObject);
                    placedTrapPreview.GetComponent<SpriteRenderer>().sprite = trapSprite;
                    placedTrapPreview.transform.position = placedTrapPos;
                    placedTrapPreview.SetActive(true);
                    ghostSprites.Add(placedTrapPreview);
                }

                var target = OverlayTileManager.Instance.GetOverlayTileOnMouseCursor();
                if (target)
                {
                    if (!target.CheckEntityGameObjectOnTile() && !target.CheckObstacleOnTile() && !target.CheckTrapOnTile() && skillRangeTiles.Any(tile => tile == target))
                    {
                        var preview = GetClone(activeSkill.PlacableGameObject);
                        var previewSr = preview.GetComponent<SpriteRenderer>();
                        
                        previewSr.sprite = trapSprite;
                        previewSr.color = trapSr.color;

                        preview.transform.position = target.transform.position;
                        preview.transform.localScale = activeSkill.PlacableGameObject.transform.localScale;

                        ghostSprites.Add(preview);
                        preview.SetActive(true);

                        if (Input.GetMouseButtonDown(0) && !trapList.Contains(preview.transform.position))
                        {
                            Vector2 direction = OverlayTileManager.Instance.GetOverlayTileInWorldPos(preview.transform.position).gridLocation2D - thisUnit.ActiveTile.gridLocation2D;

                            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                            {
                                thisUnit.Direction = direction.x > 0 ? Direction.NORTH : Direction.SOUTH;
                            }
                            else
                            {
                                thisUnit.Direction = direction.y > 0 ? Direction.WEST : Direction.EAST;
                            }

                            activeSkill.StartCoroutine(activeSkill.PlaySkillAnimation(thisUnit, "RuneTrap"));
                            trapList.Add(preview.transform.position);
                            trapCount++;
                        }
                    }
                }
                yield return new WaitForSeconds(0.001f);
            }

            foreach (var placedTrapPos in trapList)
            {
                var newTrap = Instantiate(activeSkill.PlacableGameObject, placedTrapPos, Quaternion.identity);
                var skill = activeSkill;
                activeTraps.Add(newTrap, skill);
            }
            
            //Check for extras
            var count = activeTraps.Values.Cast<object>().Count(skill => skill as Skill == activeSkill);
            if (count > activeSkill.MaxCount)
            {
                var enumerator = activeTraps.GetEnumerator();
                var trapsToRemove = new List<object>();
                while (enumerator.MoveNext() && count > activeSkill.MaxCount)
                { 
                    if (enumerator.Value as Skill != activeSkill) continue;
                    trapsToRemove.Add(enumerator.Key);
                    count--;
                }

                foreach (var trap in trapsToRemove)
                {
                    activeTraps.Remove(trap);
                    Destroy((GameObject)trap);
                }
            }
            
            EndTurn();
        }

        IEnumerator WaitForSkill(Entity target)
        {
            var targetTransform = target.transform.position;
            if (activeSkill.Cast(target))
            {
                skillIsCasting = true;
                lockInput = true;

                if (activeSkill.VFXGraph)
                {
                    StartCoroutine(PlayVFX(activeSkill.VFXGraph, targetTransform));
                }
                
                #region Animations
                RenderOverlayTile.Instance.ClearTargetingRenders();
                var activeUnit = activeSkill.gameObject.GetComponent<Entity>();
                
                if(activeUnit.FrontAnimator != null || activeUnit.BackAnimator != null)
                {
                    yield return StartCoroutine(WaitForAnimationCompletion(activeUnit));
                }
                #endregion

                yield return new WaitForSeconds(0.1f);

                EndTurn();
                yield return null;

            }
            else if (secondaryTargeting)
            {
                yield return new WaitUntil(() => activeSkill.Cast(target));
                skillIsCasting = true;
                lockInput = true;
                
                if (activeSkill.VFXGraph)
                {
                    StartCoroutine(PlayVFX(activeSkill.VFXGraph, targetTransform));
                }

                #region Animations
                //wait for animations
                RenderOverlayTile.Instance.ClearTargetingRenders();
                var activeUnit = activeSkill.gameObject.GetComponent<Entity>();

                if (activeUnit.FrontAnimator != null || activeUnit.BackAnimator != null)
                {
                    yield return StartCoroutine(WaitForAnimationCompletion(activeUnit));
                }

                #endregion

                yield return new WaitForSeconds(0.1f);

                EndTurn();
            }
        }
        
        IEnumerator WaitForSkill(OverlayTile target, List<OverlayTile> aoeTiles)
        {
            if (activeSkill.Cast(target, aoeTiles))
            {
                skillIsCasting = true;
                lockInput = true;
                
                if (activeSkill.VFXGraph)
                {
                    StartCoroutine(PlayVFX(activeSkill.VFXGraph, target.transform.position));
                }
                
                #region Animations
                //wait for animations
                RenderOverlayTile.Instance.ClearTargetingRenders();
                var activeUnit = activeSkill.gameObject.GetComponent<Entity>();

                if (activeUnit.FrontAnimator != null || activeUnit.BackAnimator != null)
                {
                    yield return StartCoroutine(WaitForAnimationCompletion(activeUnit));
                }

                #endregion

                yield return new WaitForSeconds(0.1f);

                EndTurn();

                yield return null;
            }
            else if (secondaryTargeting)
            {
                yield return new WaitUntil(() => activeSkill.Cast(target, aoeTiles));
                skillIsCasting = true;
                lockInput = true;

                if (activeSkill.VFXGraph)
                {
                    StartCoroutine(PlayVFX(activeSkill.VFXGraph, target.transform.position));
                }
                
                #region Animations
                //wait for animations
                RenderOverlayTile.Instance.ClearTargetingRenders();
                var activeUnit = activeSkill.gameObject.GetComponent<Entity>();

                if (activeUnit.FrontAnimator != null || activeUnit.BackAnimator != null)
                {
                    yield return StartCoroutine(WaitForAnimationCompletion(activeUnit));
                }

                #endregion

                yield return new WaitForSeconds(0.1f);

                EndTurn();
            }
        }

        IEnumerator WaitForAnimationCompletion(Entity activeUnit)
        {
            if (activeUnit.Direction == Direction.NORTH || activeUnit.Direction == Direction.WEST)
            {
                yield return new WaitUntil(() => activeUnit.BackAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 );
            }
            else if (activeUnit.Direction == Direction.SOUTH || activeUnit.Direction == Direction.EAST)
            {
                yield return new WaitUntil(() => activeUnit.FrontAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1);
            }

            yield return new WaitForSeconds(0.1f);

            yield return null;
        }

        public IEnumerator ChooseFacingDirection(Entity activeUnit)
        {
            chosenDirection = Direction.NONE;
            var directionButton = activeUnit.GetComponentsInChildren<Button>(includeInactive: true);

            for (int i = 0; i < directionButton.Length; i++)
            {
                if (directionButton[i].onClick.GetPersistentEventCount() == 0)
                {
                    Direction directionEnum = (Direction)i;
                    directionButton[i].onClick.AddListener(() => ChooseDirection(directionEnum));
                }
            }

            Vector2Int[] directions = {
                new Vector2Int(1, 0),  // N
                new Vector2Int(-1, 0), // S
                new Vector2Int(0, -1),  // E
                new Vector2Int(0, 1)  // W
            };

            for(int i = 0; i< directions.Length; i++)
            {
                var chooseTile = OverlayTileManager.Instance.GetOverlayTile(activeUnit.ActiveTile.gridLocation2D + directions[i]);
                
                if(chooseTile == null)
                {
                    continue;
                }

                directionButton[i].gameObject.SetActive(true);
            }

            yield return new WaitUntil(() => chosenDirection != Direction.NONE);

            for (int i = 0; i < directionButton.Length; i++)
            {
                directionButton[i].gameObject.SetActive(false);
            }

            activeUnit.Direction = chosenDirection;
            activeUnit.UpdateLocation();
        }

        #endregion

        #region Object Pooling

        private List<GameObject> clonePool = new();
        private int cloneCount = 0;

        GameObject GetClone(GameObject gameObject)
        {
            foreach (var clone in clonePool.Where(clone => !clone.activeInHierarchy))
            {
                //disable all child objects
                foreach(Transform transform in  clone.transform) 
                {
                    transform.gameObject.SetActive(false);
                }

                //enable spriterenderer by default
                clone.GetComponent<SpriteRenderer>().enabled = true;
                return clone;
            } 
  
            var obj = Instantiate(gameObject);
            
            if (obj.GetComponent<SpriteRenderer>() == null)
            {
                obj.AddComponent<SpriteRenderer>();
            }

            obj.SetActive(false); 
            obj.name = $"{obj.name} {cloneCount++}";
            obj.layer = LayerMask.NameToLayer("Ignore Raycast");
            clonePool.Add(obj); 
            return obj; 
        }

        private List<OverlayTile> overlayTileClonePool = new();
        private int overlayTileCloneCount = 0;
        
        OverlayTile GetOverlayTileClone()
        {
            foreach (var clone in overlayTileClonePool)
            {
                if (!clone.gameObject.activeInHierarchy) 
                { 
                    clone.gameObject.SetActive(true);
                    ghostTiles.Add(clone);
                    return clone; 
                }
            }
            
            var tile = Instantiate(OverlayTileManager.Instance.overlaytilePrefab, OverlayTileManager.Instance.overlayContainer.transform);
            tile.gameObject.SetActive(false);
            tile.name = "Ghost Tile";
            overlayTileClonePool.Add(tile);

            return tile; 
        }
        
        private List<GameObject> vfxPool = new();
        private int vfxCount = 0;
        
        GameObject GetVfx(VisualEffectAsset vfx)
        {
            foreach (var freeVfx in vfxPool.Where(clone => !clone.activeInHierarchy))
            {
                freeVfx.GetComponent<VisualEffect>().visualEffectAsset = vfx;
                freeVfx.SetActive(false);
                return freeVfx;
            }

            var vfxObj = Instantiate(gameObject);
            vfxObj.SetActive(false);
            var vfxComponent = vfxObj.AddComponent<VisualEffect>();
            vfxComponent.visualEffectAsset = vfx;
            vfxComponent.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("UI");
    
            vfxPool.Add(vfxObj); 
            return vfxObj; 
        }

        IEnumerator PlayVFX(VisualEffectAsset vfx, Vector3 location)
        {
            var vfxObj = Instantiate(gameObject);
            vfxObj.SetActive(false);
            var vfxComponent = vfxObj.AddComponent<VisualEffect>();
            vfxComponent.visualEffectAsset = vfx;
            vfxComponent.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("UI");
            vfxObj.transform.position = location;
            vfxObj.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            Destroy(vfxObj);
        }
        
        #endregion
    }
}
