using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "MoveRangeDebuff", menuName = "Unit Modifiers/Debuff/MoveRange Debuff")]
    public class MoveRangeDebuff : Modifier
    {
        [Space(15), Header("Status Effect Values")]
        [SerializeField] int moveRangeDebuff;
        [SerializeField] int debuffDuration;

        public override void AwakeStatusEffect()
        {
            genericValue = moveRangeDebuff;
            modifierDuration = debuffDuration;

        }

        public override void ApplyEffect(GameObject unit)
        {

        }

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
        {
            mod.moveRangeModifier -= moveRangeDebuff;
            return mod;
        }

        public override void UpdateLifeTime()
        {
            modifierDuration--;

        }

    }
}
