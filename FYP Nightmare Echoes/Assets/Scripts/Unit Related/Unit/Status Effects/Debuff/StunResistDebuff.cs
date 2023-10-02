using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//created by Alex
namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "StunResistDebuff", menuName = "Unit Modifiers/Debuff/StunResist Debuff")]
    public class StunResistDebuff : Modifier
    {
        [Space(15), Header("Runtime Values")]
        [SerializeField] float stunResistDebuff;
        [SerializeField] int debuffDuration;

        public override void AwakeStatusEffect()
        {
            stunResistDebuff = genericValue;
            debuffDuration = modifierDuration;

        }

        public override void ApplyEffect(Entity unit)
        {

        }

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
        {
            mod.stunResistModifier -= stunResistDebuff;
            return mod;
        }

        public override void IncreaseLifeTime()
        {
            debuffDuration++;
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
