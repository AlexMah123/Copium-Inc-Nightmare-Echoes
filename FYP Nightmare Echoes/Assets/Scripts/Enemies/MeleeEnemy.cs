using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.Enemy
{
    public class MeleeEnemy : MonoBehaviour
    {
        [SerializeField] BaseEnemy meleeEnemy;

        private void OnEnable()
        {
            meleeEnemy.Object = gameObject;
        }

        void Start()
        {
            Debug.Log(meleeEnemy.Name);
        }

        void Update()
        {
            meleeEnemy.ResetData();
        }
    }
}
