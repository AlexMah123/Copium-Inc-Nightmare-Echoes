using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//created by Alex
namespace NightmareEchoes.UI
{
    public class FloatingText : MonoBehaviour
    {
        public float destroyTime = 3f;
        public GameObject spawnedFrom;
        public Vector3 offset;

        void Start()
        {
            Destroy(gameObject, destroyTime);
        }

        private void Update()
        {
            if(spawnedFrom != null)
            {
                transform.position = spawnedFrom.transform.position + offset;
            }
        }

    }
}
