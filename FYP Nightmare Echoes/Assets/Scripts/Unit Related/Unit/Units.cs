using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using NightmareEchoes.Grid;
using System.Linq;
using NightmareEchoes.Unit.Pathfinding;
using NightmareEchoes.Unit.AI;

//created by Alex, edited by Ter
namespace NightmareEchoes.Unit
{
    [RequireComponent(typeof(PolygonCollider2D), typeof(Rigidbody2D)), Serializable]
    public class Units : MonoBehaviour
    {
        public event Action<Units> OnDestroyedEvent;

        [Header("Unit Info")]
        [SerializeField] protected string _name;
        [SerializeField] protected Sprite sprite;
        protected SpriteRenderer spriteRenderer;
        [SerializeField] protected GameObject damageTextPrefab;
        //[SerializeField] protected GameObject popUpTextPrefab;

        [SerializeField] protected bool isHostile;
        [SerializeField] protected Direction direction;
        [SerializeField] protected TypeOfUnit typeOfUnit;

        public BaseStats baseStats = new BaseStats();
        public BaseStats stats = new BaseStats();
        public ModifiersStruct modifiedStats = new();

        [Space(15), Header("Buff Debuff")]
        [SerializeField] protected List<Modifier> buffList = new List<Modifier>();
        [SerializeField] protected List<Modifier> debuffList = new List<Modifier>();
        [SerializeField] protected List<Modifier> tokenList = new List<Modifier>();

        [Space(15), Header("Positive Token Bools")]
        [SerializeField] protected bool dodgeToken;
        [SerializeField] protected bool blockToken;
        [SerializeField] protected bool strengthToken;
        [SerializeField] protected bool hasteToken;
        [SerializeField] protected bool barrierToken;
        [SerializeField] protected bool stealthToken;

        [Space(15), Header("Negative Token Bools")]
        [SerializeField] protected bool blindToken;
        [SerializeField] protected bool vulnerableToken;
        [SerializeField] protected bool weakenToken;
        [SerializeField] protected bool vertigoToken;
        [SerializeField] protected bool stunToken;
        [SerializeField] protected bool immobilizeToken;


        [Space(15), Header("Unit Skills")]
        [SerializeField] protected Skill basicAttack;
        [SerializeField] protected Skill skill1;
        [SerializeField] protected Skill skill2;
        [SerializeField] protected Skill skill3;
        [SerializeField] protected Skill passive;

        [Space(15), Header("Sprite Directions"), Tooltip("Sprites are ordered in north, south, east, west")]
        [SerializeField] protected List<Sprite> sprites = new List<Sprite>(); //ordered in NSEW
        [SerializeField] protected GameObject frontModel;
        [SerializeField] protected GameObject backModel;
        [SerializeField] protected Animator frontAnimator;
        [SerializeField] protected Animator backAnimator;


        [Space(15), Header("Tile Related")]
        [SerializeField] protected OverlayTile activeTile;

        #region Class Properties

        #region Unit Info Properties
        public Sprite Sprite
        {
            get => sprite;
            set => sprite = value;
        }

        public SpriteRenderer SpriteRenderer
        {
            get => spriteRenderer;
            set => spriteRenderer = value;
        }

        public string Name
        {
            get => _name;
            private set => _name = value;
        }

        public bool IsHostile
        {
            get => isHostile;
            private set => IsHostile = value;
        }

        public Direction Direction
        {
            get => direction;
            set => direction = value;
        }

        public TypeOfUnit TypeOfUnit
        {
            get => typeOfUnit;
            set => typeOfUnit = value;
        }

        #endregion


        #region Positive Token Bool
        public bool DodgeToken
        {
            get => dodgeToken;
            set
            {
                dodgeToken = value;

                if(dodgeToken)
                {

                }
                else
                {
                    UpdateTokenLifeTime(STATUS_EFFECT.DODGE_TOKEN);
                }
            }
        }

