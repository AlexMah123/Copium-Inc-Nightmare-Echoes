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
        public override void ApplyEffect(Entity unit)
        {

            if (!unit.BlockToken)
            {
                unit.VulnerableToken = true;
                unit.ShowPopUpText("Vulnerable!", Color.yellow);
            }
            else if (unit.BlockToken)
            {
                unit.ShowPopUpText("Negated Block!", Color.blue);
                unit.UpdateTokenLifeTime(STATUS_EFFECT.BLOCK_TOKEN);
                unit.UpdateTokenLifeTime(STATUS_EFFECT.VULNERABLE_TOKEN);
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
                unit.ShowPopUpText("Vulnerable token reached max limit!", Color.blue);
            }
        }

        public override void UpdateLifeTime(Entity unit)
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
