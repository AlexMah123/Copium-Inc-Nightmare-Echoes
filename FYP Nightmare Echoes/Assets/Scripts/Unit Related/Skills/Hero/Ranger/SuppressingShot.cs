using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class SuppressingShot : Skill
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

            if (DealDamage(target))
            {
                if ((stunChance - target.stats.StunResist) > (Random.Range(0, 101)))
                {
                    target.AddBuff(GetStatusEffect.CreateModifier(STATUS_EFFECT.STUN_TOKEN, 1, 1));
                }
            }
        }
    }
}
