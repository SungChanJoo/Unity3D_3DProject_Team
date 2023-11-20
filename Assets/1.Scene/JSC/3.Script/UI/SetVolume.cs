using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SetVolume : MonoBehaviour
{
    public AudioMixer Mixer;
    public Slider Slider;
    public string ExposeName;
    private void Start()
    {
        Slider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
    }

    public void SetLevel(float sliderVal)
    {
        Mixer.SetFloat(ExposeName, Mathf.Log10(sliderVal) * 20);
        PlayerPrefs.SetFloat("MusicVolume", sliderVal);
    }
}
