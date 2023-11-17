using System.Collections;
using System.Collections.Generic;
using NightmareEchoes.Grid;
using NightmareEchoes.Sound;
using NightmareEchoes.Unit.Combat;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class StaticStorm : Skill
    {
        public override bool Cast(Entity target)
        {
            DealDamage(target);

            //AudioManager.instance.PlaySFX("StaticStorm");
            return true;
        }

        public override bool Cast(OverlayTile target, List<OverlayTile> aoeTiles)
        {
            base.Cast(target, aoeTiles);

            StartCoroutine(Attack(target, new List<OverlayTile>(aoeTiles)));

            //AudioManager.instance.PlaySFX("StaticStorm");
            return true;
        }

        IEnumerator Attack(OverlayTile target, List<OverlayTile> aoeTiles)
        {
            yield return new WaitForSeconds(0.1f);

            //animations
            animationCoroutine = StartCoroutine(PlaySkillAnimation(thisUnit, "Attacking"));

            yield return new WaitUntil(() => animationCoroutine == null);

            var copy = new List<OverlayTile>(aoeTiles);

            foreach (var tile in aoeTiles)
            {
                if (!tile.CheckEntityGameObjectOnTile()) continue;

                var unit = tile.CheckEntityGameObjectOnTile().GetComponent<Entity>();

                if (DealDamage(unit, default, checkBlind: false))
                {
                    if ((DebuffChance - unit.stats.Resist) > Random.Range(0, 101))
                    {
                        unit.AddBuff(GetStatusEffect.CreateModifier(STATUS_EFFECT.WEAKEN_TOKEN, 1, 1));
                    }
                }
            }

            CombatManager.Instance.SetActiveAoe(this, copy);
        }
    }
}
