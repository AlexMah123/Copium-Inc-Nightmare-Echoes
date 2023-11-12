using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    //Written by Ter (stolen from jh)
    public class DarkVolt : Skill
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
            animationCoroutine = StartCoroutine(PlaySkillAnimation(thisUnit, "Attacking"));

            yield return new WaitUntil(() => animationCoroutine == null);

            if (DealDamage(target))
            {
                if ((debuffChance - target.stats.Resist) > Random.Range(0, 101))
                {
                    target.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.WEAKEN_TOKEN, 1, 1));
                }
            }
        }
    }
}
