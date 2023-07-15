using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CPUTimer : MonoBehaviour
{
    public float CPUCountdownTime;
    public static bool CPUcountdownTimerIsRunning;
    private Text CPUTimerText;

    void Start()
    {
        CPUCountdownTime = 45f;
        CPUTimerText = gameObject.GetComponent<Text>();
        CPUcountdownTimerIsRunning = false;
        CountdownTimerScript.DisplayTime(CPUCountdownTime, CPUTimerText);
    }

    void Update()
    {
        if(CPUCountdownTime > 0 && CPUcountdownTimerIsRunning)
        {
            CountdownTimerScript.DisplayTime(CPUCountdownTime, CPUTimerText);
            CPUCountdownTime -= Time.deltaTime;
        }

        if(CPUCountdownTime <= 0){CPUTimerText.text = "00:00:0";}
    }
}
