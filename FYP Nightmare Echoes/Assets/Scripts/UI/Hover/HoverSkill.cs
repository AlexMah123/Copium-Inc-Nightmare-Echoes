using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using NightmareEchoes.TurnOrder;

//created by Alex
namespace NightmareEchoes.UI
{
    public class HoverSkill : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        string toolTip;
        [SerializeField] Button button;
        [SerializeField] float timeToWait = 0.5f;

        void OnDisable()
        {
            StopAllCoroutines();

            if(ToolTipManager.OnMouseLoseFocusSkill != null)
            {
                ToolTipManager.OnMouseLoseFocusSkill();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            StopAllCoroutines();
            StartCoroutine(StartTimer());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StopAllCoroutines();
            ToolTipManager.OnMouseLoseFocusSkill();
        }

        private void ShowSkillToolTip()
        {
            if(TurnOrderController.Instance.CurrentUnit == null)
            {
                return;
            }

            switch(button)
            {
                case Button.Basic:
                    if (string.IsNullOrEmpty(TurnOrderController.Instance.CurrentUnit.BasicAttackSkill.Description))
                        return;

                    toolTip = TurnOrderController.Instance.CurrentUnit.BasicAttackSkill.Description;
                    break;

                case Button.Skill1:
                    if (string.IsNullOrEmpty(TurnOrderController.Instance.CurrentUnit.Skill1Skill.Description))
                        return;

                    toolTip = TurnOrderController.Instance.CurrentUnit.Skill1Skill.Description;
                    break;

                case Button.Skill2:
                    if (string.IsNullOrEmpty(TurnOrderController.Instance.CurrentUnit.Skill2Skill.Description))
                        return;

                    toolTip = TurnOrderController.Instance.CurrentUnit.Skill2Skill.Description;
                    break;

                case Button.Skill3:
                    if (string.IsNullOrEmpty(TurnOrderController.Instance.CurrentUnit.Skill3Skill.Description))
                        return;

                    toolTip = TurnOrderController.Instance.CurrentUnit.Skill3Skill.Description;
                    break;

                case Button.Passive:
                    if (string.IsNullOrEmpty(TurnOrderController.Instance.CurrentUnit.PassiveSkill.Description))
                        return;

                    toolTip = TurnOrderController.Instance.CurrentUnit.PassiveSkill.Description;
                    break;
            }

            ToolTipManager.OnMouseHoverSkill(toolTip, Input.mousePosition);
        }

        private IEnumerator StartTimer()
        {
            yield return new WaitForSeconds(timeToWait);
            ShowSkillToolTip();
        }

        private enum Button
        {
            Basic = 0,
            Skill1 = 1,
            Skill2 = 2,
            Skill3 = 3,
            Passive = 4,
        }
    }
}
