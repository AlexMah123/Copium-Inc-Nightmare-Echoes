using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//created by Alex
namespace NightmareEchoes.UI
{
    public class HoverTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [field : TextArea] [SerializeField] string toolTip;
        [SerializeField] float timeToWait = 0.5f; 

        public void OnPointerEnter(PointerEventData eventData)
        {
            StopAllCoroutines();
            StartCoroutine(StartTimer());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StopAllCoroutines();
            ToolTipManager.OnMouseLoseFocus();
        }

        private void ShowTip()
        {
            ToolTipManager.OnMouseHover(toolTip, Input.mousePosition);
        }

        private IEnumerator StartTimer()
        {
            yield return new WaitForSeconds(timeToWait);
            ShowTip();
        }
    }
}
