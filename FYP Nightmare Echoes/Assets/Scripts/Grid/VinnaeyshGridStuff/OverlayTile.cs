using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Grid
{
   public class OverlayTile : MonoBehaviour
    {
        public int G;
        public int H;

        public int F { get { return G + H; } }

        public bool isBlocked;
        public OverlayTile prevTile;

        public Vector3Int gridLocation;
        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HideTile();
            }
        }

        public void ShowTile()
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1.5f, 0.5f);
        }

        public void HideTile()
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1.5f, 0f);

        }
    }
}

