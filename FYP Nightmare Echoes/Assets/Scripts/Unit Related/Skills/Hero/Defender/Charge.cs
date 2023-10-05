using NightmareEchoes.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class Charge : Skill
    {
        public override bool Cast(Entity target)
        {
            var distance = target.ActiveTile.gridLocation - thisUnit.ActiveTile.gridLocation;
            var xDist = distance.x;
            var yDist = distance.y;
            if (xDist > 0) xDist -= 1; else if (xDist < 0) xDist += 1;
            if (yDist > 0) yDist -= 1; else if (yDist < 0) yDist += 1;

            var newPos = new Vector2Int(thisUnit.ActiveTile.gridLocation.x + xDist, thisUnit.ActiveTile.gridLocation.y + yDist);
            
            var destinationTile = OverlayTileManager.Instance.GetOverlayTile(newPos);

            if (destinationTile.CheckUnitOnTile() || destinationTile.CheckObstacleOnTile()) return false;

            StartCoroutine(Pathfinding.PathfindingManager.Instance.MoveTowardsTile(thisUnit, destinationTile, 0.15f));
            StartCoroutine(DelayedAttack(target));
            
            return true;
        }
        
        IEnumerator DelayedAttack(Entity target)
        {
            yield return new WaitForSeconds(0.16f);
            base.Cast(target);
            DealDamage(target);
            
            if (target)
                Knockback(thisUnit.ActiveTile, target);
        }
    }
}
