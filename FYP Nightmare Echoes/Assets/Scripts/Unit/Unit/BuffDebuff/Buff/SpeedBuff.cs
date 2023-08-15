using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//created by Alex
namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "SpeedBuff", menuName = "Unit Modifiers/Buff/Speed Buff")]
    public class SpeedBuff : Modifier
    {
        [Space(15), Header("Status Effect Values")]
        [SerializeField] int speedBuff;
        [SerializeField] int buffDuration;

        public override void AwakeStatusEffect()
        {
            genericValue = speedBuff;
            modifierDuration = buffDuration;

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
            modifierDuration--;
        }
    }
}
