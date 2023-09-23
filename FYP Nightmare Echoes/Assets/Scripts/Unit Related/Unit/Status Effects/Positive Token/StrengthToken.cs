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
        public override void ApplyEffect(Units unit)
        {
            unit.StrengthToken = true;

            if (!unit.WeakenToken)
            {
                unit.ShowPopUpText("Strength!");
            }
            else if (unit.WeakenToken)
            {
                unit.ShowPopUpText("Negated Weaken!");
                unit.UpdateTokenLifeTime(STATUS_EFFECT.WEAKEN_TOKEN);
                unit.UpdateTokenLifeTime(STATUS_EFFECT.STRENGTH_TOKEN);
            }
        }

        public override void TriggerEffect(Units unit)
        {

        }
        #endregion

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
        {
            return mod;
        }

        #region LifeTime Related
        public override void IncreaseLifeTime()
        {
            tokenStack++;
        }

        public override void UpdateLifeTime(Units unit)
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
