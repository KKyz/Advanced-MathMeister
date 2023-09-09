﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsFunctions : MonoBehaviour
{
    private GameObject AlertMessage, BGToggle, BGMSlider, SFXSlider;
    public AudioMixer BGMMixer, SFXMixer;
    private int BGPref;

    void Start()
    {
        AlertMessage = GameObject.Find("AlertMessage");
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
        AlertMessage.GetComponent<Animator>().Play("AlertMessageEnter");
    }

    public void DismissAlert()
    {
        AlertMessage.GetComponent<Animator>().Play("AlertMessageExit");
    }

    public void DisableBG(bool isActive)
    {
        if (BGToggle.GetComponent<Toggle>().isOn == true)
        {
            PlayerPrefs.SetInt("BGActive", 1);
            isActive = false;
        }
        else if (BGToggle.GetComponent<Toggle>().isOn == false)
        {
            PlayerPrefs.SetInt("BGActive", 0);
            isActive = true; 
        }
    }
    public void EraseAll()
    {
        PlayerPrefs.DeleteAll();
    }
}