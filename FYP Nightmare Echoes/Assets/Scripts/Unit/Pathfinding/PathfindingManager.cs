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

        List<OverlayTile> pathList = new List<OverlayTile>();
        List<OverlayTile> tempList = new List<OverlayTile>();
        [SerializeField] List<OverlayTile> tilesInRange = new List<OverlayTile>();

        RaycastHit2D? focusedTile;
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
        }

        private void Start()
        {

        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ifSelectedUnit = false;
                HideTilesInRange(tilesInRange);
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
                            tilesInRange = PathFinding.FindTilesInRange(currentSelectedUnit.ActiveTile, currentSelectedUnit.stats.MoveRange);

                            ShowTilesInRange(tilesInRange);
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
            
            focusedTile = GetFocusedTile();

            if (focusedTile.HasValue)
            {
                overlayTile = focusedTile.Value.collider.GetComponent<OverlayTile>();
                transform.position = overlayTile.transform.position;

                if (Input.GetMouseButtonDown(0) && ifSelectedUnit)
                {
                    if (currentSelectedUnitGO != null)
                    {

                        if (!overlayTile.CheckUnitOnTile())
                        {
                            pathList = PathFinding.FindPath(currentSelectedUnit.ActiveTile, overlayTile, tilesInRange);
                        }
                    }
                    
                }
            }

            if (pathList.Count > 0)
            {
                CameraControl.Instance.UpdateCameraPan(currentSelectedUnitGO);
                MoveAlongPath(currentSelectedUnitGO, pathList, tilesInRange);
            }

            
        }

        #region Movement along Tile
        public void MoveAlongPath(GameObject go, List<OverlayTile> pathList, List<OverlayTile> tilesInRange)
        {
            var step = movingSpeed * Time.deltaTime;
            var zIndex = pathList[0].transform.position.z;

            go.transform.position = Vector2.MoveTowards(go.transform.position, pathList[0].transform.position, step);
            go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, zIndex);

            if (Vector2.Distance(go.transform.position, pathList[0].transform.position) < 0.0001f)
            {
                SetUnitPositionOnTile(pathList[0], go);
                pathList.RemoveAt(0);
            }

            if (pathList.Count == 0)
            {
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
