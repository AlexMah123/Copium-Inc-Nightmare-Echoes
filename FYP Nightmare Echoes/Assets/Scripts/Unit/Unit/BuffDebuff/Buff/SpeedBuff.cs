using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//created by Alex
namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "SpeedBuff", menuName = "Unit Modifiers/Buff/Speed Buff")]
    public class SpeedBuff : Modifer
    {
        [SerializeField] int speedBuff;

        public override void Awake()
        {

        }

        public override Modifiers ApplyEffect(Modifiers mod)
        {
            mod.speedModifier += speedBuff;
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
