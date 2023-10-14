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

        public override void ApplyEffect(Entity unit)
        {
            unit.ShowPopUpText("Crippled!", Color.red);
        }

        public override void TriggerEffect(Entity unit)
        {
            unit.ShowPopUpText("Crippled Dmg!", Color.red);
            unit.ShowPopUpText($"-{crippledDmg}", Color.yellow);
            unit.stats.Health -= crippledDmg;
        }

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
        {
            return mod;
        }

        public override void IncreaseLifeTime(int stack = 0)
        {
            crippledStack += stack;
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
