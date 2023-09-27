using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "ImmobilizeToken", menuName = "Unit Modifiers/NegativeToken/Immobilize Token")]
    public class ImmobilizeToken : Modifier
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
            unit.ShowPopUpText("Immobilized!", Color.red);
            unit.ImmobilizeToken = true;
        }

        public override void TriggerEffect(Units unit)
        {
            unit.ShowPopUpText("Cannot Move", Color.red);
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
                unit.ImmobilizeToken = false;
            }
        }

        public override float ReturnLifeTime()
        {
            return tokenStack;
        }
        #endregion
    }
}
