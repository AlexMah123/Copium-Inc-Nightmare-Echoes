using NightmareEchoes.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class Charge : Skill
    {
        public override bool Cast(Units unit)
        {
            base.Cast(unit);
            DealDamage(unit);

            var direction = unit.transform.position - transform.position;
            var destination = unit.transform.position + direction;

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

            if (tileOccupied) return true;
            unit.transform.position += direction;
            unit.UpdateLocation();

            return true;
        }
    }
}
