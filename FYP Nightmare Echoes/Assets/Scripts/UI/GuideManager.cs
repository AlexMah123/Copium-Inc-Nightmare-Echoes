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
        }

        public void Update()
        {
            if(currentGuideIndex == 0 && prevGuideButton.gameObject.activeSelf && prevGuideButton.interactable) 
            {
                prevGuideButton.interactable = false;
            }
            else
            {
                if(!prevGuideButton.interactable)
                {
                    prevGuideButton.interactable = true;
                }
            }

            if (currentGuideIndex == 11 && nextGuideButton.gameObject.activeSelf && nextGuideButton.interactable)
            {
                nextGuideButton.interactable = false;
            }
            else
            {
                if(!nextGuideButton.interactable)
                {
                    nextGuideButton.interactable = true;
                }
            }
        }

        public void NextGuideButton()
        {
            if (guideList.Count - 1 > currentGuideIndex)
            {
                currentGuideIndex++;
                currentGuide.sprite = guideList[currentGuideIndex];
            }
        }

        public void PrevGuideButton()
        {
            if (currentGuideIndex > 0)
            {
                currentGuideIndex--;
                currentGuide.sprite = guideList[currentGuideIndex];
            }
        }
    }
}
