using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace NightmareEchoes.UI
{
    public class GlossaryResize : MonoBehaviour
    {
        [SerializeField] RectTransform rectTransform;
        List<GameObject> statusEffectContainer = new List<GameObject>();
        Vector2 startSize;

        #region Properties
        public List<GameObject> StatusEffectContainer
        {
            get => statusEffectContainer;
            set
            {
                statusEffectContainer = value;
                UpdateGlossary();
            }
        }
        #endregion

        private void Awake()
        {
            startSize = rectTransform.sizeDelta;
            UpdateGlossary();
        }

        public void UpdateGlossary()
        {
            statusEffectContainer.Clear();
            float updatedHeight = 20;

            foreach (Transform t in gameObject.transform)
            {
                updatedHeight += t.gameObject.GetComponent<RectTransform>().sizeDelta.y + 20;
            }

            Vector2 newSize = new Vector2(startSize.x, updatedHeight);

            if(startSize.y < newSize.y)
            {
                rectTransform.sizeDelta = newSize;
            }
            else
            {
                rectTransform.sizeDelta = startSize;
            }
        }
    }
}
