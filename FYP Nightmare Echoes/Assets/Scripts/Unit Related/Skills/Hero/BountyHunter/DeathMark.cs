using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class DeathMark : Skill
    {
        public override bool Cast(Entity target)
        {
            if (!target.IsHostile) return false;
            
            base.Cast(target);

            target.RemoveBuff(STATUS_EFFECT.BLOCK_TOKEN);

            StartCoroutine(Attack(target));
            
            return true;
        }

        IEnumerator Attack(Entity target)
        {
            yield return new WaitForSeconds(0.1f);
            //animation
            animationCoroutine = StartCoroutine(PlaySkillAnimation(thisUnit, "DeathMark"));

            yield return new WaitUntil(() => animationCoroutine == null);

            target.AddBuff(GetStatusEffect.CreateModifier(STATUS_EFFECT.BLIND_TOKEN, 1, 1));
            target.AddBuff(GetStatusEffect.CreateModifier(STATUS_EFFECT.VULNERABLE_TOKEN, 2, 1));
            target.AddBuff(GetStatusEffect.CreateModifier(STATUS_EFFECT.DEATHMARK_TOKEN, 1, 2));
        }
    }
}
