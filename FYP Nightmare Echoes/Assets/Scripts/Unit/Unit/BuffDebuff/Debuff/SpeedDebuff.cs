using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//created by Alex
namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "SpeedDebuff", menuName = "Unit Modifiers/Debuff/Speed debuff")]
    public class SpeedDebuff : Modifier
    {
        [Space(15), Header("Status Effect Values")]
        [SerializeField] int speedDebuff;
        [SerializeField] int debuffDuration;

        public override void AwakeStatusEffect()
        {
            genericValue = speedDebuff;
            modifierDuration = debuffDuration;

        }

        public override void ApplyEffect(GameObject unit)
        {

        }

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
        {
            mod.speedModifier -= speedDebuff;
            return mod;
        }

        public override void UpdateLifeTime()
        {
            modifierDuration--;

        }


    }
}
