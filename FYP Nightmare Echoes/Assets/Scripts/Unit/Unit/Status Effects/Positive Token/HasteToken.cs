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
        [SerializeField] int buffDuration;

        public override void AwakeStatusEffect()
        {
            buffDuration = modifierDuration;

        }

        public override void ApplyEffect(GameObject unit)
        {
            unit.GetComponent<Units>().StunToken = true;
        }

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
        {
            return mod;
        }

        public override void UpdateLifeTime()
        {
            buffDuration--;
        }

        public override float ReturnLifeTime()
        {
            return buffDuration;
        }
    }
}
