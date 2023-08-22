using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using NightmareEchoes.Grid;
using System.Linq;

//created by Alex
namespace NightmareEchoes.Unit
{
    [RequireComponent(typeof(Rigidbody2D), typeof(PolygonCollider2D)), Serializable]
    public class Units : MonoBehaviour
    {
        [Header("Unit Info")]
        [SerializeField] protected string _name;
        [SerializeField] protected Sprite _sprite;
        protected SpriteRenderer spriteRenderer;
        [SerializeField] protected GameObject damageTextPrefab;
        //[SerializeField] protected GameObject popUpTextPrefab;

        [SerializeField] protected bool _isHostile;
        [SerializeField] protected Direction _direction;
        [SerializeField] protected TypeOfUnit _typeOfUnit;

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

        [Space(15),Header("Sprite Directions"), Tooltip("Sprites are ordered in north, south, east, west")]
        [SerializeField] List<Sprite> sprites = new List<Sprite>(); //ordered in NSEW

        [Space(15), Header("Tile Related")]
        [SerializeField] protected OverlayTile activeTile;
        [SerializeField] protected OverlayTile isOccupiedTile;

        #region Class Properties

        #region Unit Info Properties
        public Sprite Sprite
        {
            get => _sprite;
            set => _sprite = value;
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
            get => _isHostile;
            private set => IsHostile = value;
        }

        public Direction Direction
        {
            get => _direction;
            set => _direction = value;
        }

        public TypeOfUnit TypeOfUnit
        {
            get => _typeOfUnit;
            set => _typeOfUnit = value;
        }

        #endregion


        #region Positive Token Bool
        public bool DodgeToken
        {
            get => dodgeToken;
            set => dodgeToken = value;
        }

        public bool BlockToken
        {
            get => blockToken;
            set => blockToken = value;
        }

        public bool StrengthToken
        {
            get => strengthToken;
            set => strengthToken = value;
        }

        public bool HasteToken
        {
            get => hasteToken;
            set
            {
                hasteToken = value;

                if(hasteToken == false)
                {
                    UpdateTokenEffect(STATUS_EFFECT.HASTE_TOKEN);
                }
            }
        }

        public bool BarrierToken
        {
            get => barrierToken;
            set => barrierToken = value;
        }

        public bool StealthToken
        {
            get => stealthToken;
            set => stealthToken = value;
        }
        #endregion 


        #region Negative Token Bool
        public bool BlindToken
        {
            get => blindToken;
            set => blindToken = value;
        }

        public bool VulnerableToken
        {
            get => vulnerableToken;
            set => vulnerableToken = value;
        }

        public bool WeakenToken
        {
            get => weakenToken;
            set => weakenToken = value;
        }

        public bool VertigoToken
        {
            get => vertigoToken;
            set => vertigoToken = value;
        }

        public bool StunToken
        {
            get => stunToken;
            set
            {
                stunToken = value;

                if (stunToken == false)
                {
                    UpdateTokenEffect(STATUS_EFFECT.STUN_TOKEN);
                }
            }
        }

        public bool ImmobilizeToken
        {
            get => immobilizeToken;
            set => immobilizeToken = value;
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
                if(basicAttack == null)
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
                if(basicAttack == null)
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
        #endregion


        #region Tile Related
        public OverlayTile ActiveTile
        {
            get => activeTile;
            set => activeTile = value;
        }
        #endregion
        #endregion

        protected virtual void Awake()
        {
            Direction = Direction.North;
            spriteRenderer = GetComponent<SpriteRenderer>();

            //collider presets
            Vector2[] points = new Vector2[4];
            points[0] = new Vector2(-0.45f, 0f);
            points[1] = new Vector2(0f, -0.225f);
            points[2] = new Vector2(0.45f, 0f);
            points[3] = new Vector2(0, 0.225f);

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
            UpdateAllStats();
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
                Destroy(gameObject);
            }

            if (sprites.Count > 0)
            {
                switch (Direction)
                {
                    case Direction.North:
                        SpriteRenderer.sprite = sprites[(int)Direction.North];
                        break;

                    case Direction.South:
                        SpriteRenderer.sprite = sprites[(int)Direction.South];
                        break;

                    case Direction.East:
                        SpriteRenderer.sprite = sprites[(int)Direction.East];
                        break;

                    case Direction.West:
                        SpriteRenderer.sprite = sprites[(int)Direction.West];
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

            UpdateAllStats();
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

        public void UpdateAllStatusEffectLifeTime()
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
        void UpdateTokenEffect(STATUS_EFFECT enumIndex)
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

        public void UpdateAllStats()
        {
            modifiedStats = ApplyAllModifiers(modifiedStats);

            stats.MaxHealth = baseStats.MaxHealth + modifiedStats.healthModifier;
            stats.Health = stats.Health;
            stats.Speed = baseStats.Speed + modifiedStats.speedModifier;
            stats.MoveRange = baseStats.MoveRange + modifiedStats.moveRangeModifier;
            stats.StunResist = baseStats.StunResist + modifiedStats.stunResistModifier;
            stats.Resist = baseStats.Resist + modifiedStats.resistModifier;
        }


        //used in UpdateStats, do not directly call
        protected ModifiersStruct ApplyAllModifiers(ModifiersStruct modifiers)
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