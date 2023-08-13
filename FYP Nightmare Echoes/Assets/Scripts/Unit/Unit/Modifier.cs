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
        public string description;
        public ModifierType modifierType;


        public abstract void Awake();

        public abstract void ApplyEffect(GameObject unit);
        public abstract ModifiersStruct ApplyModifier(ModifiersStruct mod);
        public abstract void UpdateLifeTime();
        public abstract void Remove();
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


