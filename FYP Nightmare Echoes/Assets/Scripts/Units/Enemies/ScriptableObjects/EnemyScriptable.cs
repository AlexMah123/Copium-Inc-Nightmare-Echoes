using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.Unit.Enemy
{
    [CreateAssetMenu(fileName = "Insert Enemy Name", menuName = "Enemy/Enemy SO")]
    public class EnemyScriptable : ScriptableObject
    {
        [Header("Current Stats")]
        [SerializeField] GameObject _object;
        [SerializeField] string _name;
        [SerializeField] int _health;
        [SerializeField] int _speed;
        [SerializeField] int _energy;
        [SerializeField] EnemyType _type;

        [Header("Defaut Stats")]
        [SerializeField] int defautHealth;
        [SerializeField] int defaultSpeed;
        [SerializeField] int defautEnergy;

        #region Class Properties
        public GameObject Object
        {
            get => _object;
            private set => _object = value;
        }

        public string Name
        {
            get => _name;
            private set => _name = value;
        }

        public int Health
        {
            get => _health;
            private set => _health = value;
        }

        public int Speed
        {
            get => _speed;
            private set => _speed = value;
        }

        public int Energy
        {
            get => _energy;
            private set => _energy = value;
        }

        public EnemyType Type
        {
            get => _type;
            private set => _type = value;
        }
        #endregion

        public void ResetData()
        {
            Object = null;
            Health = defautHealth;
            Speed = defaultSpeed;
            Energy = defautEnergy;
        }

        public enum EnemyType
        {
            Melee = 1,
            Range = 2,
            Support = 3
        }
    }

    
}
