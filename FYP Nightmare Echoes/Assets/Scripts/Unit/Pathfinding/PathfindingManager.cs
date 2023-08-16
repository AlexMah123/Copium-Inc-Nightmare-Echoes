using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using NightmareEchoes.Grid;
using NightmareEchoes.Inputs;
using PlasticPipe.PlasticProtocol.Messages;

//created by Vinn, editted by Alex and Ter
namespace NightmareEchoes.Unit.Pathfinding
{
    public class PathfindingManager : MonoBehaviour
    {
        public static PathfindingManager Instance;

        [SerializeField] GameObject OTC; 

        [Header("Current Unit")]
        [SerializeField] GameObject currentSelectedUnitGO;
        [SerializeField] float movingSpeed;
        Units currentSelectedUnit;
        bool cameraPanning = false;


        //Vinn test changes
        public string UnitTag;
        [SerializeField] List <GameObject> UnitsOnScreen = new List<GameObject>();
        [SerializeField] GameObject testUnitPos;
        [SerializeField] GameObject OverLayTileManagerObj;
        //OverlayTileManager tilemapManager;
        [SerializeField] bool testBool;

        [SerializeField] bool ifSelectedUnit = false;

        List<OverlayTile> path = new List<OverlayTile>();
        [SerializeField] List<OverlayTile> inRangeTiles = new List<OverlayTile>();
        public List<GameObject> UnitListOnScreen;

        RaycastHit2D? focusedTileHit;
        OverlayTile overlayTile;
        OverlayTileManager overlayTileManager;

        //Changes by Vinn
        [SerializeField] private LayerMask UnitLayer;

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
                RangeTilesOff();
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

                            GetInRangeTiles();
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
            
            focusedTileHit = GetFocusedTile();

            if (focusedTileHit.HasValue)
            {
                overlayTile = focusedTileHit.Value.collider.GetComponent<OverlayTile>();
                transform.position = overlayTile.transform.position;

                //gameObject.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder;

                if (Input.GetMouseButtonDown(0) && ifSelectedUnit)
                {
                    //commented out so it does not show tile randomly
                    //overlayTile.ShowTile();

                    if (currentSelectedUnitGO == null)
                    {
                        //PositionCharacterOnTile(overlayTile);
                    }
                    else if (currentSelectedUnitGO != null)
                    {

                        if (!overlayTile.CheckUnitOnTile())
                        {
                            path = PathFind.FindPath(currentSelectedUnit.ActiveTile, overlayTile, inRangeTiles);
                        }
                    }
                    
                }
            }

            if (path.Count > 0)
            {
                CameraControl.Instance.UpdateCameraPan(currentSelectedUnitGO);
                MoveAlongPath(currentSelectedUnitGO, path);
            }
        }

        #region Movement along Tile
        public void MoveAlongPath(GameObject go, List<OverlayTile> pathL)
        {
            var step = movingSpeed * Time.deltaTime;

            var zIndex = pathL[0].transform.position.z;

            go.transform.position = Vector2.MoveTowards(go.transform.position, pathL[0].transform.position, step);

            go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, zIndex);

            if (Vector2.Distance(go.transform.position, pathL[0].transform.position) < 0.0001f)
            {
                PositionCharacterOnTile(pathL[0], go);
                pathL.RemoveAt(0);
            }

            if (pathL.Count == 0)
            {
                //if i dont place this function here it wont render the tile range after it moves (Only on  the initial click)
                //GetInRangeTiles();
                RangeTilesOff();
                ifSelectedUnit = false;

                //RangeIsActive = false;  

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

        private void PositionCharacterOnTile(OverlayTile tile, GameObject go)
        {
            go.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z);
            go.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
            go.GetComponent<Units>().ActiveTile = tile;
        }

        private void GetInRangeTiles()
        {
            //This hides the previous patterns once it starts moving again
            foreach (var item in inRangeTiles)
            {
                item.HideTile();
            }

            //Gets the value of the start pos and the maximum range is the amount you can set
            inRangeTiles = RangeMovementFind.TileMovementRange(currentSelectedUnit.ActiveTile, currentSelectedUnit.stats.MoveRange);

            //This displays all the tiles in range 
            foreach (var item in inRangeTiles)
            {
                item.ShowMoveTile();
            }
        }

        private void RangeTilesOff()
        {
            foreach (var item in inRangeTiles)
            {
                item.HideTile();
            }
        }


        #endregion
    }
}
