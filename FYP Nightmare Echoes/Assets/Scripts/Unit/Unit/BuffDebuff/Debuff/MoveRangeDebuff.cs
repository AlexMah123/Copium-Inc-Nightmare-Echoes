using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "MoveRangeDebuff", menuName = "Unit Modifiers/Debuff/MoveRange Debuff")]
    public class MoveRangeDebuff : Modifier
    {
        [SerializeField] int moveRangeDebuff;

        public override void Awake()
        {

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

        }

        public override void Remove()
        {

        }
    }
}
