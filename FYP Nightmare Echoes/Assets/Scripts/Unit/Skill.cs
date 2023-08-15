using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NightmareEchoes.Grid;
using UnityEngine;

//Created by JH
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
        [SerializeField] protected TargetUnitAlignment targetUnitAlignment;
        [SerializeField] protected AOEType aoeType;
        [SerializeField] protected SkillType skillType;
        
        [Header("Two-step targeting")]
        [SerializeField] protected bool hasSecondaryTargeting;
        [SerializeField] protected int secondaryDamage;
        [SerializeField] protected int secondaryRange;
        
        [field: TextArea(1,10)][SerializeField] protected string skillDescription;

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

        public TargetUnitAlignment TargetUnitAlignment
        {
            get => targetUnitAlignment;
            set => targetUnitAlignment = value;
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

        public bool HasSecondaryTargeting
        {
            get => hasSecondaryTargeting;
            set => hasSecondaryTargeting = value;
        }

        public int SecondaryDamage
        {
            get => secondaryDamage;
            set => secondaryDamage = value;
        }

        public int SecondaryRange
        {
            get => secondaryRange;
            set => secondaryRange = value;
        }
        
        public string SkillDescription
        {
            get => skillDescription;
            set => skillDescription = value;
        }

        #endregion

        public virtual bool Cast(Units target)
        {
            throw new System.NotImplementedException();
        }

        public virtual bool Cast(OverlayTile target, List<OverlayTile> aoeTiles)
        {
            throw new System.NotImplementedException();
        }

        public virtual bool SecondaryCast(Units target)
        {
            throw new System.NotImplementedException();
        }

        public virtual bool SecondaryCast(OverlayTile target, List<OverlayTile> aoeTiles)
        {
            throw new System.NotImplementedException();
        }

        public virtual IEnumerator SecondaryStep()
        {
            throw new System.NotImplementedException();
        }
    }

    public enum TargetType
    {
        Single = 0,
        AOE = 1
    }

    public enum TargetArea
    {
        Line = 0,
        Square = 1,
        Crosshair = 2
    }

    public enum TargetUnitAlignment
    {
        Hostile = 0,
        Friendly = 1,
        Both = 2
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
