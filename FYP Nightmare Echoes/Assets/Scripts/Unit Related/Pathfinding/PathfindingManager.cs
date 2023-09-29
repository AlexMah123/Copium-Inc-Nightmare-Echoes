using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using NightmareEchoes.Grid;
using NightmareEchoes.Inputs;
using TMPro;


//created by Vinn, editted by Alex and Ter
namespace NightmareEchoes.Unit.Pathfinding
{
    public class PathfindingManager : MonoBehaviour
    {
        public static PathfindingManager Instance;

        [Header("Overlay Tile Container")]
        [SerializeField] GameObject overlayTileContainer; 

        [Header("Current Unit")]
        Units currentSelectedUnit;
        [SerializeField] float movingSpeed;
        [SerializeField] bool ifSelectedUnit = false;

        [Header("Path list + Tiles in Range")]
        [SerializeField] List<OverlayTile> pathList = new List<OverlayTile>();
        [SerializeField] List<OverlayTile> tempPathList = new List<OverlayTile>();
        public List<OverlayTile> playerTilesInRange = new List<OverlayTile>();

        //ArrowBool Detection Stuff
        bool isMoving = false;
        bool isDragging = false;
        bool isDraggingFromPlayer = false;

        //Mouse Detection
        Vector3 mousePrevPos;
        [SerializeField] float dragThreshold = 1f;

        //Temp values whenever a player revert/resets
        private OverlayTile revertUnitPosition;
        private Direction revertUnitDirection;
        private int revertUnitHealth;

        //hovered tile related
        RaycastHit2D? hoveredTile;
        OverlayTile currentHoveredOverlayTile;
        OverlayTile lastAddedTile = null;

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

        void Update()
        {
            //if you cancel movement
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //UNCOMMENT IF WE WANT TO RESET THE TILES SHOWN AND SELECTED UNIT
                //ifSelectedUnit = false;
                //HideTilesInRange(playerTilesInRange);

                if (currentSelectedUnit != null && revertUnitPosition != null)
                {
                    SetUnitPositionOnTile(revertUnitPosition, currentSelectedUnit);
                    currentSelectedUnit.Direction = revertUnitDirection;
                    currentSelectedUnit.stats.Health = revertUnitHealth;

                    //Add Section to have reverts if they hit a trap.

                    //Resets everything, not moving, not dragging, and lastaddedtile is null
                    isMoving = false;
                    isDragging = false;
                    lastAddedTile = null;

                    ClearArrow(tempPathList);

                }
            }

            PlayerInputPathfinding();
        }

        public void PlayerInputPathfinding()
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

            #region Check for types of input
            //if player clicks
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = false;
                mousePrevPos = Input.mousePosition;

                //sets it if you starting dragging from the players activetile
                if(currentHoveredOverlayTile == currentSelectedUnit?.ActiveTile)
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

            #region selecting a player to move
            //if player clicked and has not previously selected a unit, raycast and check
            if (Input.GetMouseButtonDown(0) && !ifSelectedUnit)
            {
                //Mouse Position to select unit
                var hitUnit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Unit"));

                //if you hit a unit then get component
                if (hitUnit)
                {
                    var unit = hitUnit.collider.gameObject.GetComponent<Units>();

                    //if unit exist and is a player
                    if (unit && !unit.IsHostile)
                    {
                        //set currentSelected to the raycasted unit
                        currentSelectedUnit = unit;
                        ifSelectedUnit = true;

                        //assign the units active tile, currentHoverOverlayTile wont be null because you always are on the map
                        currentSelectedUnit.ActiveTile = currentHoveredOverlayTile;

                        //Gets the value of the start pos and the maximum range is the amount you can set
                        playerTilesInRange = Pathfinding.FindTilesInRange(currentSelectedUnit.ActiveTile, currentSelectedUnit.stats.MoveRange);

                        //store values for players when they cancel their action
                        revertUnitPosition = currentSelectedUnit.ActiveTile;
                        revertUnitDirection = currentSelectedUnit.Direction;
                        revertUnitHealth = currentSelectedUnit.stats.Health;

                        //display tiles in range
                        ShowTilesInRange(playerTilesInRange);

                    }
                }
            }
            #endregion

