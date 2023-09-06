using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartCountDown : MonoBehaviour
{
    public float timeRemaining;
    public Text countDownText;
    public SpawnBlocks spawnBlocks;
    public CountdownTimerScript countdownTimer;

    private void Awake()
    {
        var gameInterface = GameObject.Find("GameInterface").transform;
        spawnBlocks = gameInterface.Find("Spawner").GetComponent<SpawnBlocks>();
        countdownTimer = gameInterface.Find("LabelCanvas").Find("TimeCounter").GetComponent<CountdownTimerScript>();
        timeRemaining = 3f;
        countDownText = GetComponent<Text>();
        gameObject.SetActive(true);
        BlockOff();
    }
    private void Update()
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
            countdownTimer.countdownTimerIsRunning = true;
            StartCoroutine(Fade.SemiFadeOut());
            gameObject.SetActive(false);
            spawnBlocks.canBeSelected = true;

            if (SceneManager.GetActiveScene().name == "Level4")
            {
                CPUCounter.canAct = true;
            }
        }
    }

    private void BlockOff()
    { 
        countdownTimer.countdownTimerIsRunning = false;
       spawnBlocks.canBeSelected = false;

       if (SceneManager.GetActiveScene().name == "Level4")
       {
           CPUCounter.canAct = false;
       }
    }
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        countDownText.text =  "" + (int)timeToDisplay;
    }
}
