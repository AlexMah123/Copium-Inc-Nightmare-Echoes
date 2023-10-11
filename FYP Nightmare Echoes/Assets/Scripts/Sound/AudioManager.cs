using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace NightmareEchoes.Sound
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;

        public Sound[] musicSounds;
        public Sound[] sfxSounds;
        public AudioSource musicSource, sfxSource;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
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
        }

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
    }
}
