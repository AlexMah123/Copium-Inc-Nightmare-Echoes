using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    [RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public abstract class BaseUnit : MonoBehaviour
    {
        [Header("Unit Info")]
        [SerializeField] protected string _name;
        [SerializeField] protected int _health;
        [SerializeField] protected int _speed;
        [SerializeField] protected int _energy;
        [SerializeField] protected bool _isHostile;
        [SerializeField] protected Direction direction = Direction.North;

        [Tooltip("Sprites are ordered in north, south, east, west")]
        [SerializeField] List<Sprite> sprites = new List<Sprite>(); //ordered in NSEW

        Animator animator;
        SpriteRenderer spriteRenderer;

        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();    
            spriteRenderer = GetComponent<SpriteRenderer>();    
        }

        protected virtual void Start()
        {
            
        }

        protected virtual void Update()
        {
            switch(direction)
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

        #region Class Properties
        public string Name
        {
            get => _name;
            set => _name = value;
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

        public int Energy
        {
            get => _energy;
            set => _energy = value;
        }

        public bool IsHostile
        {
            get => _isHostile;
            private set => IsHostile = value;
        }
        #endregion

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


        public enum Direction
        {
            North = 0,
            South = 1,
            East = 2,
            West = 3,
        }
    }
}
