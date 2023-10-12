using System.Collections.Generic;
using NightmareEchoes.Grid;
using UnityEngine;

//Created by JH (stolen and edited by ter)
namespace NightmareEchoes.Unit
{
    public class Fireball : Skill
    {
        public override bool Cast(Entity target)
        {
            OverlayTile targetTile = target.ActiveTile;
            List<Vector2Int> tilesToCheck = new List<Vector2Int>();

            tilesToCheck.Add(new Vector2Int((targetTile.gridLocation.x), (targetTile.gridLocation.y - 1)));
            tilesToCheck.Add(new Vector2Int((targetTile.gridLocation.x), (targetTile.gridLocation.y + 1)));
            tilesToCheck.Add(new Vector2Int((targetTile.gridLocation.x - 1), (targetTile.gridLocation.y)));
            tilesToCheck.Add(new Vector2Int((targetTile.gridLocation.x + 1), (targetTile.gridLocation.y)));

            List<OverlayTile> aoeTiles = OverlayTileManager.Instance.TrimOutOfBounds(tilesToCheck);

            base.Cast(targetTile, aoeTiles);

            DealDamage(target);
            /*if (targetTile.CheckEntityGameObjectOnTile())
            {
                var unit = targetTile.CheckEntityGameObjectOnTile().GetComponent<Entity>();
                DealDamage(unit);
            }*/

            aoeTiles.Remove(targetTile);

            foreach (var tile in aoeTiles)
            {
                if (!tile.CheckEntityGameObjectOnTile()) continue;

                var unit = tile.CheckEntityGameObjectOnTile().GetComponent<Entity>();

                //manually check for token
                if (thisUnit.WeakenToken)
                {
                    int newDamage = Mathf.RoundToInt(secondaryDamage * 0.5f);
                    unit.TakeDamage(newDamage);

                    thisUnit.ShowPopUpText($"Attack was weakened!", Color.red);
                    thisUnit.UpdateTokenLifeTime(STATUS_EFFECT.WEAKEN_TOKEN);
                }
                else if (thisUnit.StrengthToken)
                {
                    int newDamage = Mathf.RoundToInt(secondaryDamage * 1.5f);
                    unit.TakeDamage(newDamage);

                    thisUnit.ShowPopUpText($"Attack was strengthen!", Color.red);
                    thisUnit.UpdateTokenLifeTime(STATUS_EFFECT.STRENGTH_TOKEN);
                }
                else
                {
                    unit.TakeDamage(secondaryDamage);
                }

                Knockback(targetTile, unit);
            }

            return true;
        }
    }
}
