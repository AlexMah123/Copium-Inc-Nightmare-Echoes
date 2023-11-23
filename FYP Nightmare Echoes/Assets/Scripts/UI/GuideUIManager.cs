using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NightmareEchoes.UI
{
    public class GuideUIManager : MonoBehaviour
    {
        [SerializeField] List<CanvasGroup> guidePanels = new();
        [SerializeField] Button nextGuideButton;
        [SerializeField] Button prevGuideButton;
        int currentGuideIndex;

        private void OnEnable()
        {
            for (int i = 0; i < guidePanels.Count; i++)
            {
                guidePanels[i].gameObject.SetActive(false);
            }

            currentGuideIndex = 0;
            guidePanels[currentGuideIndex].gameObject.SetActive(true);
            nextGuideButton.interactable = true;
            prevGuideButton.interactable = true;

            UpdateButtonState();
        }

        public void NextGuideButton()
        {
            //disable the current index
            guidePanels[currentGuideIndex].gameObject.SetActive(false);

            currentGuideIndex++;

            //enable the index after update
            guidePanels[currentGuideIndex].gameObject.SetActive(true);

            UpdateButtonState();
        }

        public void PrevGuideButton()
        {
            //disable the current index
            guidePanels[currentGuideIndex].gameObject.SetActive(false);

            currentGuideIndex--;

            //enable the index after update
            guidePanels[currentGuideIndex].gameObject.SetActive(true);


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
            if (currentGuideIndex >= guidePanels.Count - 1 && nextGuideButton.gameObject.activeSelf && nextGuideButton.interactable)
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
