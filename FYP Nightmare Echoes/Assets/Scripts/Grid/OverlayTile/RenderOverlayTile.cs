using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//created by JH
namespace NightmareEchoes.Grid
{
    public class RenderOverlayTile : MonoBehaviour
    {
        public static RenderOverlayTile Instance;
        [SerializeField] private List<OverlayTile> targetingRenders;
        [SerializeField] private List<OverlayTile> aoePreviewRenders;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;
        }

        public void RenderAttackRangeTiles(List<OverlayTile> tiles)
        {
            foreach (var tile in tiles)
            {
                tile.ShowAttackTile();
            }

            targetingRenders = tiles;
        }

        public void RenderAoeTiles(List<OverlayTile> tiles)
        {
            foreach (var tile in aoePreviewRenders)
            {
                tile.HideTile();
            }
            aoePreviewRenders.Clear();
            
            foreach (var tile in tiles)
            {
                tile.ShowAoeTiles();
                aoePreviewRenders.Add(tile);
            }
        }

        public void RenderEnemyTiles(List<OverlayTile> list)
        {
            foreach (var tile in list)
            {
                foreach (var activeTile in targetingRenders.Where(activeTile => activeTile == tile))
                {
                    activeTile.ShowEnemyTile();
                    break;
                }
            }
        }
        
        public void RenderFriendlyTiles(List<OverlayTile> list)
        {
            foreach (var tile in list)
            {
                foreach (var activeTile in targetingRenders.Where(activeTile => activeTile == tile))
                {
                    activeTile.ShowFriendlyTile();
                    break;
                }
            }
        }

        public void ClearTargetingRenders()
        {
            foreach (var tile in targetingRenders)
            {
                tile.HideTile();
            }
            targetingRenders.Clear();
            
            foreach (var tile in aoePreviewRenders)
            {
                tile.HideTile();
            }
            aoePreviewRenders.Clear();
        }
    }
}
