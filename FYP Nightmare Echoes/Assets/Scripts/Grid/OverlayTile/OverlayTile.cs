using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NightmareEchoes.Grid.ArrowScript;
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
        public bool isClickable = true;

        private SpriteRenderer sr;
        [SerializeField] Color inactiveColor;

        [SerializeField] Color moveColor;
        [SerializeField] Color attackRangeColor; 
        [SerializeField] Color aoeRangeColor;

        [SerializeField] Color enemyColor;
        [SerializeField] Color friendlyColor;


        public List<Sprite> arrows;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
        }
        
        public void ShowMoveTile()
        {
            if(sr != null)
            {
                sr.color = ConvertColor(moveColor);

            }
        }
        
        public void ShowAttackTile()
        {
            if(sr != null) 
            {
                sr.color = ConvertColor(attackRangeColor);
            }
        }

        public void ShowAoeTiles()
        {
            if(sr != null)
            {
                sr.color = ConvertColor(aoeRangeColor);

            }
        }

        public void ShowEnemyTile()
        {
            if(sr != null)
            {
                sr.color = ConvertColor(enemyColor);

            }
        }

        public void ShowFriendlyTile()
        {
            if(sr != null)
            {
                sr.color = ConvertColor(friendlyColor);
            }
        }

        public void ShowCustomColor(Color c)
        {
            if(sr != null)
            {
                sr.color = ConvertColor(c);
            }
        }
        
        public void HideTile()
        {
            if (sr != null)
            {
                sr.color = ConvertColor(inactiveColor);
                SetArrowSprite(ArrowDirections.None);
            }
        }

        private Color ConvertColor(Color c)
        {
            return new Color(c.r, c.g, c.b, c.a);
        }

        public GameObject CheckUnitOnTile()
        {
            var hit = Physics2D.Raycast(transform.position, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Unit"));


            if (!hit ) return null;
            var target = hit.collider.gameObject;
            return !target ? null : target;
        }

        public GameObject CheckObstacleOnTile()
        {
            var hit = Physics2D.Raycast(transform.position, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Obstacles"));

            if (!hit) return null;
            var target = hit.collider.gameObject;
            return !target ? null : target;
        }

        public void SetArrowSprite(ArrowDirections d)
        {
            var arrow = GetComponentsInChildren<SpriteRenderer>()[1];
            if (d == ArrowDirections.None)
            {
                arrow.color = new Color(1, 1, 1, 0);
            }
            else
            {
                arrow.color = new Color(1, 1, 1, 1);
                arrow.sprite = arrows[(int)d];
            }
        }
    }
}

