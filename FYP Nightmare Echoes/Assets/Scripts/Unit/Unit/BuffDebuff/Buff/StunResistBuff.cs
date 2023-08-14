using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//created by Alex
namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "StunResistBuff", menuName = "Unit Modifiers/Buff/StunResist Buff")]
    public class StunResistBuff : Modifier
    {
        [SerializeField] float stunResistBuff;

        public override void Awake()
        {
            genericValue = stunResistBuff;

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

        }

        public override void Remove()
        {

        }
    }
}
