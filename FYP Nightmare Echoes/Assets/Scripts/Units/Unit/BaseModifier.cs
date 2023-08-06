using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public abstract class BaseModifier : ScriptableObject
    {
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
        public int stunResistModifier;
        public int resistModifier;

        public Modifiers(int healthMod, int speedMod, int moveRangeMod, int stunResistMod, int resistMod)
        {
            healthModifier = healthMod;
            speedModifier = speedMod;
            moveRangeModifier = moveRangeMod;
            stunResistModifier = stunResistMod;
            resistModifier = resistMod;
        }


    }
}


