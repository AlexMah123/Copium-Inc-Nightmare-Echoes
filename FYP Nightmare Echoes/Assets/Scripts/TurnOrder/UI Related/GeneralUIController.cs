using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NightmareEchoes.TurnOrder
{
    public class GeneralUIController : MonoBehaviour
    {
        public static GeneralUIController Instance;

        [Header("Pause + Settings")]
        public Button pauseButton;
        [SerializeField] GameObject pausePanel;
        [SerializeField] GameObject settingPanel;
        [NonSerialized] public static bool gameIsPaused = false;

        [Space(20), Header("Resolution")]
        Resolution[] Resolutions;
        [SerializeField] private Dropdown _resDropDown;


        [Space(20), Header("Guide")]
        [SerializeField] GameObject guidePanel;
        public Button guideButton;

        [Header("Game Over")]
        [SerializeField] GameObject gameOverPanel;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        void Start()
        {
            /*Resolutions = Screen.resolutions;

            _resDropDown.ClearOptions();

            List<string> ResOptions = new List<string>();
            for (int i = 0; i < Resolutions.Length; i++)
            {
                string resOption = Resolutions[i].width + " X " + Resolutions[i].height;
                ResOptions.Add(resOption);
            }

            _resDropDown.AddOptions(ResOptions);*/
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void PauseGame(bool state)
        {
            if (state)
            {
                gameIsPaused = state;
                Time.timeScale = 0;
            }
            else
            {
                gameIsPaused = state;
                Time.timeScale = 1;
            }
        }

        #region Button Functions
        public void PauseButton()
        {
            if (!pausePanel.activeSelf)
            {
                PauseGame(true);
                pausePanel.SetActive(true);
            }
            else
            {
                PauseGame(false);
                pausePanel.SetActive(false);
            }
        }

        public void SettingsButton()
        {
            if (!settingPanel.activeSelf)
            {
                pausePanel.SetActive(false);
                pauseButton.gameObject.SetActive(false);
                guideButton.gameObject.SetActive(false);

                settingPanel.SetActive(true);
            }
            else
            {
                pausePanel.SetActive(true);
                pauseButton.gameObject.SetActive(true);
                guideButton.gameObject.SetActive(true);

                settingPanel.SetActive(false);
            }
        }

        public void GuideButton()
        {
            if (!guidePanel.activeSelf)
            {
                if (!pausePanel.activeSelf)
                {
                    PauseGame(true);
                    pausePanel.SetActive(true);
                }

                guideButton.gameObject.SetActive(false);
                pauseButton.gameObject.SetActive(false);

                guidePanel.SetActive(true);
            }
            else
            {
                if(!pausePanel.activeSelf)
                {
                    PauseGame(false);
                    pausePanel.SetActive(false);
                }

                guideButton.gameObject.SetActive(true);
                pauseButton.gameObject.SetActive(true);

                guidePanel.SetActive(false);
            }
        }

        public void MainMenuButton(int sceneIndex)
        {
            PauseGame(false);
            gameOverPanel.SetActive(false);

            SceneManager.LoadScene(sceneIndex);
        }

        public void GameOver()
        {
            PauseGame(true);
            pauseButton.gameObject.SetActive(false);
            guideButton.gameObject.SetActive(false);
            gameOverPanel.SetActive(true);
        }
        #endregion



    }
}
