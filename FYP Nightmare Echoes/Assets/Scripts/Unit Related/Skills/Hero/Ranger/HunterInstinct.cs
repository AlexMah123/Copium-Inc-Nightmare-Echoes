using System;
using NightmareEchoes.Grid;
using NightmareEchoes.Unit.Combat;
using System.Collections;
using System.Collections.Generic;
using NightmareEchoes.Unit.AI;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class HunterInstinct : Skill
    {
        private bool isActive;
        private OverlayTile currentPos;
        private Entity unit;

        private void Start()
        {
            unit = gameObject.GetComponent<Entity>();
        }

        private void Update()
        {
            isActive = CombatManager.Instance.ActiveAoes.ContainsKey(this);

            if (!isActive) return;
            if (unit.ActiveTile != currentPos)
            {
                CombatManager.Instance.ClearActiveAoe(this);
            }
        }

        public override bool Cast(Entity target)
        {
            if (!target.IsHostile || target.IsProp) return false;
            
            if (target.GetComponent<BasicEnemyAI>().hasMoved)
            {
                DealDamage(target);
                return true;
            }
            
            if (target.GetComponent<BasicEnemyAI>().hasAttacked)
            {
                DealDamage(target, secondaryDamage);
                return true;
            }

            return false;
        }

        public override bool Cast(OverlayTile target, List<OverlayTile> aoeTiles)
        {
            var copy = new List<OverlayTile>(aoeTiles);
            CombatManager.Instance.SetActiveAoe(this, copy);
            base.Cast(target, aoeTiles);
            currentPos = unit.ActiveTile;
            return true;
        }
    }
}
