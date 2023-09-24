using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "WeakenToken", menuName = "Unit Modifiers/NegativeToken/Weaken Token")]
    public class WeakenToken : Modifier
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
            unit.WeakenToken = true;

            if (!unit.StrengthToken)
            {
                unit.ShowPopUpText("Weaken!");
            }
            else if (unit.StrengthToken)
            {
                unit.ShowPopUpText("Negated Strength!");
                unit.UpdateTokenLifeTime(STATUS_EFFECT.STRENGTH_TOKEN);
                unit.UpdateTokenLifeTime(STATUS_EFFECT.WEAKEN_TOKEN);
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
                unit.WeakenToken = false;
            }
        }

        public override float ReturnLifeTime()
        {
            return tokenStack;
        }
        #endregion
    }
}