using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "BlockToken", menuName = "Unit Modifiers/PositiveToken/Block Token")]
    public class BlockToken : Modifier
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
            unit.BlockToken = true;

            if (!unit.VulnerableToken && !unit.BlockToken)
            {
                unit.ShowPopUpText("Gained Block!", Color.red);
            }
            else if (unit.VulnerableToken)
            {
                unit.ShowPopUpText("Negated Vulnerable!", Color.red);
                unit.UpdateTokenLifeTime(STATUS_EFFECT.VULNERABLE_TOKEN);
                unit.UpdateTokenLifeTime(STATUS_EFFECT.BLOCK_TOKEN);
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
        public override void IncreaseLifeTime()
        {
            tokenStack++;
        }

        public override void UpdateLifeTime(Entity unit)
        {
            tokenStack--;

            if (tokenStack == 0)
            {
                unit.BlockToken = false;
            }
        }

        public override float ReturnLifeTime()
        {
            return tokenStack;
        }
        #endregion
    }
}
