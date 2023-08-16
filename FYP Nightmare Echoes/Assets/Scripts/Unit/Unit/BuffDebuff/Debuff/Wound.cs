using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "Wound", menuName = "Unit Modifiers/Debuff/Wound")]
    public class Wound : Modifier
    {
        [Space(15), Header("Runtime Values")]
        [SerializeField] int woundDmg;
        [SerializeField] int woundStack;

        public override void AwakeStatusEffect()
        {
            woundDmg = (int)genericValue;
            woundStack = modifierDuration;
        }

        public override void ApplyEffect(GameObject unit)
        {
            unit.GetComponent<Units>().stats.Health -= woundDmg * woundStack;

        }

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
        {
            return mod;
        }

        public override void UpdateLifeTime()
        {
            woundStack--;
        }

        public override float ReturnLifeTime()
        {
            return woundStack;
        }

    }
}
