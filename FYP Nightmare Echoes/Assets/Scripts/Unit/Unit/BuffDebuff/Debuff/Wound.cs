using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "Wound", menuName = "Unit Modifiers/Debuff/Wound")]
    public class Wound : Modifier
    {
        [SerializeField] int woundDmg;
        [SerializeField] int woundStack = 1;

        public override void Awake()
        {
            genericValue = woundStack;
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

        }

        public override void Remove()
        {

        }

        
    }
}
