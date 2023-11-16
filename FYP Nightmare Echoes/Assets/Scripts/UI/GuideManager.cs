using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NightmareEchoes.UI
{
    public class GuideManager : MonoBehaviour
    {
        [SerializeField] List<Sprite> guideList = new List<Sprite>();
        [SerializeField] Image currentGuide;
        [SerializeField] Button nextGuideButton;
        [SerializeField] Button prevGuideButton;

        int currentGuideIndex;

        private void OnEnable()
        {
            currentGuide.sprite = guideList[0];
            currentGuideIndex = 0;

            //disable the interaction for prev button
            prevGuideButton.interactable = false;
            nextGuideButton.interactable = true;
        }

        public void NextGuideButton()
        {
            if (guideList.Count - 1 > currentGuideIndex)
            {
                currentGuideIndex++;
                currentGuide.sprite = guideList[currentGuideIndex];
            }

            UpdateButtonState();

        }

        public void PrevGuideButton()
        {
            if (currentGuideIndex > 0)
            {
                currentGuideIndex--;
                currentGuide.sprite = guideList[currentGuideIndex];
            }

            UpdateButtonState();
        }

        void UpdateButtonState()
        {
            //if the index is 0, disable the prev button
            if (currentGuideIndex == 0 && prevGuideButton.gameObject.activeSelf && prevGuideButton.interactable)
            {
                prevGuideButton.interactable = false;
            }
            else
            {
                //enable otherwise
                prevGuideButton.interactable = true;
            }

            //if the index is the child count - 1, disable the next button
            if (currentGuideIndex >= guideList.Count - 1 && nextGuideButton.gameObject.activeSelf && nextGuideButton.interactable)
            {
                nextGuideButton.interactable = false;
            }
            else
            {
                //enable otherwise
                nextGuideButton.interactable = true;
            }
        }
    }
}
