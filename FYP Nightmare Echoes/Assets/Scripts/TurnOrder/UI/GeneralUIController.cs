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
using NightmareEchoes.Unit;
using System.Linq;
using NightmareEchoes.Scene;

namespace NightmareEchoes.TurnOrder
{
    public class GeneralUIController : MonoBehaviour
    {
        public static GeneralUIController Instance;

        Entity CurrentUnit { get => TurnOrderController.Instance != null ? TurnOrderController.Instance.CurrentUnit : null; }

        [Header("Game Over + Win Related")]
        [SerializeField] GameObject gameOverPanel;
        [SerializeField] GameObject winPanel;

        [Header("Pause + Settings UI")]
        public Button pauseButton;
        [SerializeField] GameObject pausePanel;
        [SerializeField] GameObject settingPanel;
        public static bool gameIsPaused = false;

        [Space(20), Header("Guide Related")]
        public GameObject guidePanel;
        public Button guideButton;

        [Space(20), Header("In Game Settings")]
        [SerializeField] Slider combatSpeedSlider;
        [SerializeField] TextMeshProUGUI combatSpeedText;

        public static float originalEnemyDelay;
        public static float originalPhaseDelay;
        public static float originalPassTurnDelay;

        [SerializeField] Toggle autoCenterToggle;
        [SerializeField] Toggle runInBGToggle;


        [Space(20), Header("Screen/UI Settings")]
        [SerializeField] TMP_Dropdown resDropDown;
        [SerializeField] TMP_Dropdown screenDropDown;

        [Space(20)]
        [SerializeField] List<CanvasGroup> hotkeyCanvasList = new List<CanvasGroup>();
        [SerializeField] Toggle showHotKeyToggle;

        [Space(20)]
        [SerializeField] List<CanvasGroup> uiCanvasList = new List<CanvasGroup>();
        [SerializeField] Slider uiTransparencySlider;
        [SerializeField] TextMeshProUGUI uiTransparencyText;
        static FullScreenMode screenMode = FullScreenMode.ExclusiveFullScreen;    

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
            if(TurnOrderController.Instance)
            {
                originalPhaseDelay = TurnOrderController.Instance.phaseDelay;
                originalEnemyDelay = TurnOrderController.Instance.enemythinkingDelay;
                originalPassTurnDelay = TurnOrderController.Instance.passTurnDelay;
            }
            

            LoadSettings();

            //just a bandaid to disable
            if(guideButton)
                guideButton.gameObject.SetActive(false);
        }


        public void LoadSettings()
        {
            AudioManager.Instance.LoadSoundSettings();
            
            SetCombatSpeed(PlayerPrefs.GetFloat("CombatSpeed", 1f)); //1x speed
            SetAutoCenterUnit(IntToBool(PlayerPrefs.GetInt("AutoCenterUnit", 1)));
            SetRunInBG(IntToBool(PlayerPrefs.GetInt("RunInBG", 0)));

            SetResolution(PlayerPrefs.GetInt("Resolution", 2)); // 1920 x 1080
            SetScreenMode(PlayerPrefs.GetInt("ScreenMode", 0)); // Fullscreen
            SetShowHotKeys(IntToBool(PlayerPrefs.GetInt("ShowHotKey", 1)));
            SetUITransparency(PlayerPrefs.GetFloat("UITransparency", 1f)); //1x transparency

            PlayerPrefs.Save();
        }

        public void SaveSettings()
        {
            AudioManager.Instance.SaveSoundSettings();

            PlayerPrefs.SetFloat("CombatSpeed", combatSpeedSlider.value);
            PlayerPrefs.SetInt("AutoCenterUnit", BoolToInt(autoCenterToggle.isOn));
            PlayerPrefs.SetInt("RunInBG", BoolToInt(runInBGToggle.isOn));

            PlayerPrefs.SetInt("Resolution", resDropDown.value);
            PlayerPrefs.SetInt("ScreenMode", screenDropDown.value);
            PlayerPrefs.SetInt("ShowHotKey", BoolToInt(showHotKeyToggle.isOn));
            PlayerPrefs.SetFloat("UITransparency", uiTransparencySlider.value);

            PlayerPrefs.Save();
        }

