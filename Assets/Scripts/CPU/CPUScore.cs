using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CPUScore : MonoBehaviour
{
    public int Score;
    private Text scoreText;
    private GameObject CPUCounter, Timer;
    private float TimeLeft;
    private int CPUInt;
    public static bool CPUScoreAddFlag;

    // Start is called before the first frame update
    void Start()
    {
        CPUScoreAddFlag = false;
        Timer = GameObject.Find("TimeCounter");
        CPUCounter = GameObject.Find("CPUCounter");
        scoreText = gameObject.GetComponent<Text>();
        Score = 0; 
        scoreText.text = Score.ToString("00000");
    }

    // Update is called once per frame
    void Update()
    {
        CPUInt = CPUCounter.GetComponent<CPUCounter>().CPUInt;
        TimeLeft = Timer.GetComponent<CountdownTimerScript>().currentCountdownTime;

        if (TimeLeft < 30 && CPUInt == CPUGoal.CPGoal)
        {
            CPUAddPoints(100);
        }

        if (TimeLeft >= 30 && TimeLeft < 60 && CPUInt == CPUGoal.CPGoal)
        {
            CPUAddPoints(150);
        }

        if (TimeLeft >= 60 && TimeLeft < 90 && CPUInt == CPUGoal.CPGoal)
        {
            CPUAddPoints(200);
        }

        if (TimeLeft >= 90 && TimeLeft < 120 && CPUInt == CPUGoal.CPGoal)
        {
            CPUAddPoints(250);
        }

        if (TimeLeft >= 120 && CPUInt == CPUGoal.CPGoal)
        {
            CPUAddPoints(350);
        }
    }

    private void CPUAddPoints(int Amount)
    {
        Score += Amount;
        scoreText.text = Score.ToString("00000");
        CPUScoreAddFlag = false;
    }
}
