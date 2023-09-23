using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "StunToken", menuName = "Unit Modifiers/NegativeToken/Stun Token")]
    public class StunToken : Modifier
    {
        [Space(15), Header("Runtime Values")]
        [SerializeField] int tokenStack;

        public override void AwakeStatusEffect()
        {
            tokenStack = modifierDuration;
        }

        public override void ApplyEffect(Units unit)
        {
            unit.ShowPopUpText("Stunned!");
            unit.StunToken = true;
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
