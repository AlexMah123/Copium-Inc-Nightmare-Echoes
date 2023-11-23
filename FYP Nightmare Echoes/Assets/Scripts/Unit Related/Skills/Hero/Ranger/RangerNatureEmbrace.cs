using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class RangerNatureEmbrace : Skill
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
            yield return StartCoroutine(PlaySkillAnimation(thisUnit, "Heal"));

            target.stats.Health += heal;
            target.ShowPopUpText("Healed!", Color.green);
            target.ShowPopUpText($"{heal}", Color.green);


        }
    }
}