        public bool BlockToken
        {
            get => blockToken;
            set
            {
                blockToken = value;

                if (blockToken)
                {
                    vulnerableToken = false;
                    UpdateTokenLifeTime(STATUS_EFFECT.VULNERABLE_TOKEN);
                }
                else
                {
                    UpdateTokenLifeTime(STATUS_EFFECT.BLOCK_TOKEN);
                }
            }
        }

        public bool StrengthToken
        {
            get => strengthToken;
            set
            {
                strengthToken = value;

                if(strengthToken)
                {
                    weakenToken = false;
                    UpdateTokenLifeTime(STATUS_EFFECT.WEAKEN_TOKEN);
                }
                else
                {
                    UpdateTokenLifeTime(STATUS_EFFECT.STRENGTH_TOKEN);
                }
            }
        }

        public bool HasteToken
        {
            get => hasteToken;
            set
            {
                hasteToken = value;

                if(hasteToken)
                {
                    vertigoToken = false;
                    UpdateTokenLifeTime(STATUS_EFFECT.VERTIGO_TOKEN);
                }
                else
                {
                    UpdateTokenLifeTime(STATUS_EFFECT.HASTE_TOKEN);
                }
            }
        }

        public bool BarrierToken
        {
            get => barrierToken;
            set
            {
                barrierToken = value;

                if(barrierToken)
                {

                }
                else
                {
                    UpdateTokenLifeTime(STATUS_EFFECT.BARRIER_TOKEN);
                }
            }
        }

        public bool StealthToken
        {
            get => stealthToken;
            set
            {
                stealthToken = value;

                if(stealthToken)
                {

                }
                else
                {
                    UpdateTokenLifeTime(STATUS_EFFECT.STEALTH_TOKEN);
                }
            }
        }
        #endregion 


        #region Negative Token Bool
        public bool BlindToken
        {
            get => blindToken;
            set
            {
                blindToken = value;

                if(blindToken)
                {

                }
                else
                {
                    UpdateTokenLifeTime(STATUS_EFFECT.BLIND_TOKEN);
                }
            }
        }

        public bool VulnerableToken
        {
            get => vulnerableToken;
            set
            {
                vulnerableToken = value;

                if (vulnerableToken)
                {

                }
                else
                {
                    UpdateTokenLifeTime(STATUS_EFFECT.VULNERABLE_TOKEN);
                }
            }
        }

        public bool WeakenToken
        {
            get => weakenToken;
            set
            {
                weakenToken = value;

                if (weakenToken)
                {

                }
                else
                {
                    UpdateTokenLifeTime(STATUS_EFFECT.WEAKEN_TOKEN);
                }
            }
        }

        public bool VertigoToken
        {
            get => vertigoToken;
            set
            {
                vertigoToken = value;

                if (vertigoToken)
                {
                    
                }
                else
                {
                    UpdateTokenLifeTime(STATUS_EFFECT.VERTIGO_TOKEN);
                }
            }
        }

        public bool StunToken
        {
            get => stunToken;
            set
            {
                stunToken = value;

                if(stunToken)
                {

                }
                else
                {
                    UpdateTokenLifeTime(STATUS_EFFECT.STUN_TOKEN);
                }
            }
        }

        public bool ImmobilizeToken
        {
            get => immobilizeToken;
            set
            {
                immobilizeToken = value;

                if (immobilizeToken)
                {

                }
                else
                {
                    UpdateTokenLifeTime(STATUS_EFFECT.IMMOBILIZE_TOKEN);
                }
            }
        }
        #endregion


        #region Buff Debuff Token
        public List<Modifier> BuffList
        {
            get => buffList;
            set => buffList = value;
        }

        public List<Modifier> DebuffList
        {
            get => debuffList;
            set => debuffList = value;
        }

        public List<Modifier> TokenList
        {
            get => tokenList;
            set => tokenList = value;
        }
        #endregion


        #region Unit Skill Properties

