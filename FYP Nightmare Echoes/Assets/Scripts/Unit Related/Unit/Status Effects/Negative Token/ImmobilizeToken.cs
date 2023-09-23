using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "ImmobilizeToken", menuName = "Unit Modifiers/NegativeToken/Immobilize Token")]
    public class ImmobilizeToken : Modifier
    {
        [Space(15), Header("Runtime Values")]
        [SerializeField] int tokenStack;

        public override void AwakeStatusEffect()
        {
            tokenStack = modifierDuration;
        }

        public override void ApplyEffect(Units unit)
        {
            unit.ShowPopUpText("Immobilized!");
            unit.ImmobilizeToken = true;
        }

        public override void TriggerEffect(Units unit)
        {
            unit.ShowPopUpText("Cannot Move");
        }

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
        {
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
