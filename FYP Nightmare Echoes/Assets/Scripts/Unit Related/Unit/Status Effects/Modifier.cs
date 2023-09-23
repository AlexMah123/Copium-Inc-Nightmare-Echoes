using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.Unit
{
    [Serializable]
    public abstract class Modifier : ScriptableObject
    {
        [Header("Modifier Details")]
        public Sprite icon;
        public new string name;
        [TextArea (1,5)]public string description;

        [Space(15), Header("Status Effect Values")]
        public STATUS_EFFECT statusEffect;
        public ModifierType modifierType;
        [SerializeField] public float genericValue;
        [SerializeField] public int modifierDuration;

        public abstract void AwakeStatusEffect();
        public abstract void ApplyEffect(Units unit);

        public virtual void TriggerEffect(Units unit)
        {

        }

        public abstract ModifiersStruct ApplyModifier(ModifiersStruct mod);
        public abstract void IncreaseLifeTime();
        public abstract void UpdateLifeTime();
        public abstract float ReturnLifeTime();
    }

    public enum ModifierType
    {
        BUFF = 0,
        DEBUFF = 1,
        POSITIVETOKEN = 2,
        NEGATIVETOKEN = 3,
    }

    [System.Serializable]
    public struct ModifiersStruct
    {
        public int healthModifier;
        public int speedModifier;
        public int moveRangeModifier;
        public float stunResistModifier;
        public float resistModifier;

        public ModifiersStruct(int healthMod, int speedMod, int moveRangeMod, float stunResistMod, float resistMod)
        {
            healthModifier = healthMod;
            speedModifier = speedMod;
            moveRangeModifier = moveRangeMod;
            stunResistModifier = stunResistMod;
            resistModifier = resistMod;
        }
    }
}


