using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class SuppressingShot : Skill
    {
        [SerializeField] float stunChance = 80;
        public override bool Cast(Units target)
        {
            base.Cast(target);

            if (isBackstabbing)
            {
                target.TakeDamage(damage + backstabBonus);
            }
            else
            {
                target.TakeDamage(damage);
            }
            isBackstabbing = false;

            if ((stunChance - target.stats.StunResist) > (Random.Range(0, 101)))
            {
                target.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.STUN_TOKEN, 1, 1));
            }

            return true;
        }
    }
}
