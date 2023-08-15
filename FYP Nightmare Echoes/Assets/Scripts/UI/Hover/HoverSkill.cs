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
                    if (string.IsNullOrEmpty(TurnOrderController.Instance.CurrentUnit.BasicAttackDesc))
                        return;

                    toolTip = TurnOrderController.Instance.CurrentUnit.BasicAttackDesc;
                    break;

                case Button.Skill1:
                    if (string.IsNullOrEmpty(TurnOrderController.Instance.CurrentUnit.Skill1Desc))
                        return;

                    toolTip = TurnOrderController.Instance.CurrentUnit.Skill1Desc;
                    break;

                case Button.Skill2:
                    if (string.IsNullOrEmpty(TurnOrderController.Instance.CurrentUnit.Skill2Desc))
                        return;

                    toolTip = TurnOrderController.Instance.CurrentUnit.Skill2Desc;
                    break;

                case Button.Skill3:
                    if (string.IsNullOrEmpty(TurnOrderController.Instance.CurrentUnit.Skill3Desc))
                        return;

                    toolTip = TurnOrderController.Instance.CurrentUnit.Skill3Desc;
                    break;

                case Button.Passive:
                    if (string.IsNullOrEmpty(TurnOrderController.Instance.CurrentUnit.PassiveDesc))
                        return;

                    toolTip = TurnOrderController.Instance.CurrentUnit.PassiveDesc;
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
