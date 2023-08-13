using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "ResistBuff", menuName = "Unit Modifiers/Buff/Resist Buff")]
    public class ResistBuff : Modifier
    {
        [SerializeField] float resistBuff;

        public override void Awake()
        {
            
        }

        public override void ApplyEffect(GameObject unit)
        {

        }

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
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
