using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class Skill : MonoBehaviour
    {
        public string skillName;
        public int coolDown;
        public int range;
        public TargetType targetType;
        public AOEType aoeType;
    }

    public enum TargetType
    {
        Single = 0,
        Multi = 1,
        AOE = 2
    }

    public enum AOEType
    {
        NonAOE = 0,
        Square = 1,
        Cross = 2
    }
    
}
