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

        public void MasterVolume()
        {
            AudioManager.instance.MasterVolume(_MasterSlider.value);

            if (_MasterSlider.value == 0)
            {
                _MusicSlider.value = 0;
                _SFXSlider.value = 0;
            }
        }

        public void MusicVolume()
        {
            AudioManager.instance.MusicVolume(_MusicSlider.value);

            _MusicSlider.value = _MasterSlider.value;

            if (_MasterSlider.value == 0)
            {
                AudioManager.instance.MusicVolume(0);
            }
        }

        public void SfxVolume()
        {
            AudioManager.instance.SFXVolume(_SFXSlider.value);

            _SFXSlider.value = _MasterSlider.value;

            if (_MasterSlider.value == 0)
            {
                _SFXSlider.value = 0;
            }
        }
    }
}
