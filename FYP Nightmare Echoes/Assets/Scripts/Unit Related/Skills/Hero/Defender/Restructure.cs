using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class Restructure : Skill
    {
        public override bool Cast()
        {
            StartCoroutine(Attack());

            return true;
        }

        IEnumerator Attack()
        {
            yield return new WaitForSeconds(0.1f);
            //animation
            animationCoroutine = StartCoroutine(PlaySkillAnimation(thisUnit, "Restructure"));

            yield return new WaitUntil(() => animationCoroutine == null);

            thisUnit.stats.Health += heal;
            thisUnit.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.BARRIER_TOKEN, 1, 1));
            thisUnit.ClearAllStatusEffectOfType(ModifierType.NEGATIVETOKEN);
        }
    }
}
