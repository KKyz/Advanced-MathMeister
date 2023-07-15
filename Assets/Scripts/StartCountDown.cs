using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartCountDown : MonoBehaviour
{
    public Text countDownText;
    public static float timeRemaining;
    public GameObject CountdownFade;

    void Awake()
    {
        timeRemaining = 3f;
        gameObject.SetActive(true);
        BlockOff();
    }
    void Update()
    {
        if (timeRemaining > 0)
        {
            BlockOff();
            timeRemaining -= Time.deltaTime;
            DisplayTime(timeRemaining);
        }
        else
        {
            timeRemaining = 0;
            CountdownTimerScript.countdownTimerIsRunning = true;
            CountdownFade.GetComponent<Animator>().Play("SemiFadeOut");
            gameObject.SetActive(false);
            SwipeBlocks.canBeSelected = true;
            
            if (SceneManager.GetActiveScene().name == "Level4")
            {CPUCounter.canAct = true;}
        }
    }

    public void BlockOff()
    {
       CountdownFade.GetComponent<Animator>().Play("SemiFadeIn");   
       CountdownTimerScript.countdownTimerIsRunning = false;
       SwipeBlocks.canBeSelected = false;

       if (SceneManager.GetActiveScene().name == "Level4")
        {CPUCounter.canAct = false;} 
    }
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        countDownText.text =  "" + (int)timeToDisplay;
    }
}
