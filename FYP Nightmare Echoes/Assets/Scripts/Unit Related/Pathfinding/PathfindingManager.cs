using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using NightmareEchoes.Grid;
using NightmareEchoes.Inputs;
using static NightmareEchoes.Grid.ArrowScript;
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

        [Header("Arrow Renderer")]
        ArrowScript arrowScript;
        bool isMoving = false;


        List<OverlayTile> pathList = new List<OverlayTile>();
        public List<OverlayTile> playerTilesInRange = new List<OverlayTile>();

        RaycastHit2D? hoveredTile;
        OverlayTile overlayTile;
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

        private void Start()
        {
            arrowScript = new ArrowScript();
        }


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ifSelectedUnit = false;
                HideTilesInRange(playerTilesInRange);
            }

            PlayerInputPathfinding();
        }

        public void PlayerInputPathfinding()
        { 
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
            
            hoveredTile = GetFocusedTile();

            if (hoveredTile.HasValue)
            {
                overlayTile = hoveredTile.Value.collider.GetComponent<OverlayTile>();
                transform.position = overlayTile.transform.position;

                if (playerTilesInRange.Contains(overlayTile) && !isMoving)
                {
                    pathList = PathFinding.FindPath(currentSelectedUnit.ActiveTile, overlayTile, playerTilesInRange);

                    foreach (var item in playerTilesInRange)
                    {
                        item.SetArrowSprite(ArrowDirections.None);
                    }

                    for (int i = 0; i < pathList.Count; i++)
                    {
                        var prevTile = i > 0 ? pathList[i - 1] : currentSelectedUnit.ActiveTile;
                        var futTile = i < pathList.Count - 1 ? pathList[i + 1] : null;

                        var arrowDir = arrowScript.TranslateDirection(prevTile, pathList[i], futTile);
                        pathList[i].SetArrowSprite(arrowDir);
                    }
                }

                if (Input.GetMouseButtonDown(0) && ifSelectedUnit)
                {
                    if (currentSelectedUnitGO != null)
                    {
                        if (!overlayTile.CheckUnitOnTile() && !overlayTile.CheckObstacleOnTile())
                        {
                            //pathList = PathFinding.FindPath(currentSelectedUnit.ActiveTile, overlayTile, playerTilesInRange);
                            isMoving = true;
                        }
                    }
                }
            }
            if (pathList.Count > 0)
            {
                CameraControl.Instance.UpdateCameraPan(currentSelectedUnitGO);
                MoveAlongPath(currentSelectedUnit, pathList, playerTilesInRange);
                isMoving = false;
            }
        }

        #region Movement along Tile
        public void MoveAlongPath(Units unit, List<OverlayTile> pathList, List<OverlayTile> tilesInRange)
        {
            var step = movingSpeed * Time.deltaTime;
            var zIndex = pathList[0].transform.position.z;

            unit.transform.position = Vector2.MoveTowards(unit.transform.position, pathList[0].transform.position, step);
            unit.transform.position = new Vector3(unit.transform.position.x, unit.transform.position.y, zIndex);

            if (Vector2.Distance(unit.transform.position, pathList[0].transform.position) < 0.01f)
            {
                SetUnitPositionOnTile(pathList[0], unit.gameObject);
                pathList.RemoveAt(0);
            }

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
                HideTilesInRange(tilesInRange);
                ifSelectedUnit = false;
            }
        }
        #endregion


        #region Overlay Tile Related
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


        #endregion
    }
}
