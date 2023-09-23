using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "VulnerableToken", menuName = "Unit Modifiers/NegativeToken/Vulnerable Token")]
    public class VulnerableToken : Modifier
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
            unit.VulnerableToken = true;

            if (!unit.BlockToken)
            {
                unit.ShowPopUpText("Vulnerable!");
            }
            else if (unit.BlockToken)
            {
                unit.ShowPopUpText("Negated Block!");
                unit.UpdateTokenLifeTime(STATUS_EFFECT.BLOCK_TOKEN);
                unit.UpdateTokenLifeTime(STATUS_EFFECT.VULNERABLE_TOKEN);
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
                unit.VulnerableToken = false;
            }
        }

        public override float ReturnLifeTime()
        {
            return tokenStack;
        }
        #endregion
    }
}
