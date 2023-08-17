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

        public List<ModifierDictionary> TotalPossibleModifiers = new List<ModifierDictionary>();


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

        public Modifier CreateModifier(string name, int value, int duration)
        {
            foreach(ModifierDictionary modifier in TotalPossibleModifiers) 
            {
                if(modifier.modifierName == name)
                {
                    Modifier newModifier = Instantiate(modifier.modValue);

                    newModifier.genericValue = value;
                    newModifier.modifierDuration = duration;
                    return newModifier;
                }
            }

            return null;
        }
    }

    [Serializable]
    public class ModifierDictionary
    {
        public string modifierName;
        public Modifier modValue;
    }
}
