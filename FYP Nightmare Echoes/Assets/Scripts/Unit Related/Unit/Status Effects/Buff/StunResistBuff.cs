using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//created by Alex
namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "StunResistBuff", menuName = "Unit Modifiers/Buff/StunResist Buff")]
    public class StunResistBuff : Modifier
    {
        [Space(15), Header("Runtime Values")]
        [SerializeField] float stunResistBuff;
        [SerializeField] int buffDuration;

        public override void AwakeStatusEffect()
        {
            stunResistBuff = genericValue;
            buffDuration = modifierDuration;
        }

        public override void ApplyEffect(Entity unit)
        {
            unit.ShowPopUpText("Stun Resist Increased!", Color.green);

        }

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
        {
            mod.stunResistModifier += stunResistBuff;
            return mod;
        }

        public override void IncreaseLifeTime(int stack = 0)
        {
            buffDuration++;
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
