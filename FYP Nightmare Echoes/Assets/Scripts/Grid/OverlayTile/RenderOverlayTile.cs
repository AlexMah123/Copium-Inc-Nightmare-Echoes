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
        [SerializeField] private List<OverlayTile> activeRenders;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;
        }

        public void RenderTiles(List<OverlayTile> tiles)
        {
            foreach (var tile in tiles)
            {
                tile.ShowAttackTile();
            }

            activeRenders = tiles;
        }

        public void RenderEnemyTiles(List<OverlayTile> list)
        {
            foreach (var tile in list)
            {
                foreach (var activeTile in activeRenders.Where(activeTile => activeTile == tile))
                {
                    activeTile.ShowEnemyTile();
                    break;
                }
            }
        }

        public void ClearRenders()
        {
            foreach (var tile in activeRenders)
            {
                tile.HideTile();
            }
            activeRenders.Clear();
        }
    }
}
