using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "HasteToken", menuName = "Unit Modifiers/PositiveToken/Haste Token")]
    public class HasteToken : Modifier
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
            unit.HasteToken = true;

            if (!unit.VertigoToken)
            {
                unit.ShowPopUpText("Haste!");
            }
            else if (unit.VertigoToken)
            {
                unit.ShowPopUpText("Negated Vertigo!");
                unit.UpdateTokenLifeTime(STATUS_EFFECT.VERTIGO_TOKEN);
                unit.UpdateTokenLifeTime(STATUS_EFFECT.HASTE_TOKEN);
            }
        }

        public override void TriggerEffect(Units unit)
        {
            
        }
        #endregion

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
        {
            mod.speedModifier += (int)genericValue;

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
                unit.HasteToken = false;
            }
        }

        public override float ReturnLifeTime()
        {
            return tokenStack;
        }
        #endregion

    }
}
