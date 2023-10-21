using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NightmareEchoes.Grid;
using NightmareEchoes.Inputs;
using NightmareEchoes.Unit.Combat;


//created by Vinn, editted by Alex and Ter
namespace NightmareEchoes.Unit.Pathfinding
{
    public class PathfindingManager : MonoBehaviour
    {
        public static PathfindingManager Instance;

        [Header("Overlay Tile Container")]
        [SerializeField] GameObject overlayTileContainer; 

        [Header("Current Unit")]
        Entity currentPathfindingUnit;
        [SerializeField] float movingSpeed;

        [Header("Path list + Tiles in Range")]
        public List<OverlayTile> pathList = new List<OverlayTile>();
        public List<OverlayTile> tempPathList = new List<OverlayTile>();
        public List<OverlayTile> playerTilesInRange = new List<OverlayTile>();

        //ArrowBool Detection Stuff
        public bool isMoving = false;
        public bool hasMoved = false;
        public bool isDragging = false;
        public bool isDraggingFromPlayer = false;

        //Mouse Detection
        Vector3 mousePrevPos;
        [SerializeField] float dragThreshold = 1f;

        //Temp values whenever a player revert/resets
        private OverlayTile revertUnitPosition;
        private Direction revertUnitDirection;
        private int revertUnitHealth;

        //hovered tile related
        RaycastHit2D? hoveredTile;
        public OverlayTile currentHoveredOverlayTile;
        public OverlayTile lastAddedTile = null;


        #region properties
        public Entity CurrentPathfindingUnit
        {
            get => currentPathfindingUnit;
            set => currentPathfindingUnit = value;
        }

        public OverlayTile RevertUnitPosition
        {
            get => revertUnitPosition;
            set => revertUnitPosition = value;
        }

        public Direction RevertUnitDirection
        {
            get => revertUnitDirection;
            set => revertUnitDirection = value;
        }

        public int RevertUnitHealth
        { 
            get { return revertUnitHealth; }
            set { revertUnitHealth = value; }
        }
        #endregion

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            playerTilesInRange.Clear();
        }

        private void Update()
        {
            //Check HoverTile based on mouse pos if its on the map
            hoveredTile = GetFocusedTile();

            if (hoveredTile.HasValue)
            {
                //assign the hovered Tile to the hovered tile
                currentHoveredOverlayTile = hoveredTile.Value.collider.GetComponent<OverlayTile>();

                //update the hovered tile to
                transform.position = currentHoveredOverlayTile.transform.position;
            }
            else
            {
                currentHoveredOverlayTile = null;
            }
        }

        //Called in Player Phase
        public void StartPlayerPathfinding(Entity hero)
        {
            if (hero != null && !hero.IsHostile && !hero.IsProp)
            {
                //set currentSelected to the raycasted unit
                currentPathfindingUnit = hero;

                //Gets the value of the start pos and the maximum range is the amount you can set
                playerTilesInRange = Pathfind.FindTilesInRange(hero.ActiveTile, hero.stats.MoveRange, ignoreProps: false);

                //store values for players when they cancel their action
                revertUnitPosition = hero.ActiveTile;
                revertUnitDirection = hero.Direction;
                revertUnitHealth = hero.stats.Health;

                //display tiles in range
                ShowTilesInRange(playerTilesInRange);
                CameraControl.Instance.UpdateCameraPan(currentPathfindingUnit.gameObject);
            }
        }

        //Called in Player Phase
        public void PlayerInputPathfinding()
        {
            #region Check for types of input
            //if player clicks
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = false;
                mousePrevPos = Input.mousePosition;

                //sets it if you starting dragging from the players activetile
                if(currentHoveredOverlayTile == currentPathfindingUnit?.ActiveTile)
                {
                    isDraggingFromPlayer = true;
                }
            }

            //if player drags past dragThreshold and if dragged from player activetile
            if (Input.GetMouseButton(0) && isDraggingFromPlayer)
            {
                float dist = Vector3.Distance(Input.mousePosition, mousePrevPos);

                if (dist > dragThreshold)
                {
                    isDragging = true;
                }

                mousePrevPos = Input.mousePosition;
            }
            #endregion

