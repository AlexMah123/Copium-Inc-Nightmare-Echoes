using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//created by Alex
namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "SpeedDebuff", menuName = "Unit Modifiers/Debuff/Speed debuff")]
    public class SpeedDebuff : Modifier
    {
        [Space(15), Header("Runtime Values")]
        [SerializeField] int speedDebuff;
        [SerializeField] int debuffDuration;

        public override void AwakeStatusEffect()
        {
            speedDebuff = (int)genericValue;
            debuffDuration = modifierDuration;
        }

        public override void ApplyEffect(Entity unit)
        {
            unit.ShowPopUpText("Speed Decreased!", Color.red);
        }

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
        {
            mod.speedModifier -= speedDebuff;
            return mod;
        }

        public override void IncreaseLifeTime(int stack = 0)
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
