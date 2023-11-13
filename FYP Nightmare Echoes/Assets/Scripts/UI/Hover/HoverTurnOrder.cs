using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NightmareEchoes.UI
{
    public class HoverTurnOrder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        int order;
        [SerializeField] float timeToWait = 0.1f;
        [SerializeField] float timeToHide = 0.2f;

        public void OnPointerEnter(PointerEventData eventData)
        {
            StopAllCoroutines();
            StartCoroutine(ShowTurnOrderTip());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StopAllCoroutines();
            StartCoroutine(HideTurnOrderTip());
        }

        IEnumerator ShowTurnOrderTip()
        {
            yield return new WaitForSeconds(timeToWait);

            order = gameObject.transform.GetSiblingIndex() - 1;
            ToolTipManager.OnMouseHoverTurnOrder(order, Input.mousePosition);
        }


        IEnumerator HideTurnOrderTip()
        {
            yield return new WaitForSeconds(timeToWait);
            ToolTipManager.OnMouseLoseFocusTurnOrder();
        }


    }
}
