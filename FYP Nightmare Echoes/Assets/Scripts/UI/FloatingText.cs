using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.UI
{
    public class FloatingText : MonoBehaviour
    {
        [SerializeField] float destroyTime = 3f;
        void Start()
        {
            Destroy(gameObject, destroyTime);
        }

    }
}
