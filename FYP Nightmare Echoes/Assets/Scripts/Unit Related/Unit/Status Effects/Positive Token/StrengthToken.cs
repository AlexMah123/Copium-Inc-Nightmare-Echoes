using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "StrengthToken", menuName = "Unit Modifiers/PositiveToken/Strength Token")]
    public class StrengthToken : Modifier
    {
        [Space(15), Header("Runtime Values")]
        [SerializeField] int tokenStack;

        public override void AwakeStatusEffect()
        {
            tokenStack = modifierDuration;
        }

        #region Effects Related
        public override void ApplyEffect(Entity unit)
        {

            if (!unit.WeakenToken)
            {
                unit.StrengthToken = true;
                unit.ShowPopUpText("Gained Strength!", Color.magenta);
            }
            else if (unit.WeakenToken)
            {
                unit.ShowPopUpText("Negated Weaken!", Color.magenta);
                unit.UpdateTokenLifeTime(STATUS_EFFECT.WEAKEN_TOKEN);
                unit.UpdateTokenLifeTime(STATUS_EFFECT.STRENGTH_TOKEN);
            }
        }

        public override void TriggerEffect(Entity unit)
        {

        }
        #endregion

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
        {
            return mod;
        }

        #region LifeTime Related
        public override void IncreaseLifeTime(Entity unit)
        {
            if (tokenStack < limitStack)
            {
                tokenStack++;
            }
            else
            {
                unit.ShowPopUpText("Strength reached max limit!", Color.magenta);
            }
        }

        public override void UpdateLifeTime(Entity unit)
        {
            tokenStack--;

            if (tokenStack == 0)
            {
                unit.StrengthToken = false;
            }
        }

        public override float ReturnLifeTime()
        {
            return tokenStack;
        }
        #endregion
    }
}
