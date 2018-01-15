using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolManager : MonoBehaviour 
{

    [SerializeField] private Text text;
    [SerializeField] private Slider volSlider;

    AudioSource audioSource;


    void Start()
    {
        //audioSource = GetComponent<AudioSource>();
        volSlider.value = PlayerPrefs.GetFloat("SliderVolume", AudioListener.volume);
        text.text = (volSlider.value * 100f).ToString();

        // get the float value of SliderVolumeLevel if it has been saved with PlayerPrefs.SetFloat()
        // else use defult value of audioSource.volume
        AudioListener.volume = volSlider.value;
    }

    // AudioListener.volume will have a value 0.0f to 1.0f
    public void SaveSliderValue()
    {
        text.text = (volSlider.value * 100f).ToString();
        PlayerPrefs.SetFloat("SliderVolume", volSlider.value);
        AudioListener.volume = volSlider.value;
    }
}
