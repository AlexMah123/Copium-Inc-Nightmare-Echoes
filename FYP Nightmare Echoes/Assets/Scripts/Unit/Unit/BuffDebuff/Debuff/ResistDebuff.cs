using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "ResistDebuff", menuName = "Unit Modifiers/Debuff/Resist Debuff")]
    public class ResistDebuff : Modifier
    {
        [SerializeField] float resistDebuff;

        public override void Awake()
        {
            genericValue = resistDebuff;
        }

        public override void ApplyEffect(GameObject unit)
        {

        }

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
        {
            mod.resistModifier -= resistDebuff;
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
