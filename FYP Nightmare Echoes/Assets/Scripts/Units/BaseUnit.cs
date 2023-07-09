using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public abstract class BaseUnit : MonoBehaviour
    {
        [SerializeField] GameObject _object;
        [SerializeField] string _name;
        [SerializeField] int _health;
        [SerializeField] int _speed;
        [SerializeField] int _energy;

        #region Class Properties
        public GameObject Object
        {
            get => _object;
            set => _object = value;
        }

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
        #endregion

        public abstract void Attack();

        public abstract void Passive();

        public abstract void Skill1();

        public abstract void Skill2();

        public abstract void Skill3();

    }
}