        public string BasicAttackName
        {
            get
            {
                if (basicAttack == null)
                {
                    return null;
                }
                else
                {
                    return basicAttack.SkillName;
                }
            }

            private set => basicAttack.SkillName = value;
        }

        public string BasicAttackDesc
        {
            get
            {
                if (basicAttack == null)
                {
                    return null;
                }
                else
                {
                    return basicAttack.SkillDescription;
                }
            }

            private set => basicAttack.SkillDescription = value;
        }

        public Skill BasicAttackSkill
        {
            get
            {
                if (basicAttack == null)
                {
                    return null;
                }
                else
                {
                    return basicAttack;
                }
            }

            private set => basicAttack = value;
        }

        public string Skill1Name
        {
            get
            {
                if (skill1 == null)
                {
                    return null;
                }
                else
                {
                    return skill1.SkillName;
                }
            }

            private set => skill1.SkillName = value;
        }

        public string Skill1Desc
        {
            get
            {
                if (skill1 == null)
                {
                    return null;
                }
                else
                {
                    return skill1.SkillDescription;
                }
            }

            private set => skill1.SkillDescription = value;
        }
        
        public Skill Skill1Skill
        {
            get
            {
                if (skill1 == null)
                {
                    return null;
                }
                else
                {
                    return skill1;
                }
            }

            private set => skill1 = value;
        }

        public string Skill2Name
        {
            get
            {
                if (skill2 == null)
                {
                    return null;
                }
                else
                {
                    return skill2.SkillName;
                }
            }

            private set => skill2.SkillName = value;
        }

        public string Skill2Desc
        {
            get
            {
                if (skill2 == null)
                {
                    return null;
                }
                else
                {
                    return skill2.SkillDescription;
                }
            }
            private set => skill2.SkillDescription = value;
        }

        public Skill Skill2Skill
        {
            get
            {
                if (skill2 == null)
                {
                    return null;
                }
                else
                {
                    return skill2;
                }
            }

            private set => skill2 = value;
        }

        public string Skill3Name
        {
            get
            {
                if (skill3 == null)
                {
                    return null;
                }
                else
                {
                    return skill3.SkillName;
                }
            }

            private set => skill3.SkillName = value;
        }

        public string Skill3Desc
        {
            get
            {
                if (skill3 == null)
                {
                    return null;
                }
                else
                {
                    return skill3.SkillDescription;
                }
            }
            private set => skill3.SkillDescription = value;
        }

        public Skill Skill3Skill
        {
            get
            {
                if (skill3 == null)
                {
                    return null;
                }
                else
                {
                    return skill3;
                }
            }

            private set => skill3 = value;
        }
        public string PassiveName
        {
            get
            {
                if (passive == null)
                {
                    return null;
                }
                else
                {
                    return passive.SkillName;
                }
            }

            private set => passive.SkillName = value;
        }

        public string PassiveDesc
        {
            get
            {
                if (passive == null)
                {
                    return null;
                }
                else
                {
                    return passive.SkillDescription;
                }
            }

            private set => passive.SkillDescription = value;
        }

        public Skill PassiveSkill
        {
            get
            {
                if (passive == null)
                {
                    return null;
                }
                else
                {
                    return passive;
                }
            }

            private set => passive = value;
        }
        #endregion


        #region Tile Related
        public OverlayTile ActiveTile
        {
            get => activeTile;
            set => activeTile = value;
        }
        #endregion


        #region Sprites Directions
        public GameObject FrontModel
        {
            get => frontModel;
            private set => frontModel = value;
        }

        public GameObject BackModel
        {
            get => backModel;
            private set => backModel = value;
        }

        public Animator FrontAnimator
        {
            get => frontAnimator;
            private set => frontAnimator = value;
        }

        public Animator BackAnimator
        {
            get => backAnimator;
            private set => backAnimator = value;
        }

        #endregion

        #endregion

        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            //collider presets
            Vector2[] points = new Vector2[4];
            points[0] = new Vector2(-0.85f, 0f);
            points[1] = new Vector2(0f, -0.45f);
            points[2] = new Vector2(0.85f, 0f);
            points[3] = new Vector2(0, 0.45f);

