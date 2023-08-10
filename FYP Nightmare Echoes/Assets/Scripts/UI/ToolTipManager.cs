using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NightmareEchoes.TurnOrder;
using NightmareEchoes.Unit;
using NightmareEchoes.Inputs;

//created by Alex
namespace NightmareEchoes.UI
{
    public class ToolTipManager : MonoBehaviour
    {
        [Header("Skill Tool Tip")]
        [SerializeField] RectTransform tooltipWindow;
        [SerializeField] TextMeshProUGUI tooltipText;
        public static Action<string, Vector2> OnMouseHoverSkill;
        public static Action OnMouseLoseFocusSkill;

        [Header("Turn Order Tool Tip")]
        [SerializeField] GameObject hoverIndicator;
        [SerializeField] Color tooltip;
        [SerializeField] private float frequency = 2.0f;
        [SerializeField] private float magnitude = 0.05f;
        [SerializeField] private float offset = 0.75f;
        public static Action<int, Vector2> OnMouseHoverTurnOrder;
        public static Action OnMouseLoseFocusTurnOrder;
        public bool isHoveringTurnOrderUnit;
        GameObject hoveredTurnOrderUnit;

        private void OnEnable()
        {
            OnMouseHoverSkill += DisplayToolTip;
            OnMouseLoseFocusSkill += HideToolTip;

            OnMouseHoverTurnOrder += DisplayTurnOrderToolTip;
            OnMouseLoseFocusTurnOrder += HideTurnOrderToolTip;
        }

        private void OnDisable()
        {
            OnMouseHoverSkill -= DisplayToolTip;
            OnMouseLoseFocusSkill -= HideToolTip;

            OnMouseHoverTurnOrder -= DisplayTurnOrderToolTip;
            OnMouseLoseFocusTurnOrder -= HideTurnOrderToolTip;
        }

        private void Update()
        {
            if (isHoveringTurnOrderUnit) 
            {
                hoverIndicator.transform.position = new Vector3(
                    hoveredTurnOrderUnit.transform.position.x, hoveredTurnOrderUnit.transform.position.y + offset, hoveredTurnOrderUnit.transform.position.z)
                    + transform.up * Mathf.Sin(Time.time * frequency) * magnitude;
            }
        }

        private void Start()
        {
            HideToolTip();
            hoverIndicator.SetActive(false);
        }

        private void DisplayToolTip(string tipText, Vector2 mousePos)
        {
            tooltipText.text = tipText;

            tooltipText.ForceMeshUpdate();
            tooltipWindow.sizeDelta = tooltipText.GetPreferredValues();

            tooltipWindow.gameObject.SetActive(true);
            tooltipWindow.transform.position = new Vector2(mousePos.x + tooltipWindow.sizeDelta.x / 4, mousePos.y + tooltipWindow.sizeDelta.y * 2);
        }

        private void HideToolTip()
        {
            tooltipText.text = default;
            tooltipWindow.gameObject.SetActive(false);
        }

        private void DisplayTurnOrderToolTip(int order, Vector2 mousePos)
        {

            hoveredTurnOrderUnit = TurnOrderController.Instance.CurrentUnitQueue.ToArray()[order].gameObject;

            if (hoveredTurnOrderUnit != null)
            {
                if (!hoverIndicator.activeSelf)
                {
                    hoverIndicator.GetComponent<SpriteRenderer>().color = new Color(tooltip.r, tooltip.g, tooltip.b, tooltip.a);
                    isHoveringTurnOrderUnit = true;
                    hoverIndicator.SetActive(true);
                }

                CameraControl.Instance.UpdateCameraPan(hoveredTurnOrderUnit);
            }
            else
            {
                isHoveringTurnOrderUnit = false;
            }
        }

        private void HideTurnOrderToolTip()
        {
            CameraControl.Instance.UpdateCameraPan(TurnOrderController.Instance.CurrentUnit.gameObject);
            hoverIndicator.SetActive(false);
        }
    }
}
