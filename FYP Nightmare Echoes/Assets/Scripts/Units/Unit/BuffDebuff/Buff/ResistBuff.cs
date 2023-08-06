using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "ResistBuff", menuName = "Unit Modifiers/Buff/Resist Buff")]
    public class ResistBuff : BaseModifier
    {
        [SerializeField] float resistBuff;

        public override void Awake()
        {
            
        }

        public override Modifiers ApplyEffect(Modifiers mod)
        {
            mod.resistModifier += resistBuff;
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
