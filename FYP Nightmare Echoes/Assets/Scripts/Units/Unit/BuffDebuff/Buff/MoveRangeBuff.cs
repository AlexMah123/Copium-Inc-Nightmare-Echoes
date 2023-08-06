using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "MoveRangeBuff", menuName = "Unit Modifiers/Buff/MoveRange Buff")]
    public class MoveRangeBuff : BaseModifier
    {
        [SerializeField] int moveRangeBuff;

        public override void Awake()
        {

        }

        public override Modifiers ApplyEffect(Modifiers mod)
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
