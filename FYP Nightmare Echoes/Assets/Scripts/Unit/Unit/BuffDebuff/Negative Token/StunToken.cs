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
        [SerializeField] int debuffDuration;

        public override void AwakeStatusEffect()
        {
            debuffDuration = modifierDuration;

        }

        public override void ApplyEffect(GameObject unit)
        {
            
        }

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
        {
            return mod;
        }

        public override void UpdateLifeTime()
        {
            debuffDuration--;
        }

        public override float ReturnLifeTime()
        {
            return debuffDuration;
        }

        
    }
}