            #region selecting tile to move on

            //if player clicked and isnt moving + selected a unit or if they move to their activetile/starting point
            //currentHoverOverlayTile wont be null because you always are on the map
            if (Input.GetMouseButtonDown(0) && !isMoving && ifSelectedUnit && (playerTilesInRange.Contains(currentHoveredOverlayTile) || currentHoveredOverlayTile == currentSelectedUnit?.ActiveTile))
            {
                pathList = Pathfinding.FindPath(currentSelectedUnit.ActiveTile, currentHoveredOverlayTile, playerTilesInRange);
                tempPathList = new List<OverlayTile>(pathList);

                if (!currentHoveredOverlayTile.CheckUnitOnTile() && !currentHoveredOverlayTile.CheckObstacleOnTile())
                {
                    //Resets lastaddedtile is null
                    lastAddedTile = null;
                    isMoving = true;

                    RenderArrow(playerTilesInRange, pathList, currentSelectedUnit);
                }
            }
            // if player dragged move, isnt moving + selected a unit, or if they move to their activetile/starting point
            else if (isDragging && !isMoving && ifSelectedUnit &&
                (playerTilesInRange.Contains(currentHoveredOverlayTile) || currentHoveredOverlayTile == currentSelectedUnit.ActiveTile || currentHoveredOverlayTile == revertUnitPosition))
            {
                //if the first tile is diagonal or is the same tile on the player, just return and ignore it
                if (pathList.Count == 0 && !AreTilesAdjacent(currentSelectedUnit.ActiveTile, currentHoveredOverlayTile) && currentHoveredOverlayTile != revertUnitPosition)
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
                else if (currentHoveredOverlayTile == revertUnitPosition || currentHoveredOverlayTile == currentSelectedUnit.ActiveTile)
                {
                    //if you moved back to your starting position or activetile, reset
                    lastAddedTile = pathList.Count > 0 ? pathList[pathList.Count - 1] : null;
                }

                //if there is no unit on tile
                if (!currentHoveredOverlayTile.CheckUnitOnTile() && !currentHoveredOverlayTile.CheckObstacleOnTile())
                {
                    RenderArrow(playerTilesInRange, pathList, currentSelectedUnit);
                }
                else if (currentHoveredOverlayTile == currentSelectedUnit.ActiveTile)
                {
                    //if the currenthovered tile is the active tile, resets not dragging 
                    isDragging = false;
                    RenderArrow(playerTilesInRange, pathList, currentSelectedUnit);
                    pathList.Clear();
                    ClearArrow(tempPathList);
                }
                else if (currentHoveredOverlayTile == revertUnitPosition)
                {
                    //if the currenthovered tile is the active, resets not dragging
                    isDragging = false;
                    RenderArrow(playerTilesInRange, pathList, currentSelectedUnit);
                    pathList.Clear();
                    ClearArrow(tempPathList);
                }

                if (Input.GetMouseButtonUp(0))
                {
                    tempPathList = new List<OverlayTile>(pathList);
                    isMoving = true;

                    //Resets not dragging and lastAddedTile to null
                    isDragging = false;
                    lastAddedTile = null;

                    if(pathList.Count <= 0)
                    {
                        ClearArrow(tempPathList);
                    }
                }
            }

            #endregion