            #region selecting tile to move on

            //if player clicked and isnt moving + selected a unit or if they move to their activetile/starting point
            //currentHoverOverlayTile wont be null because you always are on the map
            if (Input.GetMouseButtonDown(0) && !hasMoved && (playerTilesInRange.Contains(currentHoveredOverlayTile) || currentHoveredOverlayTile == currentPathfindingUnit?.ActiveTile))
            {
                pathList = Pathfind.FindPath(currentPathfindingUnit.ActiveTile, currentHoveredOverlayTile, playerTilesInRange);
                tempPathList = new List<OverlayTile>(pathList);

                if (!currentHoveredOverlayTile.CheckEntityGameObjectOnTile() && !currentHoveredOverlayTile.CheckObstacleOnTile())
                {
                    //Resets lastaddedtile is null
                    if(pathList.Count > 0)
                    {
                        lastAddedTile = null;
                        hasMoved = true;
                        isMoving = true;
                        RenderArrow(playerTilesInRange, pathList, currentPathfindingUnit);
                    }
                }
            }
            // if player dragged move, isnt moving + selected a unit, or if they move to their activetile/starting point
            else if (isDragging && !hasMoved &&
                (playerTilesInRange.Contains(currentHoveredOverlayTile) || currentHoveredOverlayTile == currentPathfindingUnit.ActiveTile || currentHoveredOverlayTile == revertUnitPosition))
            {
                //if the first tile is diagonal or is the same tile on the player, just return and ignore it
                if (pathList.Count == 0 && !AreTilesAdjacent(currentPathfindingUnit.ActiveTile, currentHoveredOverlayTile) && currentHoveredOverlayTile != revertUnitPosition)
                {
                    return;
                }

                // if initial tile added or the lastadded tile is adjacent and if the dragtile != the starting tile
                if (lastAddedTile == null || AreTilesAdjacent(lastAddedTile, currentHoveredOverlayTile) && currentHoveredOverlayTile != revertUnitPosition)
                {
                    //if path doesnt contain the hovered tile, add it
                    if (!pathList.Contains(currentHoveredOverlayTile))
                    {
                        pathList.Add(currentHoveredOverlayTile);
                        lastAddedTile = currentHoveredOverlayTile;
                    }
                    else if (pathList.Contains(currentHoveredOverlayTile) && lastAddedTile == pathList[pathList.Count - 1])
                    {
                        //if you moved back to a path that you already moved on, remove that path
                        pathList.Remove(lastAddedTile);
                        lastAddedTile = pathList.Count > 0 ? pathList[pathList.Count - 1] : null;
                    }
                }
                else if (currentHoveredOverlayTile == revertUnitPosition || currentHoveredOverlayTile == currentPathfindingUnit.ActiveTile)
                {
                    //if you moved back to your starting position or activetile, reset
                    lastAddedTile = pathList.Count > 0 ? pathList[pathList.Count - 1] : null;
                }

                if (currentHoveredOverlayTile == currentPathfindingUnit.ActiveTile || currentHoveredOverlayTile == revertUnitPosition)
                {
                    //if the currenthovered tile is the active tile or the reverted position or if you drag out of range resets not dragging 
                    isDragging = false;
                    pathList.Clear();
                    RenderArrow(playerTilesInRange, pathList, currentPathfindingUnit);
                    ClearArrow(tempPathList);
                }

                RenderArrow(playerTilesInRange, pathList, currentPathfindingUnit);


                if (Input.GetMouseButtonUp(0))
                {
                    if(pathList.Count > 0)
                    {
                        if (pathList[pathList.Count - 1].CheckEntityGameObjectOnTile())
                        {
                            pathList.RemoveAt(pathList.Count - 1);
                        }

                        RenderArrow(playerTilesInRange, pathList, currentPathfindingUnit);
                        tempPathList = new List<OverlayTile>(pathList);
                        hasMoved = true;
                        isMoving = true;
                    }

                    //Resets not dragging and lastAddedTile to null
                    isDragging = false;
                    lastAddedTile = null;

                    if (pathList.Count <= 0)
                    {
                        ClearArrow(tempPathList);
                    }
                }

            }
            else if(isDragging && !hasMoved && !playerTilesInRange.Contains(currentHoveredOverlayTile))
            {
                //if the currenthovered tile is the active tile or the reverted position or if you drag out of range resets not dragging 
                isDragging = false;
                pathList.Clear();
                RenderArrow(playerTilesInRange, pathList, currentPathfindingUnit);
                ClearArrow(tempPathList);
            }

