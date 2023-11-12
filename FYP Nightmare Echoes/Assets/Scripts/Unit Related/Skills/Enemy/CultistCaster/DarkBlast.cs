using System.Collections;
using System.Collections.Generic;
using NightmareEchoes.Grid;
using UnityEngine;

//Created by JH (stolen and edited by ter)
namespace NightmareEchoes.Unit
{
    public class DarkBlast : Skill
    {
        public override bool Cast(Entity target)
        {
            base.Cast(target);

            StartCoroutine(Attack(target));

            return true;
        }

        IEnumerator Attack(Entity target)
        {
            yield return new WaitForSeconds(0.1f);

            //animation
            animationCoroutine = StartCoroutine(PlaySkillAnimation(thisUnit, "Attacking"));

            yield return new WaitUntil(() => animationCoroutine == null);

            OverlayTile targetTile = target.ActiveTile;
            List<Vector2Int> tilesToCheck = new List<Vector2Int>();

            tilesToCheck.Add(new Vector2Int((targetTile.gridLocation.x), (targetTile.gridLocation.y - 1)));
            tilesToCheck.Add(new Vector2Int((targetTile.gridLocation.x), (targetTile.gridLocation.y + 1)));
            tilesToCheck.Add(new Vector2Int((targetTile.gridLocation.x - 1), (targetTile.gridLocation.y)));
            tilesToCheck.Add(new Vector2Int((targetTile.gridLocation.x + 1), (targetTile.gridLocation.y)));

            List<OverlayTile> aoeTiles = OverlayTileManager.Instance.TrimOutOfBounds(tilesToCheck);

            DealDamage(target, default, checkBlind: false);

            aoeTiles.Remove(targetTile);

            foreach (var tile in aoeTiles)
            {
                if (!tile.CheckEntityGameObjectOnTile()) continue;

                var unit = tile.CheckEntityGameObjectOnTile().GetComponent<Entity>();

                DealDamage(unit, secondaryDamage, checkBlind: false);
                Knockback(targetTile, unit);
            }
        }
    }
}
