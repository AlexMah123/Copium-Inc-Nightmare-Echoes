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
        [SerializeField] Color aoeRangeColor;
        
        [SerializeField] Color enemyColor;
        [SerializeField] Color friendlyColor;
        
        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
        }
        
        public void ShowMoveTile()
        {
            sr.color = ConvertColor(moveColor);
        }
        
        public void ShowAttackTile()
        {
            sr.color = ConvertColor(attackRangeColor);
        }

        public void ShowAoeTiles()
        {
            sr.color = ConvertColor(aoeRangeColor);
        }

        public void ShowEnemyTile()
        {
            sr.color = ConvertColor(enemyColor);
        }

        public void ShowFriendlyTile()
        {
            sr.color = ConvertColor(friendlyColor);
        }
        
        public void HideTile()
        {
            sr.color = ConvertColor(inactiveColor);
        }

        private Color ConvertColor(Color c)
        {
            return new Color(c.r, c.g, c.b, c.a);
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