            PolygonCollider2D polyCollider = GetComponent<PolygonCollider2D>();
            polyCollider.points = points;
            polyCollider.isTrigger = true;

            //rb2d presets
            Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
            rb2D.freezeRotation = true;
            rb2D.gravityScale = 0f;

            //MANDATORY, DO NOT REMOVE
            baseStats.Reset();
            AwakeAllStatusEffects();
            UpdateStatsWithoutEndCycleEffect();
        }

        protected virtual void Start()
        {
            stats.Health = stats.MaxHealth;
            //AddBuff(GetStatusEffect.Instance.CreateModifier(STATUS_EFFECT.MOVERANGE_BUFF, 2, 1));
        }

        protected virtual void Update()
        {
            if (stats.Health == 0)
            {
                if (!IsHostile)
                {
                    PathfindingManager.Instance.HideTilesInRange(PathfindingManager.Instance.playerTilesInRange);
                }
                else
                {
                    PathfindingManager.Instance.HideTilesInRange(GetComponent<BasicEnemyAI>().TilesInRange);
                }

                OnDestroyedEvent?.Invoke(this);
                Destroy(gameObject);
            }

            if (sprites.Count > 0)
            {
                switch (Direction)
                {
                    case Direction.North: //back facing

                        if (SpriteRenderer != null)
                        {
                            SpriteRenderer.sprite = sprites[(int)Direction.North];
                        }
                        break;

                    case Direction.South: //front facing
                        if (SpriteRenderer != null)
                        {
                            SpriteRenderer.sprite = sprites[(int)Direction.South];

                        }
                        break;

                    case Direction.East: //front facing
                        if (SpriteRenderer != null)
                        {
                            SpriteRenderer.sprite = sprites[(int)Direction.East];

                        }
                        break;

                    case Direction.West: //back facing
                        if (SpriteRenderer != null)
                        {
                            SpriteRenderer.sprite = sprites[(int)Direction.West];

                        }
                        break;

                }
            }
            else
            {
                switch (Direction)
                {
                    case Direction.North: //back facing
                        if (frontModel != null && backModel != null)
                        {
                            frontModel.SetActive(false);
                            backModel.SetActive(true);
                        }

                        transform.localRotation = Quaternion.Euler(0, 0, 0);
                        break;

                    case Direction.South: //front facing
                        if (frontModel != null && backModel != null)
                        {
                            frontModel.SetActive(true);
                            backModel.SetActive(false);
                        }

                        transform.localRotation = Quaternion.Euler(0, 0, 0);
                        break;

                    case Direction.East: //front facing
                        if (frontModel != null && backModel != null)
                        {
                            frontModel.SetActive(true);
                            backModel.SetActive(false);
                        }

                        transform.localRotation = Quaternion.Euler(0, 180, 0);
                        break;

                    case Direction.West: //back facing
                        if (frontModel != null && backModel != null)
                        {
                            frontModel.SetActive(false);
                            backModel.SetActive(true);
                        }

                        transform.localRotation = Quaternion.Euler(0, 180, 0);
                        break;

                }
            }
        }


        #region Override Functions
        public virtual void Move()
        {

        }

        public virtual void BasicAttack()
        {

        }


        public virtual void Passive()
        {

        }


        public virtual void Skill1()
        {

        }


        public virtual void Skill2()
        {

        }


        public virtual void Skill3()
        {

        }


        public virtual void TakeDamage(int damage)
        {
            if (frontModel != null && frontModel.activeSelf && frontAnimator != null)
            {
                frontAnimator.SetBool("GettingHit", true);
            }
            else if (backModel != null && backModel.activeSelf && backAnimator != null)
            {
                backAnimator.SetBool("GettingHit", true);
            }

        }

