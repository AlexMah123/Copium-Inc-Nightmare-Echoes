using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Vinn
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

        private SpriteRenderer sr;
        private Color activeColor = new Color(1, 1, 1.5f, 0.5f);
        private Color inactiveColor = new Color(1, 1, 1.5f, 0f);

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
        }

        public void ShowTile()
        {
            sr.color = activeColor;
            Debug.Log(sr.color);
        }

        public void HideTile()
        {
            sr.color = inactiveColor;
            Debug.Log(sr.color);
        }
    }
}

