using System.Collections;
using NightmareEchoes.Grid;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    //Written by Ter (stolen from jh)
    public class Lunge : Skill
    {
        public override bool Cast(Units target)
        {
            base.Cast(target);

            //move towards
            var direction = target.transform.position - thisUnit.transform.position;
            var destination = thisUnit.transform.position + (direction/2);

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
                thisUnit.transform.position += direction;
            }

            
            if(DealDamage(target))
            {
                target.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.WOUND_DEBUFF, 1, 2));
            }

            return true;
        }
    }
}
