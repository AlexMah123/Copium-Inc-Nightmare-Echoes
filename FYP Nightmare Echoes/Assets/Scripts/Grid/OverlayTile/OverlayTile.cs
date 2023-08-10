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

        public bool isCurenttlyStandingOn = false;

        public int F { get { return G + H; } }

        public bool isBlocked;
        public OverlayTile prevTile;
        public Vector3Int gridLocation;
        public bool PlayerOnTile = false;

        private SpriteRenderer sr;
        [SerializeField] Color inactiveColor;

        [SerializeField] Color moveColor;
        [SerializeField] Color attackRangeColor;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
        }

        public void ShowAttackTile()
        {
            sr.color = new Color(attackRangeColor.r,attackRangeColor.g,attackRangeColor.b,attackRangeColor.a);
        }
        
        public void ShowMoveTile()
        {
            sr.color = new Color(moveColor.r,moveColor.g,moveColor.b,moveColor.a);;
        }

        public void HideTile()
        {
            sr.color = new Color(inactiveColor.r,inactiveColor.g,inactiveColor.b,inactiveColor.a);;
        }

        public void isPlayerOnTile()
        {
            if (PlayerOnTile == true)
            {
                HideTile();
            }
            else if (PlayerOnTile == false)
            { 
                ShowMoveTile();
            }
        }
    }
}

