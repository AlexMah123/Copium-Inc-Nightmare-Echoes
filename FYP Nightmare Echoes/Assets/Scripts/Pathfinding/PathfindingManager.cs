using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.XR;
using UnityEngine;

namespace NightmareEchoes.Pathfinding
{
    public class PathfindingManager : MonoBehaviour
    {
        [SerializeField] float speed;
        [SerializeField] GameObject currentUnit;
        [SerializeField] private GameObject OTC; //

        private CharacterData characterData;
        private List<OverlayTile> path = new List<OverlayTile>();
        private List<OverlayTile> inRangeTiles = new List<OverlayTile>(); 

        private RaycastHit2D? focusedTileHit;
        private OverlayTile overlayTile;
        [SerializeField] bool selectedUnit = false;
        [SerializeField] int RangeLimit = 1;

        private void Awake()
        {

        }

        private void Start()
        {
        }

        void Update()
        {

            if (Input.GetMouseButtonDown(0) && !selectedUnit)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

                int overlayTileMask = LayerMask.GetMask("Overlay Tile");
                int unitMask = LayerMask.GetMask("Unit");

                RaycastHit2D hitUnit = Physics2D.Raycast(mousePos2D, Vector2.zero, Mathf.Infinity, unitMask);

                if (hitUnit)
                {
                    if (hitUnit.collider.gameObject.GetComponent<CharacterData>())
                    {
                        currentUnit = hitUnit.collider.gameObject;
                        characterData = currentUnit.GetComponent<CharacterData>();
                        selectedUnit = true;

                        if (characterData.activeTile == null)
                        {
                            RaycastHit2D hitOverlayTile = Physics2D.Raycast(currentUnit.transform.position, Vector2.zero, Mathf.Infinity, overlayTileMask);

                            if (hitOverlayTile.collider.gameObject.GetComponent<OverlayTile>())
                            {
                                characterData.activeTile = hitOverlayTile.collider.GetComponent<OverlayTile>();
                                GetInRangeTiles();
                            }
                        }
                    }
                    else
                    {
                        selectedUnit = false;
                    }
                }
            }
            

            //if (Input.GetMouseButtonDown(0) && selectedUnit)
            {
                focusedTileHit = GetFocusedTile();

                if (focusedTileHit.HasValue)
                {
                    overlayTile = focusedTileHit.Value.collider.GetComponent<OverlayTile>();
                    transform.position = overlayTile.transform.position;

                    //gameObject.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder;

                    if(Input.GetMouseButtonDown(0) && selectedUnit)
                    {
                        overlayTile.ShowTile();

                        if (currentUnit == null)
                        {
                            //PositionCharacterOnTile(overlayTile);

                        }
                        else if (currentUnit != null)
                        {
                            //characterPrefab.GetComponent<CharacterData>().activeTile = overlayTile;
                            path = PathFind.FindPath(currentUnit.GetComponent<CharacterData>().activeTile, overlayTile , inRangeTiles);

                        }
                    }

               
                }
            }
              
            if (path.Count > 0)
            {
                MoveAlongPath();
            }

        }


        //Movement for player
        private void MoveAlongPath()
        {
            var step = speed * Time.deltaTime;

            var zIndex = path[0].transform.position.z;

            currentUnit.transform.position = Vector2.MoveTowards(currentUnit.transform.position, path[0].transform.position, step);

            currentUnit.transform.position = new Vector3(currentUnit.transform.position.x, currentUnit.transform.position.y, zIndex);

            if (Vector2.Distance(currentUnit.transform.position, path[0].transform.position) < 0.0001f)
            {
                PositionCharacterOnTile(path[0]);
                path.RemoveAt(0);   
            }

            if(path.Count == 0)
            {
                selectedUnit = false;
                //if i dont place this function here it wont render the tile range after it moves (Only on  the initial click)
                //GetInRangeTiles();
                RangeTilesOff();
                //RangeIsActive = false;  

            }

        }

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

        private void PositionCharacterOnTile(OverlayTile tile)
        {
            currentUnit.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z);
            currentUnit.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
            currentUnit.GetComponent<CharacterData>().activeTile = tile;
        }

        private void GetInRangeTiles()
        {
                //This hites the previous patterns once it starts moving again
                foreach (var item in inRangeTiles)
                {
                    item.HideTile();
                }

                //Gets the value of the start pos and the maximum range is the amount you can set
                inRangeTiles = RangeMovementFind.TileMovementRange(characterData.activeTile, RangeLimit);

                //This displays all the tiles in range 
                foreach (var item in inRangeTiles)
                {
                    item.ShowTile();
                }
        }

        private void RangeTilesOff()
        {
            foreach (var item in inRangeTiles)
            {
                item.HideTile();
            }
        }
    }
}
