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
            var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Overlay Tile"));
            if (!hit) return;
            var target = hit.collider.gameObject.GetComponent<OverlayTile>();
            if (!target) return;
            if (target.CheckUnitOnTile()) return;
            
            if (tileRanges.All(tile => tile != target)) return;

            targetTile = target;
        }

        public override bool Cast(Units target)
        {
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

        private IEnumerator CastTeleport(Units targetUnit)
        {
            var cm = CombatManager.Instance;
            var range = cm.SquareRange(targetUnit.ActiveTile, secondaryRange);
            tileRanges = OverlayTileManager.Instance.TrimOutOfBounds(range);
            cm.SetCustomRange(tileRanges);

            yield return new WaitUntil(GetDestination);
            
            targetUnit.transform.position = targetTile.transform.position;
            targetUnit.UpdateLocation();
        }
    }
}
