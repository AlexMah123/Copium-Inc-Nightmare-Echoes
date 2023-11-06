using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NightmareEchoes.Grid;
using NightmareEchoes.Unit.Combat;
using NightmareEchoes.Unit.Pathfinding;
using UnityEngine;

//Created by JH, edited by Ter
namespace NightmareEchoes.Unit
{
    public abstract class Skill : MonoBehaviour
    {
        protected Entity thisUnit;
        [Header("Skill Details")]
        [SerializeField] protected string skillName;
        [SerializeField] protected int damage;
        [SerializeField] protected int heal;
        [SerializeField] protected float cooldown;
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
        [SerializeField] protected int maxCount;
        [SerializeField] protected GameObject placableGameObject;

        protected bool onCooldown;
        protected float cd;
        public Coroutine animationCoroutine = null;

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

        public float Cooldown
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

        public int MaxCount
        {
            get => maxCount;
            set => maxCount = value;
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

        public bool OnCooldown
        {
            get => onCooldown;
            set => onCooldown = value;
        }

        public float Cd
        {
            get => cd;
            set => cd = value;
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
            thisUnit.ShowPopUpText(skillName, Color.red);

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
            thisUnit.ShowPopUpText(skillName, Color.red);

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
                    if (target.CheckEntityGameObjectOnTile()?.GetComponent<Entity>().Direction == Direction.NORTH)
                    {
                        isBackstabbing = true;
                    }
                }
                else
                {
                    //south
                    thisUnit.Direction = Direction.SOUTH;
                    if (target.CheckEntityGameObjectOnTile()?.GetComponent<Entity>().Direction == Direction.SOUTH)
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
                    if (target.CheckEntityGameObjectOnTile()?.GetComponent<Entity>().Direction == Direction.EAST)
                    {
                        isBackstabbing = true;
                    }
                }
                else
                {
                    //west
                    thisUnit.Direction = Direction.WEST;
                    if (target.CheckEntityGameObjectOnTile()?.GetComponent<Entity>().Direction == Direction.WEST)
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
        protected bool DealDamage(Entity target, int secondaryDmg = 0, bool checkBlind = true, bool checkDodge = true)
        {
            var damage = this.damage;
            if (secondaryDmg > 0) damage = secondaryDmg;
            
            #region Token Checks Before Dealing Dmg

            if (thisUnit.BlindToken && targetType == TargetType.Single && checkBlind)
            {
                if (thisUnit.DoesModifierExist(STATUS_EFFECT.BLIND_TOKEN).genericValue > UnityEngine.Random.Range(0, 101))
                {
                    thisUnit.ShowPopUpText($"Blinded, attack missed!!", Color.red);
                    thisUnit.UpdateTokenLifeTime(STATUS_EFFECT.BLIND_TOKEN);

                    return false;
                }
                else
                {
                    thisUnit.ShowPopUpText($"Blind did not work!", Color.red);

                    if (thisUnit.WeakenToken)
                    {
                        int newDamage = Mathf.RoundToInt(damage * 0.5f);
                        CheckForBackstab(target, newDamage, checkDodge);

                        thisUnit.ShowPopUpText($"Attack was weakened!", Color.red);
                        thisUnit.UpdateTokenLifeTime(STATUS_EFFECT.WEAKEN_TOKEN);
                    }
                    else if (thisUnit.StrengthToken)
                    {
                        int newDamage = Mathf.RoundToInt(damage * 1.5f);
                        CheckForBackstab(target, newDamage, checkDodge);

                        thisUnit.ShowPopUpText($"Attack was strengthen!", Color.red);
                        thisUnit.UpdateTokenLifeTime(STATUS_EFFECT.STRENGTH_TOKEN);
                    }
                    else
                    {
                        CheckForBackstab(target, damage, checkDodge);
                    }

                }

                thisUnit.UpdateTokenLifeTime(STATUS_EFFECT.BLIND_TOKEN);
            }
            else if (thisUnit.WeakenToken)
            {
                int newDamage = Mathf.RoundToInt(damage * 0.5f);
                CheckForBackstab(target, newDamage, checkDodge);

                thisUnit.ShowPopUpText($"Attack was weakened!", Color.red);
                thisUnit.UpdateTokenLifeTime(STATUS_EFFECT.WEAKEN_TOKEN);
            }
            else if (thisUnit.StrengthToken)
            {
                int newDamage = Mathf.RoundToInt(damage * 1.5f);
                CheckForBackstab(target, newDamage, checkDodge);

                thisUnit.ShowPopUpText($"Attack was strengthen!", Color.red);
                thisUnit.UpdateTokenLifeTime(STATUS_EFFECT.STRENGTH_TOKEN);
            }
            else
            {
                CheckForBackstab(target, damage, checkDodge);
            }

            return true;
            #endregion 
        }

        private void CheckForBackstab(Entity target, int damage, bool checkDodge)
        {
            if (isBackstabbing && targetType == TargetType.Single)
            {
                target.TakeDamage(damage + backstabBonus, checkDodge);
            }
            else
            {
                target.TakeDamage(damage, checkDodge);
            }
            isBackstabbing = false;
        }

        protected void Knockback(OverlayTile originTile, Entity target)
        {
            var prevDir = target.Direction;
            var position = target.transform.position;
            var direction = position - originTile.transform.position;
            direction = Vector3.Normalize(direction);
            var destination = position + direction;
            
            var tileDestination = OverlayTileManager.Instance.GetOverlayTileInWorldPos(destination);
            if (!tileDestination) return;

            if (tileDestination.CheckEntityGameObjectOnTile() || tileDestination.CheckObstacleOnTile()) return;
            
            StartCoroutine(PathfindingManager.Instance.MoveTowardsTile(target, tileDestination, 0.15f));
            target.Direction = prevDir;
            target.CheckCrippled();
        }

        public virtual void Reset()
        {
            
        }

        public void CheckCooldown(bool startCooldown)
        {
            if (startCooldown)
            {
                cd = cooldown;
                onCooldown = true;
            }
            else
            {
                cd -= 1;
                if (cd <= 0)
                    onCooldown = false;
            }
        }

        public IEnumerator PlaySkillAnimation(Entity unit, string animBoolName, bool reset = true)
        {
            if (unit.Direction == Direction.NORTH || unit.Direction == Direction.WEST)
            {
                unit.BackAnimator.SetBool(animBoolName, true);

                yield return new WaitUntil(() => unit.BackAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1);

                yield return new WaitForSeconds(0.25f);
                unit.BackAnimator.SetBool(animBoolName, false);
            }
            else if (unit.Direction == Direction.SOUTH || unit.Direction == Direction.EAST)
            {
                unit.FrontAnimator.SetBool(animBoolName, true);
                yield return new WaitUntil(() => unit.FrontAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1);

                yield return new WaitForSeconds(0.25f);
                unit.FrontAnimator.SetBool(animBoolName, false);
            }

            if (reset)
            {
                unit.ResetAnimator();
            }

            animationCoroutine = null;
            yield return null;
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
        FrontalAttack = 3,
        Diamond = 4,
        DiamondGap = 5,
        SquareGap = 6
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
