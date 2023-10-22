using NightmareEchoes.Grid;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.GraphicsBuffer;

//created by JH, edited by Ter
namespace NightmareEchoes.Unit.Combat
{
    public class CombatManager : MonoBehaviour
    {
        public static CombatManager Instance;

        public List<Entity> unitsInvolved;
        public List<Entity> aliveUnits;
        public List<Entity> deadUnits;
        
        public List<Entity> friendlyUnits;
        public List<Entity> aliveFriendlyUnits;
        public List<Entity> deadFriendlyUnits;
        
        public List<Entity> hostileUnits;
        public List<Entity> aliveHostileUnits;
        public List<Entity> deadHostileUnits;

        public bool turnEnded;
        
        [SerializeField] private Skill activeSkill;
        private List<OverlayTile> skillRangeTiles;
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
        
        //To prevent update() from casting multiple times
        private bool castGate = false;

        #region Properties
        public Skill ActiveSkill
        {
            get => activeSkill;
            private set => activeSkill = value;
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
            //if (castGate) return;
            
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
        void OnBattleStart()
        {
            unitsInvolved = FindObjectsOfType<Entity>().ToList();

            var iterator = new List<Entity>(unitsInvolved);
            foreach (var unit in iterator.Where(unit => unit.IsProp))
            {
                unitsInvolved.Remove(unit);
            }
            
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
            var hit = Physics2D.Raycast(unit.transform.position, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Overlay Tile"));
            if (!hit) return null;
            var target = hit.collider.gameObject.GetComponent<OverlayTile>();
            var trap = target.CheckTrapOnTile();

            if (!trap) return null;
            var enumerator = activeTraps.GetEnumerator(); 
            while (enumerator.MoveNext())
            {
                if ((GameObject)enumerator.Key != trap) continue;
                var activatedTrap = enumerator.Key;
                activeTraps.Remove(enumerator.Key);
                Destroy((Object)activatedTrap);
                return enumerator.Value as Skill;
            }

            return null;
        }
        
        private void EndTurn()
        {
            if(activeSkill.GetComponent<Entity>() != null)
            {
                activeSkill.GetComponent<Entity>().ShowPopUpText(activeSkill.SkillName, Color.red);
            }

            activeSkill.Reset();
            activeSkill = null;

            secondaryTargeting = false;
            
            RenderOverlayTile.Instance.ClearTargetingRenders();
            ClearPreviews();

            turnEnded = true;
            castGate = false;
        }
        #endregion

        #region Public Calls

        public void SelectSkill(Entity unit, Skill skill)
        {
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

            activeSkill.Cast();
            EndTurn();
        }
        public void SecondaryTargeting()
        {
            secondaryTargeting = true;
        }

        #endregion

        #region Enemy Targeting
        public void EnemyTargetUnit(Entity target, Skill skill)
        {
            activeSkill = skill;
            activeSkill.Cast(target);
            EndTurn();
        }

        public void EnemyTargetGround(OverlayTile targetTile, Skill skill)
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
            EndTurn();
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
                
                clone.transform.localScale = entity.transform.localScale;
                cloneSr.sprite = entitySr.sprite;
                var entityColor = entitySr.color;
                cloneSr.color = new Color(entityColor.r, entityColor.g, entityColor.b, 0.5f);
                cloneSr.sortingLayerID = SortingLayer.NameToID("Entity");
                ghostSprites.Add(clone);
                
                clone.SetActive(true);
                clone.transform.position = destination;
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

        #region Additional Mechanics
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
            //Debug.Log(count);
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
                    Destroy((Object)trap);
                }
            }
            
            EndTurn();
        }

        IEnumerator WaitForSkill(Entity target)
        {
            if (activeSkill.Cast(target))
            {
                EndTurn();
                yield return null;
            }else if (secondaryTargeting)
            {
                yield return new WaitUntil(() => activeSkill.Cast(target));
                EndTurn();
            }
        }
        
        IEnumerator WaitForSkill(OverlayTile target, List<OverlayTile> aoeTiles)
        {
            if (activeSkill.Cast(target, aoeTiles))
            {
                EndTurn();
                yield return null;
            }
            else if (secondaryTargeting)
            {
                yield return new WaitUntil(() => activeSkill.Cast(target, aoeTiles));
                EndTurn();
            }
        }
        #endregion

        #region Object Pooling

        private List<GameObject> clonePool = new();
        private int cloneCount = 0;

        GameObject GetClone(GameObject gameObject)
        {
            foreach (var clone in clonePool.Where(clone => !clone.activeInHierarchy))
            {
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
        #endregion
    }
}
