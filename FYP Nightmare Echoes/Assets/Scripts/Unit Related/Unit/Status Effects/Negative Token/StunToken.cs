using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "StunToken", menuName = "Unit Modifiers/NegativeToken/Stun Token")]
    public class StunToken : Modifier
    {
        [Space(15), Header("Runtime Values")]
        [SerializeField] int tokenStack;

        public override void AwakeStatusEffect()
        {
            tokenStack = modifierDuration;
        }


        #region Effect Related
        public override void ApplyEffect(Units unit)
        {
            if(!unit.StunToken)
            {
                unit.ShowPopUpText("Stunned!");
                unit.StunToken = true;
            }
            
        }

        public override void TriggerEffect(Units unit)
        {
            
        }

        #endregion

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
        {
            return mod;
        }

        #region LifeTime Related
        public override void IncreaseLifeTime()
        {
            tokenStack++;
        }

        public override void UpdateLifeTime(Units unit)
        {
            tokenStack--;

            if (tokenStack == 0)
            {
                unit.StunToken = false;
            }
        }

        public override float ReturnLifeTime()
        {
            return tokenStack;
        }
        #endregion
    }
}
