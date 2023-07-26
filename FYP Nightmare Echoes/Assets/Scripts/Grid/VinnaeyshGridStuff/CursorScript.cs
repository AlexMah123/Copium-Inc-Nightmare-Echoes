using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

namespace NightmareEchoes.Grid
{
    public class CursorScript : MonoBehaviour
    {
        public float Speed;
        public GameObject characterPrefab;
        private CharacterData character;
        private Pathfinder pathFinder;
        private List<OverlayTile> path = new List<OverlayTile>();
        // Start is called before the first frame update
        void Start()
        {
            pathFinder = new Pathfinder();
        }

        // Update is called once per frame
        void LateUpdate()
        {
            var focusedTileHit = GetFocusedTile();

            if (focusedTileHit.HasValue)
            {
                OverlayTile overlayTile = focusedTileHit.Value.collider.gameObject.GetComponent<OverlayTile>();
                transform.position = overlayTile.transform.position;
                gameObject.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder;

                if (Input.GetMouseButtonDown(0))
                {
                    overlayTile.ShowTile();

                    if (character == null)
                    {
                        character = Instantiate(characterPrefab).GetComponent<CharacterData>();
                        PositionCharacterOnTile(overlayTile);

                    }
                    else
                    {
                        path = pathFinder.FindPath(character.activeTile,overlayTile);
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

            character.transform.position = Vector2.MoveTowards(character.transform.position, path[0].transform.position, step);

            character.transform.position = new Vector3(character.transform.position.x, character.transform.position.y,zIndex);

            if (Vector2.Distance(character.transform.position, path[0].transform.position) < 0.0001f)
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
            character.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z);
            character.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
            character.activeTile = tile;
        }
    }
}
