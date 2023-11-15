using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using NightmareEchoes.Sound;
using TMPro;
using NightmareEchoes.Inputs;
using NightmareEchoes.Unit.Pathfinding;

namespace NightmareEchoes.TurnOrder
{
    public class GeneralUIController : MonoBehaviour
    {
        public static GeneralUIController Instance;

        [Header("Game Over Related")]
        [SerializeField] GameObject gameOverPanel;

        [Header("Pause + Settings UI")]
        public Button pauseButton;
        [SerializeField] GameObject pausePanel;
        [SerializeField] GameObject settingPanel;
        [NonSerialized] public static bool gameIsPaused = false;

        [Space(20), Header("Guide")]
        [SerializeField] GameObject guidePanel;
        public Button guideButton;

        [Space(20), Header("In Game Settings")]
        [SerializeField] Slider combatSpeedSlider;
        //public static float combatSpeed;
        [SerializeField] Toggle autoCenterToggle;
        //public static bool autoCenter;
        [SerializeField] Toggle runInBGToggle;

        [Space(20), Header("Screen/UI Settings")]
        [SerializeField] TMP_Dropdown resDropDown;
        [SerializeField] TMP_Dropdown screenDropDown;
        [SerializeField] Toggle showHotKeyToggle;

        [SerializeField] List<CanvasGroup> uiCanvasList = new List<CanvasGroup>();
        [SerializeField] Slider uiTransparencySlider;
        static FullScreenMode screenMode = FullScreenMode.ExclusiveFullScreen;





        Resolution[] Resolutions;

        

        

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
            LoadSettings();
            /*Resolutions = Screen.resolutions;

            resDropDown.ClearOptions();

            List<string> ResOptions = new List<string>();
            for (int i = 0; i < Resolutions.Length; i++)
            {
                string resOption = Resolutions[i].width + " X " + Resolutions[i].height;
                ResOptions.Add(resOption);
            }

            resDropDown.AddOptions(ResOptions);*/
        }

        void Update()
        {

        }

        public void LoadSettings()
        {
            AudioManager.Instance.DefaultSoundSetting();

            SetCombatSpeed(1f); //1x speed
            SetAutoCenterUnit(true);
            SetRunInBG(false);

            SetResolution(2); // 1920 x 1080
            SetScreenMode(0); // Fullscreen
            SetShowHotKeys(true);
            SetUITransparency(1f); //1x transparency
        }

        public void SaveSettings()
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

        #region In Game Settings
        public void SetCombatSpeed(float speed)
        {
            PathfindingManager.combatSpeed = speed;
            combatSpeedSlider.value = speed;
        }

        public void SetAutoCenterUnit(bool state)
        {
            CameraControl.autoCenter = state;
            autoCenterToggle.isOn = state;

            if(state)
            {
                if(TurnOrderController.Instance.CurrentUnit)
                {
                    CameraControl.Instance.UpdateCameraPan(TurnOrderController.Instance.CurrentUnit.gameObject);
                }
            }
        }

        public void SetRunInBG(bool state)
        {
            Application.runInBackground = state;
            runInBGToggle.isOn = state;
        }
        #endregion

        #region Screen/UI Settings 
        public void SetResolution(int val)
        {
            switch(val)
            {
                case 0:
                    Screen.SetResolution(1280, 720, screenMode, 60);
                    break;

                case 1:
                    Screen.SetResolution(1680, 1050, screenMode, 60);
                    break;

                case 2:
                    Screen.SetResolution(1920, 1080, screenMode, 60);
                    break;
            }

            resDropDown.value = val;
        }

        public void SetScreenMode(int val)
        {
            switch (val)
            {
                case 0:
                    screenMode = FullScreenMode.ExclusiveFullScreen;
                    break;

                case 1:
                    screenMode = FullScreenMode.FullScreenWindow;
                    break;

                case 2:
                    screenMode = FullScreenMode.Windowed;
                    break;
            }

            Screen.fullScreenMode = screenMode;
            screenDropDown.value = val;
        }

        public void SetShowHotKeys(bool state)
        {

            showHotKeyToggle.isOn = state;
        }

        public void SetUITransparency(float transparency)
        {
            for(int i = 0; i < uiCanvasList.Count; i++)
            {
                uiCanvasList[i].alpha = transparency;
            }

            uiTransparencySlider.value = transparency;
        }

        #endregion

        #region General Button Functions
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
