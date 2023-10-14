using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "MoveRangeDebuff", menuName = "Unit Modifiers/Debuff/MoveRange Debuff")]
    public class MoveRangeDebuff : Modifier
    {
        [Space(15), Header("Runtime Values")]
        [SerializeField] int moveRangeDebuff;
        [SerializeField] int debuffDuration;

        public override void AwakeStatusEffect()
        {
            moveRangeDebuff = (int)genericValue;
            debuffDuration = modifierDuration;
        }

        public override void ApplyEffect(Entity unit)
        {
            unit.ShowPopUpText("Move Range Decreased!", Color.red);

        }

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
        {
            mod.moveRangeModifier -= moveRangeDebuff;
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
