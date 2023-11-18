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
            if(CombatManager.Instance)
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
            
            if (target.HasMoved)
            {
                base.Cast(target);
                DealDamage(target);
                return true;
            }
            
            if (target.HasAttacked)
            {
                base.Cast(target);
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

        IEnumerator Attack(Entity target)
        {
            yield return new WaitForSeconds(0.1f);
            //animation
            animationCoroutine = StartCoroutine(PlaySkillAnimation(thisUnit, "Hunter's Instinct Start"));

            yield return new WaitUntil(() => animationCoroutine == null);

        }
    }
}
