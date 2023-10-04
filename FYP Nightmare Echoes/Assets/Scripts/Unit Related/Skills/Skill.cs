using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NightmareEchoes.Grid;
using NightmareEchoes.Unit.Combat;
using UnityEngine;

//Created by JH
namespace NightmareEchoes.Unit
{
    public abstract class Skill : MonoBehaviour
    {
        protected Entity thisUnit;
        [Header("Skill Details")]
        [SerializeField] protected string skillName;
        [SerializeField] protected int damage;
        [SerializeField] protected int heal;
        [SerializeField] protected int cooldown;
        [SerializeField] protected int range;
        [SerializeField] protected int minRange;
        [SerializeField] protected TargetType targetType;
        [SerializeField] protected TargetArea targetArea;
        [SerializeField] protected TargetUnitAlignment targetUnitAlignment;

        [Header("Skill AOE")]
        [SerializeField] protected AOEType aoeType;
        [SerializeField] protected int aoeSize = 1;
        [SerializeField] protected int aoeOffset = 0;
        [SerializeField] protected int aoeDuration;
        [SerializeField] protected Color aoeColor;

        [Header("Skill Type")]
        [SerializeField] protected SkillType skillType;
        
        [SerializeField] protected int secondaryDamage;
        [SerializeField] protected int secondaryRange;

        [Header("Skill Effects")]
        [SerializeField] protected int debuffChance;
        [SerializeField] protected int hitChance = 100;
        [SerializeField] protected int stunChance;

        [Header("Additional Effects")]
        [SerializeField] protected bool inflictKnockback;
        [SerializeField] protected bool isBackstabbing;
        [SerializeField] protected int backstabBonus = 3;
        [SerializeField] protected bool placable;
        [SerializeField] protected int placableCount;
        [SerializeField] protected GameObject placableGameObject;
        
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

