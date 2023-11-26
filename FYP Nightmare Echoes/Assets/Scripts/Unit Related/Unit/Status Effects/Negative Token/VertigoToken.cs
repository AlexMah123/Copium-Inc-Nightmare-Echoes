using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "VertigoToken", menuName = "Unit Modifiers/NegativeToken/Vertigo Token")]
    public class VertigoToken : Modifier
    {
        [Space(15), Header("Runtime Values")]
        [SerializeField] int tokenStack;

        public override void AwakeStatusEffect()
        {
            tokenStack = modifierDuration;
        }

        #region Effect Related
        public override void ApplyEffect(Entity unit)
        {

            if (!unit.HasteToken)
            {
                unit.VertigoToken = true;
                unit.ShowPopUpText("Vertigo!", Color.yellow);
            }
            else if (unit.HasteToken)
            {
                unit.ShowPopUpText("Negated Haste!", Color.magenta);
                unit.UpdateTokenLifeTime(STATUS_EFFECT.HASTE_TOKEN);
                unit.UpdateTokenLifeTime(STATUS_EFFECT.VERTIGO_TOKEN);
            }
        }

        public override void TriggerEffect(Entity unit)
        {
            
        }
        #endregion

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
        {
            mod.speedModifier -= (int)genericValue;

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
                unit.ShowPopUpText("Vertigo token reached max limit!", Color.magenta);
            }
        }

        public override void UpdateLifeTime(Entity unit)
        {
            tokenStack--;

            if (tokenStack == 0)
            {
                unit.VertigoToken = false;
            }
        }

        public override float ReturnLifeTime()
        {
            return tokenStack;
        }
        #endregion
    }
}
