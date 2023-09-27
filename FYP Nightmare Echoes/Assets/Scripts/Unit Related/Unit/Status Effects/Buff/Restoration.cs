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

        public override void ApplyEffect(Units unit)
        {
            unit.ShowPopUpText("Restoration!", Color.red);
            unit.ShowPopUpText($"+{restorationHeal * restorationStack}", Color.yellow);
            unit.stats.Health += restorationHeal * restorationStack;

        }

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
        {
            return mod;
        }

        public override void IncreaseLifeTime()
        {
            restorationStack++;
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
