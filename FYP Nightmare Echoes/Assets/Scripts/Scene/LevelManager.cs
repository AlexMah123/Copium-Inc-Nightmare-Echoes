using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace NightmareEchoes.Scene
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance;

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

        public void LoadScene(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }
}

[Serializable]
public enum SCENEINDEX
{
    TITLE_SCENE = 0,
    TUTORIAL_SCENE = 1,
    GAME_SCENE = 2,
    END_SCENE = 3,
}

