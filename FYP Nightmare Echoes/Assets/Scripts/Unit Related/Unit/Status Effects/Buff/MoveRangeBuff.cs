using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "MoveRangeBuff", menuName = "Unit Modifiers/Buff/MoveRange Buff")]
    public class MoveRangeBuff : Modifier
    {
        [Space(15), Header("Runtime Values")]
        [SerializeField] int moveRangeBuff;
        [SerializeField] int buffDuration;

        public override void AwakeStatusEffect()
        {
            moveRangeBuff = (int)genericValue;
            buffDuration = modifierDuration;

        }

        public override void ApplyEffect(Entity unit)
        {
            unit.ShowPopUpText("Move Range Increased!", Color.green);

        }

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
        {
            mod.moveRangeModifier += moveRangeBuff;
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