        public virtual void HealUnit(int healAmount)
        {
            if (frontModel != null && frontModel.activeSelf && frontAnimator != null)
            {
                frontAnimator.SetBool("GettingHealed", true);
            }
            else if (backModel != null && backModel.activeSelf && backAnimator != null)
            {
                backAnimator.SetBool("GettingHealed", true);
            }

        }

        [ContextMenu("Destroy Object")]
        public void DestroyObject()
        {
            Destroy(gameObject);
        }

        #endregion


        #region Utility
        public void ShowPopUpText(string damage)
        {
            if(damageTextPrefab)
            {
                GameObject prefab = Instantiate(damageTextPrefab, transform.localPosition + new Vector3(0.1f, 0.4f, 0), Quaternion.identity);
                TextMeshPro textMeshPro = prefab.GetComponentInChildren<TextMeshPro>();
                textMeshPro.text = damage;
            }
        }

        public void UpdateLocation()
        {
            var hitTile = Physics2D.Raycast(transform.position, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Overlay Tile"));
            if (hitTile)
                activeTile = hitTile.collider.gameObject.GetComponent<OverlayTile>();
        }
        #endregion


        #region Status Effects Updates
        //call to add buff to unit
        public void AddBuff(Modifier buff)
        {
            switch (buff.modifierType)
            {
                case ModifierType.BUFF:
                    buff.AwakeStatusEffect();
                    BuffList.Add(buff);
                    break;

                case ModifierType.DEBUFF:
                    buff.AwakeStatusEffect();
                    DebuffList.Add(buff);
                    break;

                case ModifierType.POSITIVETOKEN:
                    buff.AwakeStatusEffect();
                    TokenList.Add(buff);
                    break;

                case ModifierType.NEGATIVETOKEN:
                    buff.AwakeStatusEffect();
                    TokenList.Add(buff);
                    break;
            }

            UpdateStatsWithoutEndCycleEffect();
        }

        //call only on instantiation of object
        public void AwakeAllStatusEffects()
        {
            List<Modifier> totalStatusEffects = BuffList.Concat(DebuffList).Concat(TokenList).ToList();

            foreach (Modifier statusEffect in totalStatusEffects)
            {
                statusEffect.AwakeStatusEffect();
            }
        }

        public void ApplyAllBuffDebuffs()
        {
            for (int i = 0; i < BuffList.Count; i++)
            {
                BuffList[i].ApplyEffect(gameObject);
            }

            for (int i = 0; i < DebuffList.Count; i++)
            {
                DebuffList[i].ApplyEffect(gameObject);
            }
        }

        public void ApplyAllTokenEffects()
        {
            for (int i = 0; i < TokenList.Count; i++)
            {
                TokenList[i].ApplyEffect(gameObject);
            }
        }

        public void UpdateBuffDebuffLifeTime()
        {
            for (int i = BuffList.Count - 1; i >= 0 ; i--)
            {
                BuffList[i].UpdateLifeTime();

                if (BuffList[i].ReturnLifeTime() <= 0)
                {
                    BuffList.RemoveAt(i);
                }
            }

            for (int i = DebuffList.Count - 1; i >= 0; i--)
            {
                DebuffList[i].UpdateLifeTime();

                if (DebuffList[i].ReturnLifeTime() <= 0)
                {
                    DebuffList.RemoveAt(i);
                }
            }
        }

        //only call in class properties
        void UpdateTokenLifeTime(STATUS_EFFECT enumIndex)
        {
            for (int i = TokenList.Count - 1; i >= 0; i--)
            {
                if (TokenList[i].statusEffect == enumIndex)
                {
                    TokenList[i].UpdateLifeTime();


                    if (TokenList[i].ReturnLifeTime() <= 0)
                    {
                        TokenList.RemoveAt(i);
                    }
                }
            }
        }

        public void UpdateStatsWithoutEndCycleEffect()
        {
            modifiedStats = ApplyModifiersWithoutEndCycleEffect(modifiedStats);

            stats.MaxHealth = baseStats.MaxHealth + modifiedStats.healthModifier;
            stats.Health = stats.Health;
            stats.Speed = baseStats.Speed + modifiedStats.speedModifier;
            stats.MoveRange = baseStats.MoveRange + modifiedStats.moveRangeModifier;
            stats.StunResist = baseStats.StunResist + modifiedStats.stunResistModifier;
            stats.Resist = baseStats.Resist + modifiedStats.resistModifier;
        }

        public void UpdateStatsWithEndCycleEffect()
        {
            modifiedStats = ApplyModifiersWithEndCycleEffect(modifiedStats);

            stats.MaxHealth = baseStats.MaxHealth + modifiedStats.healthModifier;
            stats.Health = stats.Health;
            stats.Speed = baseStats.Speed + modifiedStats.speedModifier;
            stats.MoveRange = baseStats.MoveRange + modifiedStats.moveRangeModifier;
            stats.StunResist = baseStats.StunResist + modifiedStats.stunResistModifier;
            stats.Resist = baseStats.Resist + modifiedStats.resistModifier;
        }

        //used in UpdateStats, do not directly call
        protected ModifiersStruct ApplyModifiersWithoutEndCycleEffect(ModifiersStruct modifiers)
        {
            ModifiersStruct temp = new();

            for (int i = 0; i < buffList.Count; i++)
            {
                temp = buffList[i].ApplyModifier(temp);
            }

            for (int i = 0; i < debuffList.Count; i++)
            {
                temp = debuffList[i].ApplyModifier(temp);
            }

            for (int i = 0; i < tokenList.Count; i++)
            {
                if (tokenList[i].statusEffect == STATUS_EFFECT.HASTE_TOKEN || tokenList[i].statusEffect == STATUS_EFFECT.VERTIGO_TOKEN)
                {
                    continue;
                }

                temp = tokenList[i].ApplyModifier(temp);
            }

            modifiers = temp;

            return modifiers;
        }

        protected ModifiersStruct ApplyModifiersWithEndCycleEffect(ModifiersStruct modifiers)
        {
            ModifiersStruct temp = new();

            for (int i = 0; i < buffList.Count; i++)
            {
                temp = buffList[i].ApplyModifier(temp);
            }

            for (int i = 0; i < debuffList.Count; i++)
            {
                temp = debuffList[i].ApplyModifier(temp);
            }

            for (int i = 0; i < tokenList.Count; i++)
            {
                temp = tokenList[i].ApplyModifier(temp);
            }

            modifiers = temp;

            return modifiers;
        }

        #endregion
    }

