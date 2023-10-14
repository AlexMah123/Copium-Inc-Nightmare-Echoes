using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "DeathMarkToken", menuName = "Unit Modifiers/Unique Tokens/Death Mark Token")]
    public class DeathMarkToken : Modifier
    {
        [Space(15), Header("Runtime Values")]
        [SerializeField] int tokenStack;

        public override void AwakeStatusEffect()
        {
            tokenStack = modifierDuration;
        }

        #region Effects Related
        public override void ApplyEffect(Entity unit)
        {
            unit.DeathMarkToken = true;
            unit.ShowPopUpText("Marked For Death!", Color.magenta);
        }

        public override void TriggerEffect(Entity unit)
        {

        }
        #endregion

        public override ModifiersStruct ApplyModifier(ModifiersStruct mod)
        {
            return mod;
        }

        #region LifeTime Related
        public override void IncreaseLifeTime(Entity unit)
        {
            if (tokenStack < limitStack)
            {
                tokenStack++;
            }
            else
            {
                unit.ShowPopUpText("Death Mark reached max limit!", Color.blue);
            }
        }

        public override void UpdateLifeTime(Entity unit)
        {
            tokenStack--;

            if (tokenStack == 0)
            {
                unit.DeathMarkToken = false;
            }
        }

        public override float ReturnLifeTime()
        {
            return tokenStack;
        }
        #endregion
    }
}
