using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "ResistBuff", menuName = "Unit Modifiers/Buff/Resist Buff")]
    public class ResistBuff : Modifier
    {
        [Space(15), Header("Runtime Effect Values")]
        [SerializeField] float resistBuff;
        [SerializeField] int buffDuration;

        public override void AwakeStatusEffect()
        {
            resistBuff = genericValue;
            buffDuration = modifierDuration;
        }

        public override void ApplyEffect(Entity unit)
        {
            unit.ShowPopUpText("Resist Increased!", Color.green);

        }

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
        {
            mod.resistModifier += resistBuff;
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
