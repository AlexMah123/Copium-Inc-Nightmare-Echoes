using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "Restoration", menuName = "Unit Modifiers/Buff/Restoration")]
    public class Restoration : Modifier
    {
        [Space(15), Header("Runtime Values")]
        [SerializeField] int restorationHeal;
        [SerializeField] int restorationStack;

        public override void AwakeStatusEffect()
        {
            restorationHeal = (int)genericValue;
            restorationStack = modifierDuration;
        }

        public override void ApplyEffect(Entity unit)
        {
            unit.ShowPopUpText("Gained Restoration!", Color.green);
        }

        public override void TriggerEffect(Entity unit)
        {
            unit.ShowPopUpText("Restoration Healing!", Color.green);
            unit.ShowPopUpText($"+{restorationHeal * restorationStack}", Color.yellow);
            unit.stats.Health += restorationHeal * restorationStack;
        }

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
        {
            return mod;
        }

        public override void IncreaseLifeTime(int stack = 0)
        {
            restorationStack += stack;
        }

        public override void UpdateLifeTime()
        {
            restorationStack--;
        }

        public override float ReturnLifeTime()
        {
            return restorationStack;
        }
    }
}
