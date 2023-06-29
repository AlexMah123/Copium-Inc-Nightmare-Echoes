using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.Hero
{
    [CreateAssetMenu(fileName = "Insert Hero Name", menuName = "Hero/Hero SO")]
    public class BaseHero : ScriptableObject
    {
        [SerializeField] GameObject _object;
        [SerializeField] string _name;
        [SerializeField] int _health;
        [SerializeField] int _speed;
        [SerializeField] int _energy;
        [SerializeField] EnemyType _type;

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

        public EnemyType Type
        {
            get => _type;
            set => _type = value;
        }
        #endregion

        public void ResetData()
        {
            Object = null;
            Health = _health;
            Speed = _speed;
            Energy = _energy;
            Type = _type;
        }
    }

    public enum EnemyType
    {
        Melee = 1,
        Range = 2,
        Support = 3
    }
}