            #endregion
        }

        public void CheckMovement()
        {
            //When unit is moving
            if (isMoving && currentPathfindingUnit != null)
            {
                CameraControl.Instance.UpdateCameraPan(currentPathfindingUnit.gameObject);
                MoveAlongPath(currentPathfindingUnit, pathList, playerTilesInRange);
            }
        }

        #region Movement along Tile
        public IEnumerator MoveTowardsTile(Entity thisUnit, OverlayTile targetTile, float duration)
        {
            float counter = 0;

            //Get the current position of the object to be moved
            Vector3 startPos = thisUnit.transform.position;
            Vector3 direction = targetTile.gridLocation - thisUnit.ActiveTile.gridLocation;
            ChangeDirection(direction, thisUnit);

            while (counter < duration)
            {
                counter += Time.deltaTime;
                if (thisUnit != null)
                {
                    thisUnit.transform.position = Vector3.Lerp(startPos, targetTile.transform.position, counter / duration);
                }
                else
                {
                    counter = duration;
                    break;
                }

                yield return null;
            }

            if(thisUnit != null)
                SetUnitPositionOnTile(thisUnit, targetTile);

            #region Triggering Movement Related BuffDebuff Effect During Movement

            thisUnit.CheckCrippled();

            if (thisUnit.CheckImmobilize())
            {
                isMoving = false;
                revertUnitPosition = null;
                ClearArrow(tempPathList);

                yield break;
            }

            yield break;
            #endregion
        }

        public void MoveAlongPath(Entity thisUnit, List<OverlayTile> pathList, List<OverlayTile> tilesInRange)
        {
            //units movement
            if (pathList.Count > 0 && thisUnit != null) 
            {   
                //Setting Unit Direction
                Vector3Int direction = pathList[0].gridLocation - thisUnit.ActiveTile.gridLocation;

                //setting directions as well as the moving boolean
                ChangeDirection(direction, thisUnit);

                var step = movingSpeed * Time.deltaTime;
                var targetPosition = pathList[0].transform.position;

                thisUnit.transform.position = Vector2.MoveTowards(thisUnit.transform.position, targetPosition, step);

                //as you reach the tile, set the units position and the arrow, remove the pathlist[0] to move to the next tile
                if (Vector2.Distance(thisUnit.transform.position, targetPosition) < 0.001f)
                {
                    SetUnitPositionOnTile(thisUnit, pathList[0]);

                    pathList[0].SetArrowSprite(ArrowDirections.None);

                    pathList.RemoveAt(0);

                    #region Triggering Movement Related BuffDebuff Effect During Movement
                    thisUnit.CheckCrippled();

                    if (thisUnit.CheckImmobilize())
                    {
                        isMoving = false;
                        revertUnitPosition = null;
                        ClearArrow(pathList);
                        pathList.Clear();

                        return;
                    }

                    #endregion
                }
            }

            if (pathList.Count <= 0 )
            {
                if(thisUnit.FrontAnimator != null && thisUnit.BackAnimator != null)
                {
                    thisUnit.BackAnimator.SetBool("Moving", false);
                    thisUnit.FrontAnimator.SetBool("Moving", false);
                }

                CameraControl.Instance.isPanning = false;
                isMoving = false;
                thisUnit.HasMoved = true;
            }
        }
        #endregion

        #region Utility for unit direction
        public void ChangeDirection(Vector3 direction, Entity thisUnit)
        {
            //reset the movements
            if (thisUnit.BackModel != null && thisUnit.FrontModel != null && thisUnit.FrontAnimator != null && thisUnit.BackAnimator != null)
            {
                thisUnit.BackAnimator.SetBool("Moving", false);
                thisUnit.FrontAnimator.SetBool("Moving", false);
            }

            //setting directions as well as the moving boolean
            if (direction == new Vector3Int(1, 0, 0)) //back facing
            {
                thisUnit.Direction = Direction.NORTH;

                if (thisUnit.BackModel != null && thisUnit.BackAnimator != null)
                {
                    thisUnit.BackAnimator.SetBool("Moving", true);
                }
            }
            else if (direction == new Vector3Int(0, 1, 0)) //back facing
            {
                thisUnit.Direction = Direction.WEST;

                if (thisUnit.BackModel != null && thisUnit.BackAnimator != null)
                {
                    thisUnit.BackAnimator.SetBool("Moving", true);
                }
            }
            else if (direction == new Vector3Int(-1, 0, 0)) //front facing
            {
                thisUnit.Direction = Direction.SOUTH;

                if (thisUnit.FrontModel != null && thisUnit.FrontAnimator != null)
                {
                    thisUnit.FrontAnimator.SetBool("Moving", true);
                }
            }
            else if (direction == new Vector3Int(0, -1, 0)) //front facing
            {
                thisUnit.Direction = Direction.EAST;

                if (thisUnit.FrontModel != null && thisUnit.FrontAnimator != null)
                {
                    thisUnit.FrontAnimator.SetBool("Moving", true);
                }
            }
        }

        #endregion

        #region Overlay Tile Related
        bool AreTilesAdjacent(OverlayTile tile1, OverlayTile tile2)
        {
            // Check if the tiles are adjacent horizontally or vertically (not diagonally)
            return (Mathf.Abs(tile2.gridLocation.x - tile1.gridLocation.x) == 1 && tile1.gridLocation.y == tile2.gridLocation.y) ||
                   (Mathf.Abs(tile2.gridLocation.y - tile1.gridLocation.y) == 1 && tile1.gridLocation.x == tile2.gridLocation.x);
        }

        public void RenderArrow(List<OverlayTile> tilesInRange, List<OverlayTile> pathList, Entity thisUnit)
        {
            for (int i = 0; i < tilesInRange.Count; i++)
            {
                tilesInRange[i].SetArrowSprite(ArrowDirections.None);
            }

            for (int i = 0; i < pathList.Count; i++)
            {
                var prevTile = i > 0 ? pathList[i - 1] : thisUnit.ActiveTile;
                var futTile = i < pathList.Count - 1 ? pathList[i + 1] : null;

                var arrowDir = ArrowRenderer.TranslateDirection(prevTile, pathList[i], futTile);
                pathList[i].SetArrowSprite(arrowDir);
            }
        }

        public RaycastHit2D? GetFocusedTile()
        {
            var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Overlay Tile"));

            return hit ? hit : null;
        }

        public void SetUnitPositionOnTile(Entity unit, OverlayTile tile)
        {
            unit.gameObject.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z);
            unit.ActiveTile = tile;
            
            if (!unit.IsHostile) return;
            
            var trapDmg = CombatManager.Instance.CheckTrap(unit);

            if (trapDmg)
            {
                trapDmg.Cast(unit);
            }
        }

        public void ShowTilesInRange(List<OverlayTile> tilesInRange)
        {
            HideTilesInRange(tilesInRange);

            for(int i = 0; i < tilesInRange.Count; i++)
            {
                tilesInRange[i].ShowMoveTile();
            }
        }

        public void HideTilesInRange(List<OverlayTile> tilesInRange)
        {
            for (int i = 0; i < tilesInRange.Count; i++)
            {
                tilesInRange[i].HideTile();
            }
        }

        public void ClearUnitPosition()
        {
            //called in unit attack (UIManager button), clears all existing arrows, resets selected unit, reset revertsunitPosition
            ClearArrow(tempPathList);
            revertUnitPosition = null;
            revertUnitHealth = 0;


            hasMoved = false;
            isDragging = false;
            lastAddedTile = null;
        }

        public void ClearArrow(List<OverlayTile> pathList)
        {
            for(int i = 0; i < pathList.Count; i++)
            {
                pathList[i].SetArrowSprite(ArrowDirections.None);
            }

            pathList.Clear();
        }
        #endregion
    }
}
