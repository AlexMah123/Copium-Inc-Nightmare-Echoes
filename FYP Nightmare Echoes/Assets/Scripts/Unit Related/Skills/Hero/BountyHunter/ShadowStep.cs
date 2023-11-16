using System.Collections;
using System.Collections.Generic;
using NightmareEchoes.Grid;
using NightmareEchoes.Unit.Combat;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class ShadowStep : Skill
    {
        public override bool Cast()
        {
            base.Cast();

            if (thisUnit.DoesModifierExist(STATUS_EFFECT.STEALTH_TOKEN))
            {
                thisUnit.ShowPopUpText("Already In Stealth!!", Color.red);
                CombatManager.Instance.SelectSkill(thisUnit, this);
                return false;
            }

            //Get enemies in nearby proximity
            var grid = CombatManager.Instance.SquareRange(thisUnit.ActiveTile, 1);
            var cleanedGrid = OverlayTileManager.Instance.TrimOutOfBounds(grid);
            var enemiesInRange = new List<Entity>();
            foreach (var tile in cleanedGrid)
            {
                if (!tile.CheckEntityGameObjectOnTile()) 
                    continue;

                var target = tile.CheckEntityGameObjectOnTile().GetComponent<Entity>();

                if (!target.IsHostile || target.IsProp) 
                    continue;
                enemiesInRange.Add(target);
            }
            
            //Check if this unit is in range of said enemies
            foreach (var enemy in enemiesInRange)
            {
                grid = CombatManager.Instance.FrontalRange(enemy.ActiveTile, 1, enemy);
                cleanedGrid = OverlayTileManager.Instance.TrimOutOfBounds(grid);

                foreach (var tile in cleanedGrid)
                {
                    if (thisUnit.ActiveTile == tile)
                    {
                        thisUnit.ShowPopUpText("Cannot Go Into Stealth, Too Close to Enemy!!", Color.red);
                        CombatManager.Instance.SelectSkill(thisUnit, this);
                        return false;
                    }
                }
            }

            StartCoroutine(Attack());
            
            return true;
        }

        IEnumerator Attack()
        {
            yield return new WaitForSeconds(0.1f);
            //animation
            animationCoroutine = StartCoroutine(PlaySkillAnimation(thisUnit, "ShadowStep"));

            yield return new WaitUntil(() => animationCoroutine == null);

            thisUnit.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.STRENGTH_TOKEN, 1, 1));
            thisUnit.AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.STEALTH_TOKEN, 1, 1));
        }
    }
}
