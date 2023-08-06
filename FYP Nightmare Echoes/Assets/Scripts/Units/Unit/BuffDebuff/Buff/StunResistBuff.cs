using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "StunResistBuff", menuName = "Unit Modifiers/Buff/StunResist Buff")]
    public class StunResistBuff : BaseModifier
    {
        [SerializeField] float stunResistBuff;

        public override void Awake()
        {

        }

        public override Modifiers ApplyEffect(Modifiers mod)
        {
            mod.stunResistModifier += stunResistBuff;
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
