using System.Collections;
using System.Collections.Generic;
using NightmareEchoes.Grid;
using NightmareEchoes.Sound;
using UnityEngine;

//Created by JH
namespace NightmareEchoes.Unit
{
    public class ArcaneMissile : Skill
    {
        public override bool Cast(Entity target)
        {
            base.Cast(target);

            StartCoroutine(Attack(target));

            //AudioManager.instance.PlaySFX("ArcaneMissle");
            return true;
        }

        IEnumerator Attack(Entity target)
        {
            yield return new WaitForSeconds(0.1f);
            //animation
            StartCoroutine(PlaySkillAnimation(thisUnit, "Attacking"));

            DealDamage(target);
        }
    }
}
