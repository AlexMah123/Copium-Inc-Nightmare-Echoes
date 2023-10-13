using NightmareEchoes.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class RuneTrap : Skill
    {
        private bool trapsSet = false;

        public override bool Cast(Entity unit)
        {
            base.Cast(unit);

            unit.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.IMMOBILIZE_TOKEN, 1, 2));
            unit.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.WOUND_DEBUFF, 1, 2));
            unit.TakeDamage(damage, ignoreTokens: true);

            return true;
        }

        private bool CheckTraps()
        {
            return trapsSet;
        }
    }
}
