using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.UI
{
    public class InspectableUIManager : MonoBehaviour
    {
        GameObject temporaryInspectedUnit;

        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void SetObjectInspectable(GameObject obj)
        {
            temporaryInspectedUnit = obj;
        }

        public void SetCurrentUnit()
        {

        }
    }
}
