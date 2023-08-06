using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public abstract class Skill : MonoBehaviour
    {
        public string skillName;
        public int cooldown;
        public int range;
        public TargetType targetType;
        public AOEType aoeType;
        public SkillType skillType;

        #region properties

        public string SkillName
        {
            get => skillName;
            set => skillName = value;
        }

        public int Cooldown
        {
            get => cooldown;
            set => cooldown = value;
        }

        public int Range
        {
            get => range;
            set => range = value;
        }

        public TargetType TargetType
        {
            get => targetType;
            set => targetType = value;
        }

        public AOEType AoeType
        {
            get => aoeType;
            set => aoeType = value;
        }

        public SkillType SkillType
        {
            get => skillType;
            set => skillType = value;
        }


        #endregion

        public abstract void Cast(); //for VFX?
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

    public enum SkillType
    {
        Damage = 0,
        Heal = 1,
        Others = 2
    }
    
}
