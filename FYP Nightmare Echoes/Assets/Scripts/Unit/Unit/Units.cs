using NightmareEchoes.Unit.Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using NightmareEchoes.Grid;

//created by Alex
namespace NightmareEchoes.Unit
{
    [RequireComponent(typeof(Rigidbody2D), typeof(PolygonCollider2D)), Serializable]
    public class Units : MonoBehaviour
    {
        [Header("Unit Info")]
        [SerializeField] protected BaseUnitScriptable _unitScriptable;
        [SerializeField] protected string _name;
        [SerializeField] protected Sprite _sprite;
        protected SpriteRenderer spriteRenderer;
        [SerializeField] protected GameObject damageTextPrefab;
        //[SerializeField] protected GameObject popUpTextPrefab;

        public BaseStats baseStats = new BaseStats();
        public BaseStats stats = new BaseStats();
        public ModifiersStruct modifiedStats = new();

        [SerializeField] protected bool _isHostile;
        [SerializeField] protected Direction _direction;
        [SerializeField] protected TypeOfUnit _typeOfUnit;

        [Header("Tile Related")]
        [SerializeField] protected OverlayTile activeTile;

        [Header("Buff Debuff Token")]
        [SerializeField] protected List<Modifier> buffList = new List<Modifier>();
        [SerializeField] protected List<Modifier> debuffList = new List<Modifier>();
        [SerializeField] protected List<Modifier> tokenList = new List<Modifier>();

        [Header("Unit Skills")]
        [SerializeField] protected Skill basicAttack;
        [field: TextArea(1,1)][SerializeField] protected string basicAttackDesc;
        [SerializeField] protected Skill skill1;
        [field: TextArea(1,1)][SerializeField] protected string skill1Desc;
        [SerializeField] protected Skill skill2;
        [field: TextArea(1,1)][SerializeField] protected string skill2Desc;
        [SerializeField] protected Skill skill3;
        [field: TextArea(1,1)][SerializeField] protected string skill3Desc;
        [SerializeField] protected Skill passive;
        [field: TextArea(1,1)][SerializeField] protected string passiveDesc;


        [Tooltip("Sprites are ordered in north, south, east, west")]
        [SerializeField] List<Sprite> sprites = new List<Sprite>(); //ordered in NSEW


        #region Class Properties

        #region Unit Info Properties
        public BaseUnitScriptable UnitScriptable
        {
            get => _unitScriptable;
            private set => _unitScriptable = value;
        }

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

        #region Tile Related
        public OverlayTile ActiveTile
        {
            get => activeTile;
            set => activeTile = value;
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
        public string BasicAttackDesc
        {
            get => basicAttackDesc;
            private set => basicAttackDesc = value;
        }

        public string Skill1Desc
        {
            get => skill1Desc;
            private set => skill1Desc = value;
        }
        public string Skill2Desc
        {
            get => skill2Desc; 
            private set => skill2Desc = value;
        }

        public string Skill3Desc
        {
            get => skill3Desc; 
            private set => skill3Desc = value;
        }

        public string PassiveDesc
        {
            get => passiveDesc;
            private set => passiveDesc = value;
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

            baseStats.Reset();
            UpdateStats();
        }

        protected virtual void Start()
        {
            stats.Health = stats.MaxHealth;
        }
        
        protected virtual void Update()
        {
            if(sprites.Count > 0)
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


        #region Status Effects
        public void UpdateStats()
        {
            modifiedStats = ApplyModifiers(modifiedStats);

            stats.MaxHealth = baseStats.MaxHealth + modifiedStats.healthModifier;
            stats.Health = stats.Health;
            stats.Speed = baseStats.Speed + modifiedStats.speedModifier;
            stats.MoveRange = baseStats.MoveRange + modifiedStats.moveRangeModifier;
            stats.StunResist = baseStats.StunResist + modifiedStats.stunResistModifier;
            stats.Resist = baseStats.Resist + modifiedStats.resistModifier;
        }

        public void ApplyStatusEffects()
        {
            for (int i = 0; i < buffList.Count; i++)
            {
                buffList[i].ApplyEffect(gameObject);
            }

            for (int i = 0; i < debuffList.Count; i++)
            {
                debuffList[i].ApplyEffect(gameObject);
            }

            for (int i = 0; i < tokenList.Count; i++)
            {
                tokenList[i].ApplyEffect(gameObject);
            }
        }


        protected ModifiersStruct ApplyModifiers(ModifiersStruct modifiers)
        {
            ModifiersStruct temp = new();

            for(int i = 0; i < buffList.Count; i++) 
            {
                temp = buffList[i].ApplyModifier(temp);
            }

            for(int i = 0; i < debuffList.Count; i++)
            {
                temp = debuffList[i].ApplyModifier(temp);
            }

            for(int i = 0; i < tokenList.Count; i++)
            {
                temp = tokenList[i].ApplyModifier(temp);
            }

            modifiers = temp;

            return modifiers;
        }

        protected void AddBuff(Modifier buff)
        {
            switch(buff.modifierType)
            {
                case ModifierType.BUFF:
                    BuffList.Add(buff);
                    break;

                case ModifierType.DEBUFF:
                    DebuffList.Add(buff);
                    break;

                case ModifierType.POSITIVETOKEN:
                    TokenList.Add(buff);
                    break;
            }
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
            set => _health = value;
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
