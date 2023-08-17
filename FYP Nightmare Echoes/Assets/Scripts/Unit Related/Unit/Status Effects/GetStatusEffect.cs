using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.Unit
{
    public class GetStatusEffect : MonoBehaviour
    {
        public static GetStatusEffect Instance;

        public List<ModifierDictionary> totalPossibleModifiers = new List<ModifierDictionary>();


        public void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        public Modifier CreateModifier(STATUS_EFFECT enumIndex, int value, int duration)
        {

            foreach(ModifierDictionary modifier in totalPossibleModifiers) 
            {
                if(modifier.statusEffect == enumIndex)
                {
                    Modifier newModifier = Instantiate(modifier.modValue);

                    newModifier.genericValue = value;
                    newModifier.modifierDuration = duration;
                    return newModifier;
                }
            }

            Debug.LogWarning("STATUS EFFECT DOES NOT EXIST YET");
            return null;
        }
    }

    public enum STATUS_EFFECT
    {
        DODGE_TOKEN = 0,
        BLOCK_TOKEN = 1,
        STRENGTH_TOKEN = 2,
        HASTE_TOKEN = 3,
        BARRIER_TOKEN = 4,
        STEALTH_TOKEN = 5,

        BLIND_TOKEN = 6,
        VULNERABLE_TOKEN = 7,
        WEAKEN_TOKEN = 8,
        VERTIGO_TOKEN = 9,
        STUN_TOKEN = 10,
        IMMOBILIZE_TOKEN = 11,

        RESTORATION_BUFF = 12,
        MOVERANGE_BUFF = 13,
        SPEED_BUFF = 14,
        RESISTANCE_BUFF = 15,
        STUN_RESISTANCE_BUFF = 16,

        WOUND_DEBUFF = 17,
        MOVERANGE_DEBUFF = 18,
        SPEED_DEBUFF = 19,
        RESISTANCE_DEBUFF = 20,
        STUN_RESISTANCE_DEBUFF = 21,
    }

    [Serializable]
    public class ModifierDictionary
    {
        public STATUS_EFFECT statusEffect;
        public Modifier modValue;
    }
}
