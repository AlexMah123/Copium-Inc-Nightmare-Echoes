using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    [RequireComponent(typeof(Rigidbody2D), typeof(PolygonCollider2D))]
    public abstract class BaseUnit : MonoBehaviour
    {
        [Header("Unit Info")]
        [SerializeField] protected BaseUnitScriptable _unitScriptable;
        [SerializeField] protected Sprite _sprite;
        [SerializeField] protected GameObject damageTextPrefab;
        [SerializeField] protected string _name;
        protected int _health;
        [SerializeField] protected int _maxHealth;
        [SerializeField] protected int _speed;
        [SerializeField] protected int _moveRange;
        [SerializeField] protected int _stunResist;
        [SerializeField] protected int _resist;

        [SerializeField] protected bool _isHostile;
        [SerializeField] protected Direction _direction;
        [SerializeField] protected StatusEffect _statusEffect;
        
        [Header("Unit Skills")]
        [SerializeField] protected Skill basicAttack;
        [field: TextArea(1,1)][SerializeField] protected string basicAttackDesc;
        [SerializeField] protected Skill skill1;
        [field: TextArea(1,1)][SerializeField] protected string skill1Desc;
        [SerializeField] protected Skill skill2;
        [field: TextArea(1,1)][SerializeField] protected string skill2Desc;
        [SerializeField] protected Skill skill3;
        [field: TextArea(1,1)][SerializeField] protected string skill3Desc;
        [field: TextArea(1,1)][SerializeField] protected string passiveDesc;


        [Tooltip("Sprites are ordered in north, south, east, west")]
        [SerializeField] List<Sprite> sprites = new List<Sprite>(); //ordered in NSEW

        SpriteRenderer spriteRenderer;

        #region Class Properties
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

        public string Name
        {
            get => _name;
            private set => _name = value;
        }

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

        public int StunResist
        {
            get => _stunResist;
            set => _stunResist = value;
        }

        public int Resist
        {
            get => _resist;
            set => _resist = value;
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

        public StatusEffect StatusEffect
        {
            get => _statusEffect;
            set => _statusEffect = value;
        }

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


        }

        protected virtual void Start()
        {
            _health = _maxHealth;
        }

        protected virtual void Update()
        {
            if(sprites.Count > 0)
            {
                switch (Direction)
                {
                    case Direction.North:
                        spriteRenderer.sprite = sprites[(int)Direction.North];
                        break;

                    case Direction.South:
                        spriteRenderer.sprite = sprites[(int)Direction.South];
                        break;

                    case Direction.East:
                        spriteRenderer.sprite = sprites[(int)Direction.East];
                        break;

                    case Direction.West:
                        spriteRenderer.sprite = sprites[(int)Direction.West];
                        break;

                }
            }

            

        }

        public abstract void BasicAttack();

        public abstract void Passive();

        public abstract void Skill1();

        public abstract void Skill2();

        public abstract void Skill3();

        public abstract void TakeDamage(int damage);

        protected void ShowDamage(string damage)
        {
            if(damageTextPrefab)
            {
                GameObject prefab = Instantiate(damageTextPrefab, transform.localPosition + new Vector3(0.1f, 0.4f, 0), Quaternion.identity);
                TextMeshPro textMeshPro = prefab.GetComponentInChildren<TextMeshPro>();
                textMeshPro.text = damage;
            }
        }

        #region collision
        protected virtual void OnMouseDown()
        {
            
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

    public enum StatusEffect
    {
        None = 0,
    }
}
