using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "MoveRangeBuff", menuName = "Unit Modifiers/Buff/MoveRange Buff")]
    public class MoveRangeBuff : Modifier
    {
        [Space(15), Header("Status Effect Values")]
        [SerializeField] int moveRangeBuff;
        [SerializeField] int buffDuration;

        public override void AwakeStatusEffect()
        {
            genericValue = moveRangeBuff;
            modifierDuration = buffDuration;

        }

        public override void ApplyEffect(GameObject unit)
        {

        }

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
        {
            mod.moveRangeModifier += moveRangeBuff;
            return mod;
        }

        public override void UpdateLifeTime()
        {
            modifierDuration--;
        }

    }
}
