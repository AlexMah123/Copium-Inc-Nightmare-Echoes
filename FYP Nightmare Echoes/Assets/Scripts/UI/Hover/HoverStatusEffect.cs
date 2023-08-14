using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NightmareEchoes.UI
{
    public class HoverStatusEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        int order;
        [SerializeField] float timeToWait = 0.2f;


        public void OnPointerEnter(PointerEventData eventData)
        {
            StopAllCoroutines();
            StartCoroutine(StartTimer());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StopAllCoroutines();
            ToolTipManager.OnMouseLoseFocusStatusEffect();
        }

        IEnumerator StartTimer()
        {
            yield return new WaitForSeconds(timeToWait);
            ShowStatusEffectTip();

        }

        private void ShowStatusEffectTip()
        {
            Transform panel = gameObject.transform.parent;
            order = gameObject.transform.GetSiblingIndex();
            ToolTipManager.OnMouseHoverStatusEffect(order, Input.mousePosition, panel);

        }
    }
}
