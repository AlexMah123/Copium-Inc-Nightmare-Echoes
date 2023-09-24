using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "Crippled", menuName = "Unit Modifiers/Debuff/Crippled")]
    public class Crippled : Modifier
    {
        [Space(15), Header("Runtime Values")]
        [SerializeField] int crippledDmg;
        [SerializeField] int crippledStack;

        public override void AwakeStatusEffect()
        {
            crippledDmg = (int)genericValue;
            crippledStack = modifierDuration;
        }

        public override void ApplyEffect(Units unit)
        {
            
        }

        public override void TriggerEffect(Units unit)
        {
            unit.ShowPopUpText("Crippled!");
            unit.ShowDmgText($"-{crippledDmg}");
            unit.stats.Health -= crippledDmg;
        }

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
        {
            return mod;
        }

        public override void IncreaseLifeTime()
        {
            crippledStack++;
        }

        public override void UpdateLifeTime()
        {
            crippledStack--;
        }

        public override float ReturnLifeTime()
        {
            return crippledStack;
        }
    }
}