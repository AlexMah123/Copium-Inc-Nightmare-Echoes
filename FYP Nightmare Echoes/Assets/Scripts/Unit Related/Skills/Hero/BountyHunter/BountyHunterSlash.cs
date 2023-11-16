using System.Collections;
using System.Collections.Generic;
using NightmareEchoes.Grid;
using NightmareEchoes.Unit.Combat;
using UnityEngine;

//Created by Vinn
namespace NightmareEchoes.Unit
{
    public class BountyHunterSlash : Skill
    {
        public override bool Cast(Entity target)
        {
            if (!target.IsHostile) return false;
            base.Cast(target);

            StartCoroutine(Attack(target));

            return true;
        }

        IEnumerator Attack(Entity target)
        {
            yield return new WaitForSeconds(0.1f);
            //animation
            animationCoroutine = StartCoroutine(PlaySkillAnimation(thisUnit, "Attacking"));

            yield return new WaitUntil(() => animationCoroutine == null);

            var cacheHealth = target.stats.Health;

            if (DealDamage(target))
            {
                if (cacheHealth != target.stats.Health)
                {
                    target.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.WOUND_DEBUFF, 1, 2));
                }
            }
        }
    }
}
