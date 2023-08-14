using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NightmareEchoes.UI
{
    public class HoverTurnOrder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
            ToolTipManager.OnMouseLoseFocusTurnOrder();
        }

        IEnumerator StartTimer()
        {
            yield return new WaitForSeconds(timeToWait);
            ShowTurnOrderTip();
        }

        private void ShowTurnOrderTip()
        {
            order = gameObject.transform.GetSiblingIndex() - 1;
            ToolTipManager.OnMouseHoverTurnOrder(order, Input.mousePosition);
        }

        
    }
}
