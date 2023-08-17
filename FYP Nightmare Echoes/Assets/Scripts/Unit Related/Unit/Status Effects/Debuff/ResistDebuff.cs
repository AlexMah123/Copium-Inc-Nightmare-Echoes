using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "ResistDebuff", menuName = "Unit Modifiers/Debuff/Resist Debuff")]
    public class ResistDebuff : Modifier
    {
        [Space(15), Header("Runtime Values")]
        [SerializeField] float resistDebuff;
        [SerializeField] int debuffDuration;

        public override void AwakeStatusEffect()
        {
            resistDebuff = genericValue;
            debuffDuration = modifierDuration;
        }

        public override void ApplyEffect(GameObject unit)
        {

        }

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
        {
            mod.resistModifier -= resistDebuff;
            return mod;
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
