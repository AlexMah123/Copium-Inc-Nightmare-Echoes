using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NightmareEchoes.Grid;
using NightmareEchoes.Unit.Combat;
using UnityEngine;

//Created by JH
namespace NightmareEchoes.Unit
{
    public class Teleport : Skill
    {
        private bool enableTargeting;
        private OverlayTile targetTile;
        private List<OverlayTile> tileRanges;

        private void Update()
        {
            if (!enableTargeting) return;
            if (!Input.GetMouseButtonDown(0)) return;
            var target = OverlayTileManager.Instance.GetOverlayTileOnMouseCursor();
            if (!target) return;
            if (target.CheckEntityGameObjectOnTile()) return;
            if (target.CheckObstacleOnTile()) return;
            
            if (tileRanges.All(tile => tile != target)) return;

            targetTile = target;
        }

        public override bool Cast(Entity target)
        {
            if (target.IsProp) return false;

            if (!enableTargeting)
            {
                enableTargeting = true;
                CombatManager.Instance.SecondaryTargeting();
                StartCoroutine(CastTeleport(target));
            }

            return GetDestination();
        }

        private bool GetDestination()
        {
            return targetTile;
        }

        public override void Reset()
        {
            enableTargeting = false;
            targetTile = null;
        }

        private IEnumerator CastTeleport(Entity targetUnit)
        {
            var cm = CombatManager.Instance;
            var range = cm.SquareRange(targetUnit.ActiveTile , secondaryRange);
            tileRanges = OverlayTileManager.Instance.TrimOutOfBounds(range);
            cm.SetCustomRange(tileRanges);

            yield return new WaitUntil(GetDestination);


            base.Cast(targetUnit);

            //animations
            yield return new WaitForSeconds(0.1f);
            animationCoroutine = StartCoroutine(PlaySkillAnimation(thisUnit, "Attacking"));

            yield return new WaitUntil(() => animationCoroutine == null);
            targetUnit.transform.position = targetTile.transform.position;
            targetUnit.UpdateLocation();
        }
    }
}
