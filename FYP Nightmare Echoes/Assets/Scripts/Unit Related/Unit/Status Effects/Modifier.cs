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
        [Tooltip("Value for status effect, tokens are defaulted to 1")] public float genericValue;
        [Tooltip("duration/stacks for status effect")] public int modifierDuration;
        [Tooltip("the limit on how many this effect can exist")] public int limitStack = 1;

        public abstract void AwakeStatusEffect();
        public abstract void ApplyEffect(Entity unit);

        public virtual void TriggerEffect(Entity unit)
        {

        }

        public abstract ModifiersStruct ApplyModifier(ModifiersStruct mod);

        public virtual void IncreaseLifeTime(int stacks = 0)
        {

        }


        public virtual void IncreaseLifeTime(Entity unit)
        {

        }

        public virtual void UpdateLifeTime()
        {

        }

        public virtual void UpdateLifeTime(Entity unit)
        {

        }

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


