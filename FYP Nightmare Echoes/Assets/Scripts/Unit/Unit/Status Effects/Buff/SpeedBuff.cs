using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//created by Alex
namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "SpeedBuff", menuName = "Unit Modifiers/Buff/Speed Buff")]
    public class SpeedBuff : Modifier
    {
        [Space(15), Header("Runtime Values")]
        [SerializeField] int speedBuff;
        [SerializeField] int buffDuration;

        public override void AwakeStatusEffect()
        {
            speedBuff = (int)genericValue;
            buffDuration = modifierDuration;
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
            buffDuration--;
        }
        public override float ReturnLifeTime()
        {
            return buffDuration;
        }
    }
}
