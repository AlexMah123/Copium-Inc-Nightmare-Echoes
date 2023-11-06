using System.Collections;
using NightmareEchoes.Grid;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    //Written by Ter (stolen from jh)
    public class Kick : Skill
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
            animationCoroutine = StartCoroutine(PlaySkillAnimation(thisUnit, "Kick"));

            yield return new WaitUntil(() => animationCoroutine == null);

            if (DealDamage(target))
            {
                Knockback(thisUnit.ActiveTile, target);
            }
        }
    }

}
