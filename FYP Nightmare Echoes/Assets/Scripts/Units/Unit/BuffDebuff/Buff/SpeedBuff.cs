using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "Speed Buff", menuName = "Unit Modifiers/Buff/Speed Buff")]
    public class SpeedBuff : BaseModifier
    {
        public override void Awake()
        {

        }

        public override Modifiers ApplyEffect(Modifiers mod)
        {
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
