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
            var direction = Vector3.Normalize(distance);
            var xMagnitude = (int)(distance.x / direction.x);
            var yMagnitude = (int)(distance.y / direction.y);
            if (xMagnitude > 0) xMagnitude -= 1;
            else if (xMagnitude < 0) xMagnitude = 0;
            if (yMagnitude > 0) yMagnitude -= 1;
            else if (yMagnitude < 0) yMagnitude = 0;
            var reducedMagnitude = new Vector2Int(xMagnitude, yMagnitude);
            var newPos = new Vector2Int(thisUnit.ActiveTile.gridLocation.x + reducedMagnitude.x, thisUnit.ActiveTile.gridLocation.y + reducedMagnitude.y);
            
            var destinationTile = OverlayTileManager.Instance.GetOverlayTile(newPos);

            if (destinationTile.CheckUnitOnTile() && destinationTile.CheckObstacleOnTile()) return false;
            
            thisUnit.transform.position = destinationTile.transform.position;
            thisUnit.UpdateLocation();

            base.Cast(target);
            DealDamage(target);

            Knockback(thisUnit.ActiveTile, target);

            return true;
        }
    }
}
