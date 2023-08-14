using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//created by Alex
namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "StunResistDebuff", menuName = "Unit Modifiers/Debuff/StunResist Debuff")]
    public class StunResistDebuff : Modifier
    {
        [SerializeField] float stunResistDebuff;

        public override void Awake()
        {
            genericValue = stunResistDebuff;
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

        }

        public override void Remove()
        {

        }
    }
}
