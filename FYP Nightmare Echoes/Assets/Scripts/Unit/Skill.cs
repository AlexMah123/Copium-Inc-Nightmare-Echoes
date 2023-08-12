using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public abstract class Skill : MonoBehaviour
    {
        [SerializeField] protected string skillName;
        [SerializeField] protected int damage;
        [SerializeField] protected int cooldown;
        [SerializeField] protected int range;
        [SerializeField] protected TargetType targetType;
        [SerializeField] protected TargetArea targetArea;
        [SerializeField] protected AOEType aoeType;
        [SerializeField] protected SkillType skillType;
        
        [field: TextArea(1,1)][SerializeField] protected string skillDescription;

        #region properties

        public string SkillName
        {
            get => skillName;
            set => skillName = value;
        }

        public int Damage
        {
            get => damage;
            set => damage = value;
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
        
        public TargetArea TargetArea
        {
            get => targetArea;
            set => targetArea = value;
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

        public string SkillDescription
        {
            get => skillDescription;
            set => skillDescription = value;
        }

        #endregion

        public abstract void Cast(Units target);
    }

    public enum TargetType
    {
        Single = 0,
        Multi = 1,
        AOE = 2
    }

    public enum TargetArea
    {
        Line = 0,
        Square = 1
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
