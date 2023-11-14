using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
namespace NightmareEchoes.Sound
{
    public class UISoundController : MonoBehaviour
    {
        [Header("Sound")]
        public Slider _MasterSlider, _MusicSlider, _SFXSlider;
        public AudioMixer _AudioMixer;

        /*public void MasterVolume()
        {
            Debug.Log("here");

            AudioManager.instance.SetMasterVolume(_MasterSlider.value);

            if (_MasterSlider.value == 0)
            {
                _MusicSlider.value = 0;
                _SFXSlider.value = 0;
            }
        }

        public void MusicVolume()
        {
            Debug.Log("here");
            AudioManager.instance.SetMusicVolume(_MusicSlider.value);

            _MusicSlider.value = _MasterSlider.value;

            if (_MasterSlider.value == 0)
            {
                AudioManager.instance.SetMusicVolume(0);
            }
        }

        public void SfxVolume()
        {
            Debug.Log("here");

            AudioManager.instance.SetSFXVolume(_SFXSlider.value);

            _SFXSlider.value = _MasterSlider.value;

            if (_MasterSlider.value == 0)
            {
                _SFXSlider.value = 0;
            }
        }*/
    }
}
