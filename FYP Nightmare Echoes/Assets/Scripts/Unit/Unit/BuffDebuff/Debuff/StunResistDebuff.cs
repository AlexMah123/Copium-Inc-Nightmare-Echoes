using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//created by Alex
namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "StunResistDebuff", menuName = "Unit Modifiers/Debuff/StunResist Debuff")]
    public class StunResistDebuff : Modifier
    {
        [Space(15), Header("Status Effect Values")]
        [SerializeField] float stunResistDebuff;
        [SerializeField] int debuffDuration;

        public override void AwakeStatusEffect()
        {
            genericValue = stunResistDebuff;
            modifierDuration = debuffDuration;

        }

        public override void ApplyEffect(GameObject unit)
        {

        }

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
        {
            mod.stunResistModifier -= stunResistDebuff;
            return mod;
        }

        public override void UpdateLifeTime()
        {
            modifierDuration--;

        }

    }
}
