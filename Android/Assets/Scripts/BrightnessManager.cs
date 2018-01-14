using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrightnessManager : MonoBehaviour
{

    [SerializeField] private Text textBrightness;
    [SerializeField] private Slider brightnessSlider;

    //Change the object type to be correct for the brightness
    //GameObject brightness;


    void Start()
    {
        //Here the GameType has to be changed too!
        //brightness = GetComponent<GameObject>();
        brightnessSlider.value = PlayerPrefs.GetFloat("SliderBrightness");
        textBrightness.text = (brightnessSlider.value * 100f).ToString();

        //has to use correct subthing not sure which one so add this too. 
        //brightness.value = PlayerPrefs.GetFloat("SliderBrightness", brightness.value);
    }

    // brightness will have a value -0.5f to 0.5f
    public void SaveSliderValue()
    {
        textBrightness.text = (brightnessSlider.value * 100f).ToString();
        PlayerPrefs.SetFloat("SliderBrightness", brightnessSlider.value);
    }
}
