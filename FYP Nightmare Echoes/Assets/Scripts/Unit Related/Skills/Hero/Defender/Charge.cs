using NightmareEchoes.Grid;
using System.Collections;
using System.Collections.Generic;
using NightmareEchoes.Unit.Pathfinding;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class Charge : Skill
    {
        public override bool Cast(Entity target)
        {
            var distance = target.ActiveTile.gridLocation - thisUnit.ActiveTile.gridLocation;
            
            //Skip everything below if target is just in front of ya
            if (Mathf.Abs(distance.x) == 1 || Mathf.Abs(distance.y) == 1)
            {
                StartCoroutine(DelayedAttack(target));
                return true;
            }

            if (thisUnit.CheckImmobilize()) return false;
            
            var xDist = distance.x;
            var yDist = distance.y;
            if (xDist > 0) xDist -= 1; else if (xDist < 0) xDist += 1;
            if (yDist > 0) yDist -= 1; else if (yDist < 0) yDist += 1;

            var newPos = new Vector2Int(thisUnit.ActiveTile.gridLocation.x + xDist, thisUnit.ActiveTile.gridLocation.y + yDist);
            
            var destinationTile = OverlayTileManager.Instance.GetOverlayTile(newPos);

            if (destinationTile.CheckEntityGameObjectOnTile() || destinationTile.CheckObstacleOnTile()) return false;
            
            //Super redundant way for checking tile
            if (Mathf.Abs(distance.x) == 3 || Mathf.Abs(distance.y) == 3)
            {
                if (xDist > 0) xDist -= 1; else if (xDist < 0) xDist += 1;
                if (yDist > 0) yDist -= 1; else if (yDist < 0) yDist += 1;
                
                var frontalTilePos = new Vector2Int(thisUnit.ActiveTile.gridLocation.x + xDist, thisUnit.ActiveTile.gridLocation.y + yDist);
                var frontalTile = OverlayTileManager.Instance.GetOverlayTile(frontalTilePos);
                if (frontalTile.CheckEntityGameObjectOnTile() || frontalTile.CheckObstacleOnTile()) return false;
            }

            //animations
            StartCoroutine(PathfindingManager.Instance.MoveTowardsTile(thisUnit, destinationTile, 0.25f));
            StartCoroutine(DelayedAttack(target));
            
            return true;
        }
        
        IEnumerator DelayedAttack(Entity target)
        {
            base.Cast(target);
            yield return new WaitForSeconds(0.3f);
            //animations
            animationCoroutine = StartCoroutine(PlaySkillAnimation(thisUnit, "Charge"));

            yield return new WaitUntil (() => animationCoroutine == null);

            if (DealDamage(target))
            {
                Knockback(thisUnit.ActiveTile, target);
                if (stunChance - target.stats.StunResist > Random.Range(1,101))
                {
                    target.AddBuff(GetStatusEffect.CreateModifier(STATUS_EFFECT.STUN_TOKEN, 1, 1));
                }
            }
        }
    }
}
