using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(Animator))]
    public abstract class BaseUnit : MonoBehaviour
    {
        [Header("Unit Info")]
        [SerializeField] protected BaseUnitScriptable _unitScriptable;
        [SerializeField] protected string _name;
        [SerializeField] protected int _health;
        [SerializeField] protected int _speed;
        [SerializeField] protected bool _isHostile;
        [SerializeField] protected Direction _direction;
        [SerializeField] protected StatusEffect _statusEffect;

        [Tooltip("Sprites are ordered in north, south, east, west")]
        [SerializeField] List<Sprite> sprites = new List<Sprite>(); //ordered in NSEW

        Animator animator;
        SpriteRenderer spriteRenderer;

        #region Class Properties
        public BaseUnitScriptable UnitScriptable
        {
            get => _unitScriptable;
            private set => _unitScriptable = value;
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

        public int Speed
        {
            get => _speed;
            set => _speed = value;
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

        #endregion

        protected virtual void Awake()
        {
            _direction = Direction.North;
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {
            switch (_direction)
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

        public abstract void BasicAttack();

        public abstract void Passive();

        public abstract void Skill1();

        public abstract void Skill2();

        public abstract void Skill3();

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
