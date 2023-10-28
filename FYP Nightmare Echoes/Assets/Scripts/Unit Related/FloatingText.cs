using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.UI
{
    public class FloatingText : MonoBehaviour
    {
        public float destroyTime = 3f;
        void Start()
        {
            Destroy(gameObject, destroyTime);
        }

    }
}
