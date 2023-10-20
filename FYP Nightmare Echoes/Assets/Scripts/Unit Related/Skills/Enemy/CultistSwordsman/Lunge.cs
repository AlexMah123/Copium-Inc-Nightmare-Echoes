using System.Collections;
using NightmareEchoes.Grid;
using System.Collections.Generic;
using NightmareEchoes.Unit.Pathfinding;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    //Written by Ter (stolen from jh)
    public class Lunge : Skill
    {
        public override bool Cast(Entity target)
        {
            var distance = target.ActiveTile.gridLocation - thisUnit.ActiveTile.gridLocation;

            //Skip everything below if target is just in front of ya
            if (Mathf.Abs(distance.x) == 1 || Mathf.Abs(distance.y) == 1)
            {
                base.Cast(target);

                if (DealDamage(target))
                {
                    target.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.WOUND_DEBUFF, 1, 2));
                }
                return true;
            }

            if (!thisUnit.CheckImmobilize())
            {
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

                StartCoroutine(PathfindingManager.Instance.MoveTowardsTile(thisUnit, destinationTile, 0.15f));
                StartCoroutine(DelayedAttack(target));
            }

            

            return true;
        }

        IEnumerator DelayedAttack(Entity target)
        {
            yield return new WaitForSeconds(0.16f);
            base.Cast(target);

            if (DealDamage(target))
            {
                target.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.WOUND_DEBUFF, 1, 2));
            }
        }

        /*public override bool Cast(Entity target)
        {


            base.Cast(target);

            //move towards
            var direction = target.transform.position - thisUnit.transform.position;
            var destination = thisUnit.transform.position + (direction/2);

            var tileOccupied = false;

            if (!thisUnit.CheckImmobilize())
            {
                var hit = Physics2D.Raycast(destination, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Overlay Tile"));
                if (hit)
                {
                    var tileDestination = hit.collider.gameObject.GetComponent<OverlayTile>();
                    if (tileDestination)
                    {
                        if (tileDestination.CheckEntityGameObjectOnTile() || tileDestination.CheckObstacleOnTile())
                            tileOccupied = true;

                        if (!tileOccupied)
                        {
                            StartCoroutine(Pathfinding.PathfindingManager.Instance.MoveTowardsTile(thisUnit, tileDestination, 0.15f));
                            if (DealDamage(target))
                            {
                                target.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.WOUND_DEBUFF, 1, 2));
                            }
                        }
                    }
                }
            }

            return true;
        }*/
    }
}
