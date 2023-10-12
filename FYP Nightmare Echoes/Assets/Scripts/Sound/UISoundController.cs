using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace NightmareEchoes.Sound
{
    public class UISoundController : MonoBehaviour
    {
        public Slider _MasterSlider, _MusicSlider, _SFXSlider;

        public void MasterVolume()
        {
            AudioManager.instance.MasterVolume(_MasterSlider.value);
        }

        public void MusicVolume()
        {
            AudioManager.instance.MusicVolume(_MusicSlider.value);
        }

        public void SfxVolume()
        {
            AudioManager.instance.SFXVolume(_SFXSlider.value);
        }
    }
}
