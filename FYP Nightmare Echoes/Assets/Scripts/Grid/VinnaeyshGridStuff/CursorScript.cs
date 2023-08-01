using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NightmareEchoes.Grid
{
    public class CursorScript : MonoBehaviour
    {
        public float Speed;
        public GameObject selectedUnit;
        private CharacterData characterData;
        private List<OverlayTile> path = new List<OverlayTile>();
        private RaycastHit2D? focusedTileHit;
        private OverlayTile overlayTile;
        [SerializeField] private GameObject OTC; //

        [SerializeField] private MapManager mapManager;
        [SerializeField] Vector2Int startPos;

        private void Start()
        {
            characterData = selectedUnit.GetComponent<CharacterData>();
        }

        void LateUpdate()
        {

            if (characterData.activeTile == null) //
            {
                RaycastHit2D hit = Physics2D.Raycast(selectedUnit.transform.position, Vector2.zero);

                if(hit)
                {
                    characterData.activeTile = hit.collider.GetComponent<OverlayTile>();
                }
            }
            
            if(Input.GetMouseButtonDown(0))
            {
                focusedTileHit = GetFocusedTile();

                if (focusedTileHit.HasValue)
                {
                    overlayTile = focusedTileHit.Value.collider.GetComponent<OverlayTile>();
                    transform.position = overlayTile.transform.position;

                    //gameObject.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder;

                    overlayTile.ShowTile();

                    if (selectedUnit == null)
                    {
                        //PositionCharacterOnTile(overlayTile);

                    }
                    else if (selectedUnit != null)
                    {
                        //characterPrefab.GetComponent<CharacterData>().activeTile = overlayTile;
                        path = Pathfinder.FindPath(selectedUnit.GetComponent<CharacterData>().activeTile, overlayTile);

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
            var step = Speed * Time.deltaTime;

            var zIndex = path[0].transform.position.z;

            selectedUnit.transform.position = Vector2.MoveTowards(selectedUnit.transform.position, path[0].transform.position, step);

            selectedUnit.transform.position = new Vector3(selectedUnit.transform.position.x, selectedUnit.transform.position.y,zIndex);

            if (Vector2.Distance(selectedUnit.transform.position, path[0].transform.position) < 0.0001f)
            {
                PositionCharacterOnTile(path[0]);
                path.RemoveAt(0);   
            }

        }
        public RaycastHit2D? GetFocusedTile()
        {
            //Converting mousePos to mousePos in the 2D world
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2D, Vector2.zero);

            //Checks if the raycast has hit anything
            if (hits.Length > 0)
            {
                return hits.OrderByDescending(i => i.collider.transform.position.z).First();
            }

            return null;
        }

        private void PositionCharacterOnTile(OverlayTile tile)
        {
            selectedUnit.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z);
            selectedUnit.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
            selectedUnit.GetComponent<CharacterData>().activeTile = tile;
        }
    }
}
