using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartCountDown : MonoBehaviour
{
    public float timeRemaining;
    public Text countDownText;
    public SpawnBlocks spawnBlocks;
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameObject.Find("GameInterface").GetComponent<GameManager>();
        spawnBlocks = gameManager.transform.Find("Spawner").GetComponent<SpawnBlocks>();
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
            spawnBlocks.canBeSelected = true;

            if (SceneManager.GetActiveScene().name == "Level4")
            {
                var CPU = transform.Find("LabelCanvas").Find("CPU").GetComponent<CPU>();
                CPU.canAct = true;
            }
        }
    }

    private void BlockOff()
    { 
        gameManager.countdownTimerIsRunning = false;
       spawnBlocks.canBeSelected = false;

       if (SceneManager.GetActiveScene().name == "Level4")
       {
           var CPU = transform.Find("LabelCanvas").Find("CPU").GetComponent<CPU>();
           CPU.canAct = false;
       }
    }
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        countDownText.text =  "" + (int)timeToDisplay;
    }
}
