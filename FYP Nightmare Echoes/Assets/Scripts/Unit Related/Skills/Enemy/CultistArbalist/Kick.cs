using System.Collections;
using NightmareEchoes.Grid;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    //Written by Ter (stolen from jh)
    public class Kick : Skill
    {
        Direction prevDir;
        public override bool Cast(Entity target)
        {
            base.Cast(target);

            DealDamage(target);


            //knockback
            var direction = target.transform.position - thisUnit.transform.position;
            var destination = target.transform.position + direction;
            
            var tileOccupied = false;
            var hit = Physics2D.Raycast(destination, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Overlay Tile"));
            if (hit)
            {
                var tileDestination = hit.collider.gameObject.GetComponent<OverlayTile>();
                if (tileDestination)
                {
                    if (tileDestination.CheckUnitOnTile() || tileDestination.CheckObstacleOnTile())
                        tileOccupied = true;

                    if (!tileOccupied)
                    {
                        prevDir = target.Direction;
                        StartCoroutine(Pathfinding.PathfindingManager.Instance.MoveTowardsTile(target, tileDestination, 0.15f));
                        StartCoroutine(DelayTurn(target));
                        thisUnit.CheckCrippled();
                    }
                }
            }
            
            
            return true;
        }
        IEnumerator DelayTurn(Entity target)
        {
            yield return new WaitForSeconds(0.16f);
            Debug.Log(prevDir);
            target.Direction = prevDir;

        }

        }

}
