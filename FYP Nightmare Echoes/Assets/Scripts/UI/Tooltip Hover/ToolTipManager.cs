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


        [Header("Status Effect Tool Tip")]
        [SerializeField] GameObject currentUnitPanel;
        [SerializeField] GameObject inspectedUnitPanel;
        public static Action<int, Vector2, Transform> OnMouseHoverStatusEffect;
        public static Action OnMouseLoseFocusStatusEffect;
        [SerializeField] Modifier hoveredStatusEffect;


        private void OnEnable()
        {
            OnMouseHoverSkill += DisplayToolTip;
            OnMouseLoseFocusSkill += HideToolTip;

            OnMouseHoverTurnOrder += DisplayTurnOrderToolTip;
            OnMouseLoseFocusTurnOrder += HideTurnOrderToolTip;

            OnMouseHoverStatusEffect += DisplayStatusEffectTooltip;
            OnMouseLoseFocusStatusEffect += HideStatusEffectTooltip;
        }

        private void OnDisable()
        {
            OnMouseHoverSkill -= DisplayToolTip;
            OnMouseLoseFocusSkill -= HideToolTip;

            OnMouseHoverTurnOrder -= DisplayTurnOrderToolTip;
            OnMouseLoseFocusTurnOrder -= HideTurnOrderToolTip;

            OnMouseHoverStatusEffect -= DisplayStatusEffectTooltip;
            OnMouseLoseFocusStatusEffect -= HideStatusEffectTooltip;
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

        #region Skill Tooltip
        private void DisplayToolTip(string tipText, Vector2 mousePos)
        {
            tooltipText.text = tipText;

            tooltipText.ForceMeshUpdate();
            tooltipWindow.sizeDelta = tooltipText.GetPreferredValues();

            tooltipWindow.gameObject.SetActive(true);

            Vector2 newPosition = mousePos + new Vector2(10, tooltipWindow.sizeDelta.y * 0.5f); // Add an offset for spacing

            Vector2 tooltipPositionRelativeToCenter = new Vector2(
                newPosition.x - tooltipWindow.sizeDelta.x * 0.5f,
                newPosition.y - tooltipWindow.sizeDelta.y * 0.5f
            );

            // Adjust horizontal position if tooltip is going off the screen
            if (tooltipPositionRelativeToCenter.x < 0)
            {
                newPosition.x = tooltipWindow.sizeDelta.x * 0.5f;
            }
            else if (tooltipPositionRelativeToCenter.x + tooltipWindow.sizeDelta.x > Screen.width)
            {
                newPosition.x = Screen.width - tooltipWindow.sizeDelta.x * 0.5f;
            }

            // Adjust vertical position if tooltip is going off the screen
            if (tooltipPositionRelativeToCenter.y < 0)
            {
                newPosition.y = tooltipWindow.sizeDelta.y * 0.5f;
            }
            else if (tooltipPositionRelativeToCenter.y + tooltipWindow.sizeDelta.y > Screen.height)
            {
                newPosition.y = Screen.height - tooltipWindow.sizeDelta.y * 0.5f;
            }

            tooltipWindow.transform.position = newPosition;
        }

        private void HideToolTip()
        {
            tooltipText.text = default;

            if(tooltipWindow != null)
            {
                tooltipWindow.gameObject.SetActive(false);
            }
        }
        #endregion


        #region Turn Order Tooltip
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


                //enable inspected unit UI
                GameUIManager.Instance.inspectedUnit = TurnOrderController.Instance.CurrentUnitQueue.ToArray()[order];
                GameUIManager.Instance.EnableInspectedUI(true);

                CameraControl.Instance.UpdateCameraPan(hoveredTurnOrderUnit);
            }
            else
            {
                GameUIManager.Instance.EnableInspectedUI(false);
                isHoveringTurnOrderUnit = false;
                hoverIndicator.SetActive(false);
            }
        }

        private void HideTurnOrderToolTip()
        {
            if(TurnOrderController.Instance.CurrentUnit != null)
            {
                CameraControl.Instance.UpdateCameraPan(TurnOrderController.Instance.CurrentUnit.gameObject);
            }
            isHoveringTurnOrderUnit = false;
            hoverIndicator.SetActive(false);
        }

        #endregion


        #region Status Effect Tooltip
        private void DisplayStatusEffectTooltip(int order, Vector2 mousePos, Transform panel)
        {
            if (panel.gameObject == currentUnitPanel)
            {
                hoveredStatusEffect = GameUIManager.Instance.currentUnitTotalStatusEffectList[order];
            }
            else if(panel.gameObject == inspectedUnitPanel)
            {
                hoveredStatusEffect = GameUIManager.Instance.inspectedUnitTotalStatusEffectList[order];

            }


            if (hoveredStatusEffect != null)
            {
                tooltipText.text = hoveredStatusEffect.name;

                tooltipText.ForceMeshUpdate();
                tooltipWindow.sizeDelta = tooltipText.GetPreferredValues();

                tooltipWindow.gameObject.SetActive(true);
                tooltipWindow.transform.position = new Vector2(mousePos.x + tooltipWindow.sizeDelta.x / 4, mousePos.y + tooltipWindow.sizeDelta.y);
            }
        }

        private void HideStatusEffectTooltip()
        {
            tooltipText.text = default;
            tooltipWindow.gameObject.SetActive(false);
        }

        #endregion
    }
}
