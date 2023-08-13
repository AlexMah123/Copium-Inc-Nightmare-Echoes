using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//created by Alex
namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "SpeedDebuff", menuName = "Unit Modifiers/Debuff/Speed debuff")]
    public class SpeedDebuff : Modifier
    {
        [SerializeField] int speedDebuff;

        public override void Awake()
        {

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

        }

        public override void Remove()
        {

        }
    }
}
