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
        float startHeight;
        float updatedHeight;

        private void Awake()
        {
            startHeight = rectTransform.sizeDelta.y;
            StartCoroutine(UpdateGlossary());
        }

        private void Update()
        {
            
        }


        IEnumerator UpdateGlossary()
        {
            statusEffectContainer.Clear();
            updatedHeight = 0;

            foreach (Transform t in gameObject.transform)
            {
                updatedHeight += t.gameObject.GetComponent<RectTransform>().sizeDelta.y;
            }

            Vector2 newSize = new Vector2(0, Mathf.Clamp(rectTransform.sizeDelta.y, startHeight, updatedHeight));
            rectTransform.sizeDelta = newSize;

            yield return null;
        }
    }
}
