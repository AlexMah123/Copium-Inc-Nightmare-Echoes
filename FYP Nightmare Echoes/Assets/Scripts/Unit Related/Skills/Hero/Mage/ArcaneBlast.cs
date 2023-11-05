using System.Collections;
using System.Collections.Generic;
using NightmareEchoes.Grid;
using NightmareEchoes.Sound;
using UnityEngine;

//Created by JH
namespace NightmareEchoes.Unit
{
    public class ArcaneBlast : Skill
    {
        public override bool Cast(OverlayTile target, List<OverlayTile> aoeTiles)
        {
            base.Cast(target, aoeTiles);

            StartCoroutine(Attack(target, new List<OverlayTile>(aoeTiles)));

            
            //AudioManager.instance.PlaySFX("ArcaneBlast");
            return true;
        }

        IEnumerator Attack(OverlayTile target, List<OverlayTile> aoeTiles)
        {
            yield return new WaitForSeconds(0.1f);

            //animation
            animationCoroutine = StartCoroutine(PlaySkillAnimation(thisUnit, "Attacking"));

            yield return new WaitUntil(() => animationCoroutine == null);

            if (target.CheckEntityGameObjectOnTile())
            {
                var unit = target.CheckEntityGameObjectOnTile().GetComponent<Entity>();
                DealDamage(unit, default, checkBlind: false);
            }

            aoeTiles.Remove(target);

            foreach (var tile in aoeTiles)
            {
                if (!tile.CheckEntityGameObjectOnTile()) continue;

                var unit = tile.CheckEntityGameObjectOnTile().GetComponent<Entity>();

                DealDamage(unit, secondaryDamage, checkBlind: false);
                Knockback(target, unit);
            }
        }
    }
}
