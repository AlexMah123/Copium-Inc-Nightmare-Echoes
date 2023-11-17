using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;


namespace NightmareEchoes.Sound
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [Header("Sound List")]
        public Sound[] musicSounds;
        public Sound[] sfxSounds;

        [Header("Audio Sources")]
        public AudioSource musicSource, sfxSource;
        public AudioMixer audioMixer;

        [Header("Sound Sliders")]
        public Slider masterSlider, musicSlider, sfxSlider;
        [SerializeField] TextMeshProUGUI masterVolumeText;
        [SerializeField] TextMeshProUGUI sfxVolumeText;
        [SerializeField] TextMeshProUGUI musicVolumeText;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            { 
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            PlayMusic("BgSoundTest");

            musicSource.volume = 0.5f;
            sfxSource.volume = 0.5f;
        }

        public void SaveSoundSettings()
        {
            PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
            PlayerPrefs.SetFloat("SoundVolume", sfxSlider.value);
            PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);

            PlayerPrefs.Save();
        }

        public void LoadSoundSettings()
        {

            SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume", -40f));
            SetSFXVolume(PlayerPrefs.GetFloat("SoundVolume", 0));
            SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 0));
        }

        public void DefaultSoundSetting()
        {
            PlayMusic("BgSoundTest");

            //audio is from -80 to 0 db
            SetMasterVolume(-40f);
            SetSFXVolume(0);
            SetMusicVolume(0);
        }

        #region public functions to call for sound
        public void PlayMusic(string Name)
        {
            Sound Musicsound = Array.Find(musicSounds, x => x.Name == Name);

            if (Musicsound == null)
            {
                Debug.Log("No Music available");
            }
            else
            {
                musicSource.clip = Musicsound.clip;
                musicSource.Play();
            }
        }

        public void PlaySFX(string Name)
        {
            Sound SFXSound = Array.Find(sfxSounds, x => x.Name == Name);

            if (SFXSound == null)
            {
                Debug.Log("No SFX available");
            }
            else
            {
                sfxSource.PlayOneShot(SFXSound.clip);
            }
        }
        #endregion

        #region Slider Functions
        public void SetMasterVolume(float volume)
        {
            float ratio = 1 - Mathf.Abs(volume / 80f);
            masterVolumeText.text = string.Format("{0:P0}", ratio);

            audioMixer.SetFloat("MasterVolume", volume);
            masterSlider.value = volume;

        }

        public void SetSFXVolume(float volume)
        {
            float ratio = 1 - Mathf.Abs(volume / 80f);
            sfxVolumeText.text = string.Format("{0:P0}", ratio);

            audioMixer.SetFloat("SoundVolume", volume);
            sfxSlider.value = volume;
        }

        public void SetMusicVolume(float volume)
        {
            float ratio = 1 - Mathf.Abs(volume / 80f);
            musicVolumeText.text = string.Format("{0:P0}", ratio);

            audioMixer.SetFloat("MusicVolume", volume);
            musicSlider.value = volume;

        }
        #endregion

    }
}
