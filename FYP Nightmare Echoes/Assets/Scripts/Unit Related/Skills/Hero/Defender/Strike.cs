using NightmareEchoes.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class Strike : Skill
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

            StartCoroutine(PlaySkillAnimation(thisUnit, "Attacking"));

            if (DealDamage(target))
            {
                Knockback(thisUnit.ActiveTile, target);
            }
        }
    }
}