        int BoolToInt(bool val)
        {
            return val ? 1 : 0;
        }

        bool IntToBool(int val)
        {
            return val != 0;
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
            if(PathfindingManager.Instance != null)
                PathfindingManager.combatSpeed = speed;

            if(TurnOrderController.Instance != null)
            {
                TurnOrderController.Instance.phaseDelay = originalPhaseDelay / speed;
                TurnOrderController.Instance.enemythinkingDelay = originalEnemyDelay / speed;
                TurnOrderController.Instance.passTurnDelay = originalPassTurnDelay / speed;

                var totalHero = TurnOrderController.Instance.FindAllHeros();
                var totalEnemies = TurnOrderController.Instance.FindAllEnemies();

                List<Entity> allEntity = totalHero.Union(totalEnemies).ToList();

                for (int i = 0; i < allEntity.Count; i++)
                {
                    if (allEntity[i].FrontAnimator && allEntity[i].BackAnimator)
                    {
                        allEntity[i].FrontAnimator.speed = speed;
                        allEntity[i].BackAnimator.speed = speed;
                    }
                }
            }
            

            combatSpeedSlider.value = speed;
            combatSpeedText.text = $"{Mathf.Round(speed * 100f) / 100f}x";
        }

        public void SetAutoCenterUnit(bool state)
        {
            CameraControl.autoCenter = state;
            autoCenterToggle.isOn = state;

            if(state)
            {
                if (CurrentUnit != null)
                {
                    CameraControl.Instance.UpdateCameraPan(CurrentUnit.gameObject);
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
            Application.targetFrameRate = 60;

            switch (val)
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
            for (int i = 0; i < hotkeyCanvasList.Count; i++)
            {
                hotkeyCanvasList[i].alpha = state ? 1 : 0;
            }

            showHotKeyToggle.isOn = state;
        }

        public void SetUITransparency(float transparency)
        {
            for(int i = 0; i < uiCanvasList.Count; i++)
            {
                uiCanvasList[i].alpha = transparency;
            }

            uiTransparencySlider.value = transparency;
            uiTransparencyText.text = $"{Mathf.Round(transparency * 100f)}%";
        }

        #endregion

        #region General Button Functions
        public void PauseButton()
        {
            bool isPaused = pausePanel.activeSelf;

            PauseGame(!isPaused);
            pausePanel.SetActive(!isPaused);
        }

        public void SettingsButton()
        {
            if (!settingPanel.activeSelf)
            {
                if(SceneManager.GetActiveScene().buildIndex != (int)SCENEINDEX.TITLE_SCENE)
                {
                    pausePanel.SetActive(false);
                    pauseButton.gameObject.SetActive(false);
                    guideButton.gameObject.SetActive(false);
                }
                
                settingPanel.SetActive(true);
            }
            else
            {
                if (SceneManager.GetActiveScene().buildIndex != (int)SCENEINDEX.TITLE_SCENE)
                {
                    pausePanel.SetActive(true);
                    pauseButton.gameObject.SetActive(true);
                    //guideButton.gameObject.SetActive(true);
                }
                settingPanel.SetActive(false);
            }
        }

        public void GuideButton()
        {
            bool isGuideActive = guidePanel.activeSelf;

            if (!pausePanel.activeSelf)
            {
                PauseGame(!isGuideActive);
                pausePanel.SetActive(!isGuideActive);
            }
            
            //guideButton.gameObject.SetActive(isGuideActive);
            pauseButton.gameObject.SetActive(isGuideActive);
            guidePanel.SetActive(!isGuideActive);
        }

        public void MainMenuButton(int sceneIndex)
        {
            PauseGame(false);
            gameOverPanel.SetActive(false);
            winPanel.SetActive(false);

            LevelManager.Instance.LoadScene(sceneIndex);
        }

        public void GameOver()
        {
            PauseGame(true);
            pauseButton.gameObject.SetActive(false);
            guideButton.gameObject.SetActive(false);
            gameOverPanel.SetActive(true);
        }

        public void GameVictory()
        {
            PauseGame(true);
            pauseButton.gameObject.SetActive(false);
            guideButton.gameObject.SetActive(false);
            winPanel.SetActive(true);
        }
        #endregion



    }
}
