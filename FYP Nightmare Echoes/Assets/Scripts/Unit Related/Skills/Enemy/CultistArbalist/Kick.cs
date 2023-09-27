using System.Collections;
using NightmareEchoes.Grid;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    //Written by Ter (stolen from jh)
    public class Kick : Skill
    {
        public override bool Cast(Units target)
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
                    if (tileDestination.CheckUnitOnTile())
                        tileOccupied = true;
                }
            }
            if (!tileOccupied)
            {
                target.transform.position += direction;
            }
            
            return true;
        }
    }
}
