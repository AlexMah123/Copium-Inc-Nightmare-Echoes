using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//created by Alex
namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "StunResistBuff", menuName = "Unit Modifiers/Buff/StunResist Buff")]
    public class StunResistBuff : Modifier
    {
        [Space(15), Header("Status Effect Values")]
        [SerializeField] float stunResistBuff;
        [SerializeField] int buffDuration;

        public override void AwakeStatusEffect()
        {
            genericValue = stunResistBuff;
            modifierDuration = buffDuration;

        }

        public override void ApplyEffect(GameObject unit)
        {

        }

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
        {
            mod.stunResistModifier += stunResistBuff;
            return mod;
        }

        public override void UpdateLifeTime()
        {
            modifierDuration--;
        }
    }
}
