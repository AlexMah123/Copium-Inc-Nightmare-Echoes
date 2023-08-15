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
        List<OverlayTile> inRangeTiles = new List<OverlayTile>();
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
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

                int overlayTileMask = LayerMask.GetMask("Overlay Tile");
                int unitMask = LayerMask.GetMask("Unit");
                
                //Mouse Position to select unit
                RaycastHit2D hitUnit = Physics2D.Raycast(mousePos2D, Vector2.zero, Mathf.Infinity, unitMask);

                //if you hit a unit then get component
                if (hitUnit)
                {
                    var unit = hitUnit.collider.gameObject.GetComponent<Units>();
                    if (unit && !unit.IsHostile)
                    {
                        currentSelectedUnitGO = unit.gameObject;
                        currentSelectedUnit = unit;
                        ifSelectedUnit = true;
                        //overlayTile.PlayerOnTile = true;

                        RaycastHit2D hitOverlayTile = Physics2D.Raycast(currentSelectedUnitGO.transform.position, Vector2.zero, Mathf.Infinity, overlayTileMask);

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
                        path = PathFind.FindPath(currentSelectedUnitGO.GetComponent<Units>().ActiveTile, overlayTile, inRangeTiles);
                        //overlayTile.isCurenttlyStandingOn = false;

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
            //Converting mousePos to mousePos in the 2D world
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            int overlayTileMask = LayerMask.GetMask("Overlay Tile");

            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2D, Vector2.zero, Mathf.Infinity, overlayTileMask);

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

        private void UnitPos()
        { 
            
        }


        #endregion
    }
}
