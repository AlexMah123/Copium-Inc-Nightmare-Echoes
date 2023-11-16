using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NightmareEchoes.TurnOrder
{
    public class TutorialUIManager : MonoBehaviour
    {
        public static TutorialUIManager Instance;
        public static bool inTutorialState = false;

        [Header("Tutorial Related")]
        [SerializeField] GameObject tutorialCanvas;
        [SerializeField] Button tutorialExitButton;
        [SerializeField] Button nextGuideButton;
        [SerializeField] Button prevGuideButton;

        [Space(20)]
        [SerializeField] List<CanvasGroup> tutorialPanels = new List<CanvasGroup>();
        [NonSerialized] public int currentPanelIndex = 0;
        [NonSerialized] public int currentTutorialGuideCap;

        CanvasGroup currentGuideShown;
        int childIndex;

        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }

            inTutorialState = false;
        }

        private void Update() 
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                EnableTutorialCanvas();
            }
        

        }

        public bool InTutorialState()
        {
            //double check checkpoints
            return inTutorialState;
        }

        #region Public Calls

        public IEnumerator StartTutorial()
        {
            inTutorialState = true;

            //the current index for guide to show that is based on a list
            currentPanelIndex = 0;
            currentTutorialGuideCap = 1;
            //yield return new WaitForSecondsRealtime(2f);

            //EnableTutorialCanvas();
            yield break;
        }

        

        public void EnableTutorialCanvas()
        {
            //pause the game
            GeneralUIController.Instance.PauseGame(true);

            //reset all the panels
            for (int i = 0; i < tutorialPanels.Count; i++)
            {
                tutorialPanels[i].gameObject.SetActive(false);
            }

            //enable the canvas
            tutorialCanvas.SetActive(true);

            //check if the current index has exceed
            if (currentPanelIndex >= 0 && currentPanelIndex < tutorialPanels.Count)
            {
                //enable the guide based on the index and cache it.
                tutorialPanels[currentPanelIndex].gameObject.SetActive(true);
                currentGuideShown = tutorialPanels[currentPanelIndex];
            }

            if(currentGuideShown.transform.childCount > 1)
            {
                //enable the first child if there is more than 1
                childIndex = 0;
                currentGuideShown.transform.GetChild(childIndex).gameObject.SetActive(true);

                //enable both buttons
                nextGuideButton.gameObject.SetActive(true);
                prevGuideButton.gameObject.SetActive(true);

                //disable the interaction for prev button
                prevGuideButton.interactable = false;
                nextGuideButton.interactable = true;
            }
            else
            {
                //disable both buttons
                nextGuideButton.gameObject.SetActive(false);
                prevGuideButton.gameObject.SetActive(false);
            }
        }
        #endregion

        #region Button Functions
        public void ExitTutorialButton()
        {
            //increment the next guide to show when exiting
            if(currentPanelIndex < tutorialPanels.Count - 1)
            {
                currentPanelIndex++;
            }
            else
            {
                //reset the guide index
                currentPanelIndex = 0;
            }

            //disable the canvas, unpause the game
            tutorialCanvas.SetActive(false);
            GeneralUIController.Instance.PauseGame(false);
        }

        public void NextTutorialButton()
        {
            //disable the current index
            currentGuideShown.transform.GetChild(childIndex).gameObject.SetActive(false);

            childIndex++;

            //enable the index after update
            currentGuideShown.transform.GetChild(childIndex).gameObject.SetActive(true);

            UpdateButtonState();
        }

        public void PrevTutorialButton()
        {
            //disable the current index
            currentGuideShown.transform.GetChild(childIndex).gameObject.SetActive(false);

            childIndex--;

            //enable the index after update
            currentGuideShown.transform.GetChild(childIndex).gameObject.SetActive(true);


            UpdateButtonState();
        }

        void UpdateButtonState()
        {
            //if the index is 0, disable the prev button
            if (childIndex == 0 && prevGuideButton.gameObject.activeSelf && prevGuideButton.interactable)
            {
                prevGuideButton.interactable = false;
            }
            else
            {
                //enable otherwise
                prevGuideButton.interactable = true;
            }

            //if the index is the child count - 1, disable the next button
            if (childIndex >= currentGuideShown.transform.childCount - 1 && nextGuideButton.gameObject.activeSelf && nextGuideButton.interactable)
            {
                nextGuideButton.interactable = false;
            }
            else
            {
                //enable otherwise
                nextGuideButton.interactable = true;
            }
        }

        #endregion


    }
}
