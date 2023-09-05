using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using NightmareEchoes.Grid;
using NightmareEchoes.Inputs;

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
        [SerializeField] GameObject currentSelectedUnitGO;
        [SerializeField] float movingSpeed;
        [SerializeField] bool ifSelectedUnit = false;

        //ArrowBool Detection Stuff
        bool isMoving = false;
        bool isDragging = false;

        //Mouse Detection
        Vector3 mousePrevPos;
        [SerializeField] float dragThreshold = 1f;

        //This Vector3Int is to return player to the previous position when they press escape
        private OverlayTile revertUnitPosition;

        List<OverlayTile> pathList = new List<OverlayTile>();
        List<OverlayTile> tempPathList = new List<OverlayTile>();
        public List<OverlayTile> playerTilesInRange = new List<OverlayTile>();

        RaycastHit2D? hoveredTile;
        OverlayTile overlayTile;
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
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //ifSelectedUnit = false;
                //HideTilesInRange(playerTilesInRange);

                if (currentSelectedUnit != null && revertUnitPosition != null)
                {
                    SetUnitPositionOnTile(revertUnitPosition,currentSelectedUnitGO);

                    ClearArrow();
                }
            }

            PlayerInputPathfinding();
        }

        public void PlayerInputPathfinding()
        { 
            //Check HoverTile based on mouse pos if its on the map
            hoveredTile = GetFocusedTile();

            #region Check for types of input
            //Set Movement for Player if dragging
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = false;
                mousePrevPos = Input.mousePosition;
            }

            if (Input.GetMouseButton(0))
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

                    if (unit && !unit.IsHostile)
                    {
                        currentSelectedUnitGO = unit.gameObject;
                        currentSelectedUnit = unit;
                        ifSelectedUnit = true;

                        var hitOverlayTile = Physics2D.Raycast(currentSelectedUnitGO.transform.position, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Overlay Tile"));

                        if (hitOverlayTile.collider.gameObject.GetComponent<OverlayTile>())
                        {
                            currentSelectedUnit.ActiveTile = hitOverlayTile.collider.GetComponent<OverlayTile>();

                            //Gets the value of the start pos and the maximum range is the amount you can set
                            playerTilesInRange = PathFinding.FindTilesInRange(currentSelectedUnit.ActiveTile, currentSelectedUnit.stats.MoveRange);
                            revertUnitPosition = currentSelectedUnit.ActiveTile;
                            ShowTilesInRange(playerTilesInRange);
                        }
                    }
                    else
                    {
                        ifSelectedUnit = false;
                    }
                }
                else
                {
                    ifSelectedUnit = false;
                }
            }
            #endregion

            #region selecting tile to move on
            if (hoveredTile.HasValue)
            {
                overlayTile = hoveredTile.Value.collider.GetComponent<OverlayTile>();
                transform.position = overlayTile.transform.position;

                //if you are not moving and you selected a unit
                if (playerTilesInRange.Contains(overlayTile) && !isMoving && ifSelectedUnit)
                {
                    if(Input.GetMouseButtonDown(0))
                    {
                        pathList = PathFinding.FindPath(currentSelectedUnit.ActiveTile, overlayTile, playerTilesInRange);
                        tempPathList = pathList;

                        if (!overlayTile.CheckUnitOnTile() && !overlayTile.CheckObstacleOnTile())
                        {
                            RenderArrow();
                            isMoving = true;
                        }
                    }
                    else if(isDragging)
                    {
                        var hitTile = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Overlay Tile"));
                        
                        /*if(Camera.main.ScreenToWorldPoint(Input.mousePosition) == revertUnitPosition.gridLocation)
                        {
                            ClearArrow();
                        }*/

                        if (hitTile)
                        {
                            var dragTile = hitTile.collider.gameObject.GetComponent<OverlayTile>();

                            if (dragTile != null)
                            {
                                if(lastAddedTile == null || AreTilesAdjacent(lastAddedTile, dragTile))
                                {
                                    if(!pathList.Contains(dragTile))
                                    {
                                        pathList.Add(dragTile);
                                        lastAddedTile = dragTile;
                                    }
                                    else if(pathList.Contains(dragTile) && lastAddedTile == pathList[pathList.Count - 1])
                                    {
                                        pathList.Remove(lastAddedTile);
                                        lastAddedTile = pathList.Count > 0 ? pathList[pathList.Count - 1] : null;
                                    }
                                }
                            }
                        }
 
                        if ((!overlayTile.CheckUnitOnTile() || overlayTile == currentSelectedUnit.ActiveTile)&& !overlayTile.CheckObstacleOnTile())
                        {
                            RenderArrow();
                        }

                        if (Input.GetMouseButtonUp(0))
                        {
                            tempPathList = pathList;
                            isMoving = true;
                            isDragging = false;
                            lastAddedTile = null;
                        }
                    }
                }
            }
            #endregion


            //When unit is moving
            if (isMoving)
            {
                CameraControl.Instance.UpdateCameraPan(currentSelectedUnitGO);
                MoveAlongPath(currentSelectedUnit, pathList, playerTilesInRange);
            }
        }

        #region Movement along Tile
        public void MoveAlongPath(Units unit, List<OverlayTile> pathList, List<OverlayTile> tilesInRange)
        {
            //units movement
            if(pathList.Count > 0) 
            {
                var step = movingSpeed * Time.deltaTime;
                var zIndex = pathList[0].transform.position.z;

                unit.transform.position = Vector2.MoveTowards(unit.transform.position, pathList[0].transform.position, step);
                unit.transform.position = new Vector3(unit.transform.position.x, unit.transform.position.y, zIndex);

                if (Vector2.Distance(unit.transform.position, pathList[0].transform.position) < 0.01f)
                {
                    SetUnitPositionOnTile(pathList[0], unit.gameObject);

                    pathList[0].SetArrowSprite(ArrowDirections.None);

                    pathList.RemoveAt(0);
                }
            }

            //set the units direction facing
            if (pathList.Count > 0 && unit != null)
            {
                Vector3Int direction = pathList[0].gridLocation - unit.ActiveTile.gridLocation;

                if (direction == new Vector3Int(1, 0, 0))
                {
                    unit.Direction = Direction.North;
                }
                else if (direction == new Vector3Int(-1, 0, 0))
                {
                    unit.Direction = Direction.South;
                }
                else if (direction == new Vector3Int(0, 1, 0))
                {
                    unit.Direction = Direction.West;
                }
                else if (direction == new Vector3Int(0, -1, 0))
                {
                    unit.Direction = Direction.East;
                }
            }

            if (pathList.Count <= 0)
            {
                //Comment this out later
                //HideTilesInRange(tilesInRange);
                //ifSelectedUnit = false;
                isMoving = false;
            }
        }
        #endregion


        #region Overlay Tile Related
        bool AreTilesAdjacent(OverlayTile tile1, OverlayTile tile2)
        {
            if((Mathf.Abs(tile2.gridLocation.x - tile1.gridLocation.x) == 1) && tile1.gridLocation.y == tile2.gridLocation.y)
            {
                return true;
            }

            if ((Mathf.Abs(tile2.gridLocation.y - tile1.gridLocation.y) == 1) && tile1.gridLocation.x == tile2.gridLocation.x)
            {
                return true;
            }

            return false;
        }


        void RenderArrow()
        {
            foreach (var item in playerTilesInRange)
            {
                item.SetArrowSprite(ArrowDirections.None);
            }

            for (int i = 0; i < pathList.Count; i++)
            {
                var prevTile = i > 0 ? pathList[i - 1] : currentSelectedUnit.ActiveTile;
                var futTile = i < pathList.Count - 1 ? pathList[i + 1] : null;

                var arrowDir = ArrowRenderer.TranslateDirection(prevTile, pathList[i], futTile);
                pathList[i].SetArrowSprite(arrowDir);
            }
        }

        public RaycastHit2D? GetFocusedTile()
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Overlay Tile"));

            //Checks if the raycast has hit anything
            if (hits.Length > 0)
            {
                return hits.OrderByDescending(i => i.collider.transform.position.z).First();
            }

            return null;
        }

        private void SetUnitPositionOnTile(OverlayTile tile, GameObject go)
        {
            go.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z);
            go.GetComponent<Units>().ActiveTile = tile;
        }

        public void ShowTilesInRange(List<OverlayTile> overlayTileList)
        {
            //This hides the previous patterns once it starts moving again
            foreach (var item in overlayTileList)
            {
                item.HideTile();
            }

            //This displays all the tiles in range 
            foreach (var item in overlayTileList)
            {
                item.ShowMoveTile();
            }
        }

        public void HideTilesInRange(List<OverlayTile> tilesInRange)
        {
            foreach (var item in tilesInRange)
            {
                item.HideTile();
            }
        }

        public void ClearUnitPosition()
        {
            ClearArrow();
            revertUnitPosition = null;
            ifSelectedUnit = false;
        }

        public void ClearArrow()
        {
            foreach (var tile in tempPathList)
            {
                tile.SetArrowSprite(ArrowDirections.None);
            }

            tempPathList.Clear();
            pathList.Clear();
        }
        #endregion
    }
}
