using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//created by Alex
namespace NightmareEchoes.Unit
{
    [CreateAssetMenu(fileName = "Insert Unit Name", menuName = "Unit/Unit SO")]
    public class BaseUnitScriptable : ScriptableObject
    {
        [Header("Current Stats")]
        [SerializeField] Sprite _sprite;
        [SerializeField] string _name;
        [SerializeField] int _health;
        [SerializeField] int _speed;
        [SerializeField] int _energy;
        [SerializeField] EnemyType _type;

        [Header("Default Stats")]
        [SerializeField] int defaultHealth;
        [SerializeField] int defaultSpeed;
        [SerializeField] int defautEnergy;

        #region Class Properties
        public Sprite Sprite
        {
            get => _sprite;
            private set => _sprite = value;
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

        [ContextMenu("Reset Data")]
        public void ResetData()
        {
            Health = defaultHealth;
            Speed = defaultSpeed;
            Energy = defautEnergy;
        }

        public enum EnemyType
        {
            None = 0,
            Melee = 1,
            Range = 2,
            Support = 3
        }
    }

    
}
