using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace NightmareEchoes.UI
{
    public class ToolTipManager : MonoBehaviour
    {
        [SerializeField] RectTransform tooltipWindow;
        [SerializeField] TextMeshProUGUI tooltipText;


        public static Action<string, Vector2> OnMouseHover;
        public static Action OnMouseLoseFocus;

        private void OnEnable()
        {
            OnMouseHover += DisplayToolTip;
            OnMouseLoseFocus += HideToolTip;
        }

        private void OnDisable()
        {
            OnMouseHover -= DisplayToolTip;
            OnMouseLoseFocus -= HideToolTip;
        }

        private void Start()
        {
            HideToolTip();
        }

        private void DisplayToolTip(string tipText, Vector2 mousePos)
        {
            tooltipText.text = tipText;
            tooltipWindow.sizeDelta = new Vector2(tooltipText.preferredWidth, tooltipText.preferredHeight);

            tooltipWindow.gameObject.SetActive(true);
            tooltipWindow.transform.position = new Vector2(mousePos.x + tooltipText.preferredWidth / 4, mousePos.y + tooltipWindow.sizeDelta.y * 2);
        }

        private void HideToolTip()
        {
            tooltipText.text = default;
            tooltipWindow.gameObject.SetActive(false);
        }
    }
}
