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

        public override void ApplyEffect(Units unit)
        {
            unit.ShowPopUpText("Vertigo!");
            unit.VertigoToken = true;
        }

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
        {
            mod.speedModifier += (int)genericValue;

            return mod;
        }

        public override void IncreaseLifeTime()
        {
            tokenStack++;
        }

        public override void UpdateLifeTime()
        {
            tokenStack--;
        }

        public override float ReturnLifeTime()
        {
            return tokenStack;
        }
    }
}
