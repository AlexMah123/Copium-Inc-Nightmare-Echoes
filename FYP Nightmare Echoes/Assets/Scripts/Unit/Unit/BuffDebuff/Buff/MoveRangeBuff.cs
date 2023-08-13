using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "MoveRangeBuff", menuName = "Unit Modifiers/Buff/MoveRange Buff")]
    public class MoveRangeBuff : Modifier
    {
        [SerializeField] int moveRangeBuff;

        public override void Awake()
        {

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

        }

        public override void Remove()
        {

        }
    }
}
