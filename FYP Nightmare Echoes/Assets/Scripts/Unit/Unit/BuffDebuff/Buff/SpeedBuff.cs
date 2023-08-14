using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//created by Alex
namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "SpeedBuff", menuName = "Unit Modifiers/Buff/Speed Buff")]
    public class SpeedBuff : Modifier
    {
        [SerializeField] int speedBuff;

        public override void Awake()
        {
            genericValue = speedBuff;

        }

        public override void ApplyEffect(GameObject unit)
        {

        }

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
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