            //When unit is moving
            if (isMoving)
            {
                CameraControl.Instance.UpdateCameraPan(currentSelectedUnit.gameObject);
                MoveAlongPath(currentSelectedUnit, pathList, playerTilesInRange);
            }
        }

        #region Movement along Tile
        public IEnumerator MoveTowardsTile(Units thisUnit, OverlayTile targetTile, float duration)
        {
            #region Trigger Movement Related Status Effect Before Movement
            for (int i = thisUnit.TokenList.Count - 1; i >= 0; i--)
            {
                switch (thisUnit.TokenList[i].statusEffect)
                {
                    case STATUS_EFFECT.IMMOBILIZE_TOKEN:
                        thisUnit.TokenList[i].TriggerEffect(thisUnit);
                        isMoving = false;

                        ClearArrow(tempPathList);
                        yield return null;
                        break;
                }
            }
            #endregion

            float counter = 0;

            //Get the current position of the object to be moved
            Vector3 startPos = thisUnit.transform.position;
            Vector3 direction = targetTile.gridLocation - thisUnit.ActiveTile.gridLocation;

            ChangeDirection(direction, thisUnit);

            while (counter < duration)
            {
                counter += Time.deltaTime;
                thisUnit.transform.position = Vector3.Lerp(startPos, targetTile.transform.position, counter / duration);
                yield return null;
            }

            #region Triggering Movement Related Status Effect During Movement
            for (int i = thisUnit.BuffDebuffList.Count - 1; i >= 0; i--)
            {
                switch (thisUnit.BuffDebuffList[i].statusEffect)
                {
                    case STATUS_EFFECT.CRIPPLED_DEBUFF:
                        thisUnit.BuffDebuffList[i].TriggerEffect(thisUnit);
                        break;
                }
            }

            #endregion

            SetUnitPositionOnTile(targetTile, thisUnit);
        }

        public void MoveAlongPath(Units thisUnit, List<OverlayTile> pathList, List<OverlayTile> tilesInRange)
        {
            //units movement
            if (pathList.Count > 0 && thisUnit != null) 
            {
                #region Trigger Movement Related Status Effect Before Movement
                for (int i = thisUnit.TokenList.Count - 1; i >= 0; i--)
                {
                    switch (thisUnit.TokenList[i].statusEffect)
                    {
                        case STATUS_EFFECT.IMMOBILIZE_TOKEN:
                            thisUnit.TokenList[i].TriggerEffect(thisUnit);
                            isMoving = false;

                            ClearArrow(tempPathList);
                            pathList.Clear();
                            return;
                    }
                }
                #endregion

                #region Setting Unit Direction
                Vector3Int direction = pathList[0].gridLocation - thisUnit.ActiveTile.gridLocation;

                //setting directions as well as the moving boolean
                ChangeDirection(direction, thisUnit);

                //set the units direction facing based on the vector between player and the next tile
                if (pathList.Count > 0 && thisUnit != null)
                {

                }
                #endregion

                var step = movingSpeed * Time.deltaTime;
                var zIndex = pathList[0].transform.position.z;

                thisUnit.transform.position = Vector2.MoveTowards(thisUnit.transform.position, pathList[0].transform.position, step);
                thisUnit.transform.position = new Vector3(thisUnit.transform.position.x, thisUnit.transform.position.y, zIndex);

                //as you reach the tile, set the units position and the arrow, remove the pathlist[0] to move to the next tile
                if (Vector2.Distance(thisUnit.transform.position, pathList[0].transform.position) < 0.01f)
                {
                    SetUnitPositionOnTile(pathList[0], thisUnit);

                    pathList[0].SetArrowSprite(ArrowDirections.None);

                    pathList.RemoveAt(0);


                    #region Triggering Movement Related Status Effect During Movement
                    for (int i = thisUnit.BuffDebuffList.Count - 1; i >= 0; i--) 
                    {
                        switch(thisUnit.BuffDebuffList[i].statusEffect)
                        {
                            case STATUS_EFFECT.CRIPPLED_DEBUFF:
                                thisUnit.BuffDebuffList[i].TriggerEffect(thisUnit);
                                break;
                        }
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
                
                //remove comment out later if we want to hide tile and reset selectedUnit when we stop moving
                //HideTilesInRange(tilesInRange);
                //ifSelectedUnit = false;

                //remove comment out if we want to enable multiple movement 
                //isMoving = false;
            }
        }
        #endregion

        #region Utility for unit direction
        public void ChangeDirection(Vector3 direction, Units thisUnit)
        {
            //setting directions as well as the moving boolean
            if (direction == new Vector3Int(1, 0, 0)) //back facing
            {
                thisUnit.Direction = Direction.NORTH;

                if (thisUnit.BackModel != null && thisUnit.FrontAnimator != null && thisUnit.BackAnimator != null)
                {
                    thisUnit.BackAnimator.SetBool("Moving", true);
                    thisUnit.FrontAnimator.SetBool("Moving", false);
                }
            }
            else if (direction == new Vector3Int(0, 1, 0)) //back facing
            {
                thisUnit.Direction = Direction.WEST;

                if (thisUnit.BackModel != null && thisUnit.FrontAnimator != null && thisUnit.BackAnimator != null)
                {
                    thisUnit.BackAnimator.SetBool("Moving", true);
                    thisUnit.FrontAnimator.SetBool("Moving", false);
                }
            }
            else if (direction == new Vector3Int(-1, 0, 0)) //front facing
            {
                thisUnit.Direction = Direction.SOUTH;

                if (thisUnit.FrontModel != null && thisUnit.FrontAnimator != null && thisUnit.BackAnimator != null)
                {
                    thisUnit.FrontAnimator.SetBool("Moving", true);
                    thisUnit.BackAnimator.SetBool("Moving", false);
                }
            }
            else if (direction == new Vector3Int(0, -1, 0)) //front facing
            {
                thisUnit.Direction = Direction.EAST;

                if (thisUnit.FrontModel != null && thisUnit.FrontAnimator != null && thisUnit.BackAnimator != null)
                {
                    thisUnit.FrontAnimator.SetBool("Moving", true);
                    thisUnit.BackAnimator.SetBool("Moving", false);
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

        public void RenderArrow(List<OverlayTile> tilesInRange, List<OverlayTile> pathList, Units thisUnit)
        {
            foreach (var item in tilesInRange)
            {
                item.SetArrowSprite(ArrowDirections.None);
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
            RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Overlay Tile"));

            //Checks if the raycast has hit any tile
            if (hits.Length > 0)
            {
                return hits.OrderByDescending(i => i.collider.transform.position.z).First();
            }

            return null;
        }

        public void SetUnitPositionOnTile(OverlayTile tile, Units unit)
        {
            unit.gameObject.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z);
            unit.gameObject.GetComponent<Units>().ActiveTile = tile;
        }

        public void ShowTilesInRange(List<OverlayTile> overlayTileList)
        {
            //hides all the tiles in range to reset
            foreach (var item in overlayTileList)
            {
                item.HideTile();
            }

            //displays all the tiles in range 
            foreach (var item in overlayTileList)
            {
                item.ShowMoveTile();
            }
        }

        public void HideTilesInRange(List<OverlayTile> tilesInRange)
        {
            //hides all the tiles in range to reset
            foreach (var item in tilesInRange)
            {
                item.HideTile();
            }
        }

        public void ClearUnitPosition()
        {
            //called in unit attack (UIManager button), clears all existing arrows, resets selected unit, reset revertsunitPosition
            ClearArrow(tempPathList);
            revertUnitPosition = null;
            revertUnitHealth = 0;


            ifSelectedUnit = false;
            isMoving = false;
            isDragging = false;
            lastAddedTile = null;
        }

        public void ClearArrow(List<OverlayTile> pathList)
        {
            foreach (var tile in pathList)
            {
                tile.SetArrowSprite(ArrowDirections.None);
            }

            pathList.Clear();
        }
        #endregion
    }
}
