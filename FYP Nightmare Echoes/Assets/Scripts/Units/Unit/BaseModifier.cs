using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    [Serializable]
    public abstract class BaseModifier : ScriptableObject
    {
        [Header("Modifier Details")]
        public Sprite icon;
        public new string name;
        public ModifierType modifierType;


        public abstract void Awake();
        public abstract Modifiers ApplyEffect(Modifiers mod);
        public abstract void UpdateLifeTime();
        public abstract void Remove();
    }

    public enum ModifierType
    {
        BUFF = 0,
        DEBUFF = 1,
        TOKEN = 2,
    }

    [System.Serializable]
    public struct Modifiers
    {
        public int healthModifier;
        public int speedModifier;
        public int moveRangeModifier;
        public float stunResistModifier;
        public float resistModifier;

        public Modifiers(int healthMod, int speedMod, int moveRangeMod, float stunResistMod, float resistMod)
        {
            healthModifier = healthMod;
            speedModifier = speedMod;
            moveRangeModifier = moveRangeMod;
            stunResistModifier = stunResistMod;
            resistModifier = resistMod;
        }
    }
}


