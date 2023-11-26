using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace NightmareEchoes.Unit
{
    public class RangerArrowShot : Skill
    {
        public override bool Cast(Entity target)
        {
            base.Cast(target);

            StartCoroutine(Attack(target));

            return true;
        }

        IEnumerator Attack(Entity target)
        {
            yield return new WaitForSeconds(0.1f);
            //animation
            StartCoroutine(PlaySkillAnimation(thisUnit, "Attacking"));

            var cacheHealth = target.stats.Health;

            if (DealDamage(target))
            {
                if (cacheHealth != target.stats.Health)
                {
                    target.AddBuff(GetStatusEffect.CreateModifier(STATUS_EFFECT.WOUND_DEBUFF, 1, 2));
                }
            }

        }
    }
}