    public enum Direction
    {
        North = 0,
        South = 1,
        East = 2,
        West = 3,
    }


    public enum TypeOfUnit
    {
        RANGED_UNIT = 0,
        MELEE_UNIT = 1,
    }

    [Serializable]
    public class BaseStats
    {
        [Header("Unit Info")]
        protected int _health;
        [SerializeField] protected int _maxHealth;
        [SerializeField] protected int _speed;
        [SerializeField] protected int _moveRange;
        [SerializeField] protected float _stunResist;
        [SerializeField] protected float _resist;

        #region Unit Info Properties
        public int Health
        {
            get => _health;
            set
            {
                _health = Mathf.Clamp(value, 0, _maxHealth);
            }
        }

        public int MaxHealth
        {
            get => _maxHealth;
            set => _maxHealth = value;
        }

        public int Speed
        {
            get => _speed;
            set => _speed = value;
        }

        public int MoveRange
        {
            get => _moveRange;
            set => _moveRange = value;
        }

        public float StunResist
        {
            get => _stunResist;
            set => _stunResist = value;
        }

        public float Resist
        {
            get => _resist;
            set => _resist = value;
        }
        #endregion


        public void Reset()
        {
            MaxHealth = _maxHealth;
            Health = _health;
            Speed = _speed;
            MoveRange = _moveRange;
            StunResist = _stunResist;
            Resist = _resist;
        }

    }
}
