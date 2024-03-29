﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

//All functions here have been optimized. Nothing more to do here.
public class OptionsFunctions : MonoBehaviour
{
    private GameObject alertMessage, BGToggle, BGMSlider, SFXSlider;
    public AudioMixer BGMMixer, SFXMixer;
    private int BGPref;

    void Start()
    {
        alertMessage = transform.Find("PersonalOptionsHolder").Find("AlertMessage").gameObject;
        BGToggle = GameObject.Find("Toggle");
        BGMSlider = GameObject.Find("BGMSlider");
        SFXSlider = GameObject.Find("SFXSlider");

        BGPref = PlayerPrefs.GetInt("BGActive", 0);
        if (BGPref == 0){BGToggle.GetComponent<Toggle>().isOn = false;}
        if (BGPref == 1){BGToggle.GetComponent<Toggle>().isOn = true;}

        SFXSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("SFXVolPref", 1f);
        BGMSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("BGMVolPref", 1f);
    }

    public void SetBGMVolume (float sliderValue)
    {
        BGMMixer.SetFloat("BGMVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("BGMVolPref", sliderValue);
    }

    public void SetSFXVolume (float sliderValue)
    {
        SFXMixer.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SFXVolPref", sliderValue);
    }

    public void PullUpAlert()
    {
        LeanTween.moveLocalX(alertMessage, 170, 1f).setEaseOutSine();
    }

    public void DismissAlert()
    {
        LeanTween.moveLocalX(alertMessage, 943, 1f).setEaseInSine();
    }

    public void DisableBG()
    {
        if (BGToggle.GetComponent<Toggle>().isOn)
        {
            PlayerPrefs.SetInt("BGActive", 1);
        }
        else
        {
            PlayerPrefs.SetInt("BGActive", 0);
        }
    }
    public void EraseAll()
    {
        SaveSystem.ClearPlayer();
    }
}
