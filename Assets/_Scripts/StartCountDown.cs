using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartCountDown : MonoBehaviour
{
    public float timeRemaining;
    public Text countDownText;
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameObject.Find("GameInterface").GetComponent<GameManager>();
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
            gameManager.countdownTimerIsRunning = true;
            StartCoroutine(Fade.SemiFadeOut());
            gameObject.SetActive(false);
            gameManager.canBeSelected = true;
        }
    }

    private void BlockOff()
    { 
        gameManager.countdownTimerIsRunning = false; 
        gameManager.canBeSelected = false;
    }
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        countDownText.text =  "" + (int)timeToDisplay;
    }
}