        public int MinRange
        {
            get => minRange;
            set => minRange = value;
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

        public int AoeSize
        {
            get => aoeSize;
            set => aoeSize = value;
        }

        public int AoeOffset
        {
            get => aoeOffset;
            set => aoeOffset = value;
        }

        public int AoeDuration
        {
            get => aoeDuration;
            set => aoeDuration = value;
        }

        public Color AoeColor
        {
            get => aoeColor;
            set => aoeColor = value;
        }

        public SkillType SkillType
        {
            get => skillType;
            set => skillType = value;
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

        public bool InflictKnockback
        {
            get => inflictKnockback;
            set => inflictKnockback = value;
        }

        public bool Placable
        {
            get => placable;
            set => placable = value;
        }

        public int PlacableCount
        {
            get => placableCount;
            set => placableCount = value;
        }

        public GameObject PlacableGameObject
        {
            get => placableGameObject;
            set => placableGameObject = value;
        }

        public int HitChance
        {
            get => hitChance;
            private set => hitChance = value;
        }

        public int StunChance
        {
            get => stunChance;
            private set => stunChance = value;
        }

        public int DebuffChance
        {
            get => debuffChance;
            private set => debuffChance = value;
        }

        #endregion

        private void Awake()
        {
            thisUnit = GetComponent<Entity>();
        }

        #region Cast Related

        //Directly on units
        public virtual bool Cast(Entity target)
        {
            Vector2 CastFrom = new Vector2(thisUnit.ActiveTile.gridLocation.x, thisUnit.ActiveTile.gridLocation.y);
            Vector2 CastTo = new Vector2(target.ActiveTile.gridLocation.x, target.ActiveTile.gridLocation.y);

            float xDist = CastFrom.x - CastTo.x;
            float yDist = CastFrom.y - CastTo.y;

            if (Mathf.Abs(xDist) > Mathf.Abs(yDist))
            {
                if (xDist < 0)
                {
                    //north
                    thisUnit.Direction = Direction.NORTH;
                    if (target.Direction == Direction.NORTH) 
                    {
                        isBackstabbing = true;
                    }
                }
                else
                {
                    //south
                    thisUnit.Direction = Direction.SOUTH;
                    if (target.Direction == Direction.SOUTH)
                    {
                        isBackstabbing = true;
                    }
                }
            }
            else
            {
                if (yDist > 0)
                {
                    //east
                    thisUnit.Direction = Direction.EAST;
                    if (target.Direction == Direction.EAST)
                    {
                        isBackstabbing = true;
                    }
                }
                else
                {
                    //west
                    thisUnit.Direction = Direction.WEST;
                    if (target.Direction == Direction.WEST)
                    {
                        isBackstabbing = true;
                    }
                }
            }

            return false;
        }

        //For ground
        public virtual bool Cast(OverlayTile target, List<OverlayTile> aoeTiles)
        {
            Vector2 CastFrom = new Vector2(thisUnit.ActiveTile.gridLocation.x, thisUnit.ActiveTile.gridLocation.y);
            Vector2 CastTo = new Vector2(target.gridLocation.x, target.gridLocation.y);

            float xDist = CastFrom.x - CastTo.x;
            float yDist = CastFrom.y - CastTo.y;

            if (Mathf.Abs(xDist) > Mathf.Abs(yDist))
            {
                if (xDist < 0)
                {
                    //north
                    thisUnit.Direction = Direction.NORTH;
                    if (target.CheckUnitOnTile()?.GetComponent<Entity>().Direction == Direction.NORTH)
                    {
                        isBackstabbing = true;
                    }
                }
                else
                {
                    //south
                    thisUnit.Direction = Direction.SOUTH;
                    if (target.CheckUnitOnTile()?.GetComponent<Entity>().Direction == Direction.SOUTH)
                    {
                        isBackstabbing = true;
                    }
                }
            }
            else
            {
                if (yDist > 0)
                {
                    //east
                    thisUnit.Direction = Direction.EAST;
                    if (target.CheckUnitOnTile()?.GetComponent<Entity>().Direction == Direction.EAST)
                    {
                        isBackstabbing = true;
                    }
                }
                else
                {
                    //west
                    thisUnit.Direction = Direction.WEST;
                    if (target.CheckUnitOnTile()?.GetComponent<Entity>().Direction == Direction.WEST)
                    {
                        isBackstabbing = true;
                    }
                }
            }

            return false;
        }
        
        //For specials
        public virtual bool Cast()
        {
            throw new System.NotImplementedException();
        }

        public virtual bool SecondaryCast()
        {
            return false;
        }

        #endregion
        
        //used for attacks that are damage type
        protected bool DealDamage(Entity target, int secondaryDmg = 0)
        {
            var damage = this.damage;
            if (secondaryDmg > 0) damage = secondaryDmg;
            
            #region Token Checks Before Dealing Dmg

            if (thisUnit.BlindToken && targetType == TargetType.Single)
            {
                if (thisUnit.FindModifier(STATUS_EFFECT.BLIND_TOKEN).genericValue > UnityEngine.Random.Range(0, 101))
                {
                    thisUnit.ShowPopUpText($"Blinded!", Color.red);
                    return false;
                }
                else
                {
                    thisUnit.ShowPopUpText($"Blind did not work!", Color.red);

                    if (thisUnit.WeakenToken)
                    {
                        int newDamage = Mathf.RoundToInt(damage * 0.5f);
                        CheckForBackstab(target, newDamage);

                        thisUnit.ShowPopUpText($"Attack was weakened!", Color.red);
                        thisUnit.UpdateTokenLifeTime(STATUS_EFFECT.WEAKEN_TOKEN);
                    }
                    else if (thisUnit.StrengthToken)
                    {
                        int newDamage = Mathf.RoundToInt(damage * 1.5f);
                        CheckForBackstab(target, newDamage);

                        thisUnit.ShowPopUpText($"Attack was strengthen!", Color.red);
                        thisUnit.UpdateTokenLifeTime(STATUS_EFFECT.STRENGTH_TOKEN);
                    }
                    else
                    {
                        CheckForBackstab(target, damage);
                    }

                }

                thisUnit.UpdateTokenLifeTime(STATUS_EFFECT.BLIND_TOKEN);
            }
            else if (thisUnit.WeakenToken)
            {
                int newDamage = Mathf.RoundToInt(damage * 0.5f);
                CheckForBackstab(target, newDamage);

                thisUnit.ShowPopUpText($"Attack was weakened!", Color.red);
                thisUnit.UpdateTokenLifeTime(STATUS_EFFECT.WEAKEN_TOKEN);
            }
            else if (thisUnit.StrengthToken)
            {
                int newDamage = Mathf.RoundToInt(damage * 1.5f);
                CheckForBackstab(target, newDamage);

                thisUnit.ShowPopUpText($"Attack was strengthen!", Color.red);
                thisUnit.UpdateTokenLifeTime(STATUS_EFFECT.STRENGTH_TOKEN);
            }
            else
            {
                CheckForBackstab(target, damage);
            }

            return true;
            #endregion 
        }

        private void CheckForBackstab(Entity target, int damage)
        {
            if (isBackstabbing && targetType == TargetType.Single)
            {
                target.TakeDamage(damage + backstabBonus);
            }
            else
            {
                target.TakeDamage(damage);
            }
            isBackstabbing = false;
        }

        protected void Knockback(OverlayTile originTile, Entity target)
        {
            var position = target.transform.position;
            var direction = position - originTile.transform.position;
            direction = Vector3.Normalize(direction);
            var destination = position + direction;
            
            var tileDestination = OverlayTileManager.Instance.GetOverlayTileOnWorldPosition(destination);
            if (!tileDestination) return;

            if (tileDestination.CheckUnitOnTile() || tileDestination.CheckObstacleOnTile()) return;
            
            target.transform.position = tileDestination.transform.position;
            target.UpdateLocation();
        }

        public virtual void Reset()
        {
            
        }
    }
    
    
    #region Enums
    public enum TargetType
    {
        Single = 0,
        AOE = 1,
        Self = 2
    }
    
    public enum TargetArea
    {
        Line = 0,
        Square = 1,
        Crosshair = 2,
        FrontalAttack = 3
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

    #endregion
    
}
