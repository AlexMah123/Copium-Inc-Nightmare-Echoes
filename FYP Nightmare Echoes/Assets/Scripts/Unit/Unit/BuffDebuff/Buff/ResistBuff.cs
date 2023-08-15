using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "ResistBuff", menuName = "Unit Modifiers/Buff/Resist Buff")]
    public class ResistBuff : Modifier
    {
        [Space(15), Header("Status Effect Values")]
        [SerializeField] float resistBuff;
        [SerializeField] int buffDuration;

        public override void AwakeStatusEffect()
        {
            genericValue = resistBuff;
            modifierDuration = buffDuration;
        }

        public override void ApplyEffect(GameObject unit)
        {

        }

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
        {
            mod.resistModifier += resistBuff;
            return mod;
        }

        public override void UpdateLifeTime()
        {
            modifierDuration--;
        }

    }
}
