using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownTimerScript : MonoBehaviour
{
    public float currentCountdownTime = 150f;
    public static bool countdownTimerIsRunning;
    private int PlayerInt;
    private AudioSource TimerAudio;
    private bool countDownSound;

    private Text cTimerText;

    void Start()
    {
        countDownSound = false;
        TimerAudio = gameObject.GetComponent<AudioSource>();
        cTimerText = gameObject.GetComponent<Text>();
        countdownTimerIsRunning = false;
        DisplayTime(currentCountdownTime, cTimerText);
    }
    void Update()
    {

        if(currentCountdownTime > 0 && countdownTimerIsRunning)
        {
            currentCountdownTime -= Time.deltaTime;
            DisplayTime(currentCountdownTime, cTimerText);
        }

        if(currentCountdownTime <= 0){cTimerText.text = "00:00:0"; countdownTimerIsRunning = false;}

        if (SelectTargetNumber.scoreAddFlag && UIFunctions.equationCounter <= 5)
        {
            AddTime(7);
        }

        else if (SelectTargetNumber.scoreAddFlag && UIFunctions.equationCounter < 7)
        {
            AddTime(5);
        }

        else if (SelectTargetNumber.scoreAddFlag && UIFunctions.equationCounter >= 9)
        {
            AddTime(3);
        }

        if (currentCountdownTime <= 10 && !countDownSound)
        {
            TimerAudio.Play();
            countDownSound = true;
        }

        if (currentCountdownTime > 10 || !countdownTimerIsRunning)
        {
            TimerAudio.Stop();
            countDownSound = false;
        }
    }

    void AddTime(int timeAdded)
    {
        currentCountdownTime += timeAdded;
    }

    public static void DisplayTime(float timeToDisplay, Text timerCounter)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        float milliSeconds = (timeToDisplay % 1) * 1000;

        timerCounter.text = string.Format("{0:00}:{1:00}:{2:0}", minutes, seconds, milliSeconds);
    }
}
