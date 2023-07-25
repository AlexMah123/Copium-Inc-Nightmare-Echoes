using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NightmareEchoes.Grid
{
    public class CursorScript : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void LateUpdate()
        {
            var focusedTileHit = GetFocusedTile();

            if (focusedTileHit.HasValue)
            {
                GameObject overlayTile = focusedTileHit.Value.collider.gameObject;
                transform.position = overlayTile.transform.position;
                gameObject.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder;

                if (Input.GetMouseButtonDown(0))
                {
                    overlayTile.GetComponent<OverlayTile>().ShowTile();
                }
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
    }
}
