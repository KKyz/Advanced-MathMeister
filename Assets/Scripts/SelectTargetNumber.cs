using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#pragma warning disable 0414

public class SelectTargetNumber : MonoBehaviour
{
    public static int goalInt;
    private float CountDownTimer;
    public int minTarget, maxTarget;
    [HideInInspector]public int currentPlayerInt;
    private Text goalText;
    public GameObject Canvas, VictoryCanvas, FailCanvas, BGMSource, CountDownObject, CPU, StartCountDownText, ChangeParticle;
    private AudioSource BGM, goalSFX;
    public AudioClip CountdownBGM, VictoryBGM, LevelBGM, FailBGM, GoalChange;
    private float bestTime, musicPos;
    public static bool scoreAddFlag, triggeredVictory;
    public int gameMode;
    private int CPCounter, CPUTimeLevel, CPUTimeBest;
    private int CPUTrashBest, CPUTrashLevel, CPUScoreInt, Level2Best;
    private float CPCountdownTime;
    private GameObject changeParticle, CPUTrashLevelText;
    public bool playerSwitch;
    private float timeLimit;
    private GameObject CPUWaitFade;

    void Awake()
    {
        SetTargetInt(minTarget, maxTarget);
        
        scoreAddFlag = false;
        triggeredVictory = false;
        goalText = gameObject.GetComponent<Text>();
        goalText.text = goalInt.ToString();
        BGM = BGMSource.GetComponent<AudioSource>();

        bestTime = 0;
    }

    void Start()
    {
        goalSFX = gameObject.GetComponent<AudioSource>();
        timeLimit = 45f;
        musicPos = 0f;
        StartCoroutine(PlayBGM());
        playerSwitch = true;
        CPUTimeLevel = 1;
        CPUTrashLevel = 1;
        CPUTimeBest = PlayerPrefs.GetInt("Level3", 5);
        CPUTrashBest = PlayerPrefs.GetInt("Level4", 4);
        Level2Best = PlayerPrefs.GetInt("Level2", 7);

        SwipeBlocks.canBeSelected = false;
    }

    void Update()
    {
        currentPlayerInt = Canvas.GetComponent<UIFunctions>().playerNumber;

        //Time Trial Mode
        if (gameMode == 0)
        {
            CountDownTimer = CountDownObject.GetComponent<CountdownTimerScript>().currentCountdownTime;
            
            if (currentPlayerInt == goalInt)
            {
                scoreAddFlag = true;
                goalSFX.PlayOneShot(GoalChange, 0.5f);
                changeParticle = Instantiate(ChangeParticle, gameObject.transform.position, Quaternion.identity);
                changeParticle.transform.SetParent(null);
                Destroy(changeParticle, 2f);
                SetTargetInt(minTarget, maxTarget);
                goalText.text = goalInt.ToString();
                if (ShowScore.playerScore > (ShowBestScore.displayBestScore - 100))
                {PlayerPrefs.SetInt("TimeTrial", ShowScore.playerScore + 100);}
            }

            else
            {
                scoreAddFlag = false;
            }
            if(CountDownTimer <= 0 && !triggeredVictory)
            {
                StartCoroutine(VictoryAnimation(VictoryCanvas, VictoryBGM));
            }

            
        }

        //Unknown Goal Mode
        if (gameMode == 1)
        {
            CountDownTimer = CountDownObject.GetComponent<CountdownTimerScript>().currentCountdownTime;
            
            if (currentPlayerInt == goalInt)
            {
                scoreAddFlag = true;
                goalSFX.PlayOneShot(GoalChange, 0.5f);
                changeParticle = Instantiate(ChangeParticle, gameObject.transform.position, Quaternion.identity);
                changeParticle.transform.SetParent(null);
                Destroy(changeParticle, 2f);
                SetTargetInt(minTarget, maxTarget);
                goalText.text = goalInt.ToString();
                if (ShowScore.playerScore > (ShowBestScore.displayBestScore - 100))
                {PlayerPrefs.SetInt("Level1", ShowScore.playerScore + 100);}
            }

            else
            {
                scoreAddFlag = false;
            }
            if(CountDownTimer <= 0 && !triggeredVictory)
            {
                StartCoroutine(VictoryAnimation(VictoryCanvas, VictoryBGM));
            }

            
        }

        //Move Counter Mode
        if (gameMode == 2)
        {
            if (currentPlayerInt == goalInt)
            {
                CountMoves.moveCounter += 3;
                CountMoves.levelCounter += 1;
                goalSFX.PlayOneShot(GoalChange, 0.5f);
                changeParticle = Instantiate(ChangeParticle, gameObject.transform.position, Quaternion.identity);
                changeParticle.transform.SetParent(null);
                Destroy(changeParticle, 2f);
                SetTargetInt(minTarget, maxTarget);
                goalText.text = goalInt.ToString();
            }

            if (CountMoves.moveCounter <= 0 && !triggeredVictory)
            {
                StartCoroutine(VictoryAnimation(VictoryCanvas, VictoryBGM));
                
                if (CountMoves.levelCounter > Level2Best)
                {PlayerPrefs.SetInt("Level2", CountMoves.levelCounter);}
            }
        }

        //CPU Time Attack Mode
        if (gameMode == 3)
        {
            CountDownTimer = CountDownObject.GetComponent<CountdownTimerScript>().currentCountdownTime;
            CPCountdownTime = CPU.transform.GetChild(3).GetComponent<CPUTimer>().CPUCountdownTime;
            CPCounter = CPU.transform.GetChild(1).GetComponent<CPUCounter>().CPUInt;
            CPUWaitFade = GameObject.Find("CPUWaitFade");

            if (playerSwitch)
            {   
                if (currentPlayerInt == goalInt)
                {
                    CPCountdownTime = timeLimit;
                    goalSFX.PlayOneShot(GoalChange, 0.5f);
                    CPU.transform.GetChild(5).GetComponent<CPUGoal>().UpdateCPUGoal(minTarget, maxTarget);
                    CPUCounter.canAct = true;
                    CPUTimer.CPUcountdownTimerIsRunning = true;

                    CountdownTimerScript.countdownTimerIsRunning = false;
                    SwipeBlocks.canBeSelected = false;
                    playerSwitch = false;

                    CPUWaitFade.GetComponent<Animator>().Play("CPUWaitFadeIdle");
                }

                if (CountDownTimer <= 0 && !triggeredVictory)
                {
                    CPUCounter.canAct = false;
                    StartCoroutine(VictoryAnimation(FailCanvas, FailBGM));
                }
            }

            if (!playerSwitch)
            {   
                if (CPCounter == CPUGoal.CPGoal)
                {
                    timeLimit -= 5f;
                    CountDownTimer = timeLimit;
                    
                    CountdownTimerScript.countdownTimerIsRunning = true;
                    SwipeBlocks.canBeSelected = true;
                    playerSwitch = true;
                    goalSFX.PlayOneShot(GoalChange, 0.5f);
                    changeParticle = Instantiate(ChangeParticle, gameObject.transform.position, Quaternion.identity);
                    changeParticle.transform.SetParent(null);
                    Destroy(changeParticle, 2f);
                    SetTargetInt(minTarget, maxTarget);
                    goalText.text = goalInt.ToString();

                    CPUCounter.canAct = false;
                    CPUTimer.CPUcountdownTimerIsRunning = false;

                    CPUWaitFade.GetComponent<Animator>().Play("CPUWaitFadeExit");
                }

                if (CPCountdownTime <= 0 && !triggeredVictory)
                {
                    CPUCounter.canAct = false;
                    StartCoroutine(VictoryAnimation(VictoryCanvas, VictoryBGM));
                    CPUTimeLevel += 1;

                    if (CPUTimeLevel > CPUTimeBest)
                    {PlayerPrefs.SetInt("Level3", CPUTimeLevel);}
                }
            }
        }
        
        //CPU Trash Attack Mode
        if (gameMode == 4)
        {
            CPUTrashLevelText = GameObject.Find("LevelCounter");
            CountDownTimer = CountDownObject.GetComponent<CountdownTimerScript>().currentCountdownTime;
            CPUScoreInt = CPU.transform.GetChild(3).GetComponent<CPUScore>().Score;
            CPCounter = CPU.transform.GetChild(1).GetComponent<CPUCounter>().CPUInt;

            CPUTrashLevelText.GetComponent<Text>().text = ("Level "+ CPUTrashLevel.ToString());

            if (currentPlayerInt == goalInt)
            {
                scoreAddFlag = true;
                goalSFX.PlayOneShot(GoalChange, 0.5f);
                SetTargetInt(minTarget, maxTarget);
                goalText.text = goalInt.ToString();
            }

            if (CPCounter == CPUGoal.CPGoal)
            {
                CPUScore.CPUScoreAddFlag = true;
                CPU.transform.GetChild(5).GetComponent<CPUGoal>().UpdateCPUGoal(minTarget, maxTarget);
            }

            if (CountDownTimer <= 0 && !triggeredVictory)
            {
                CPUCounter.canAct = false;

                if (CPUScoreInt > ShowScore.playerScore)
                {StartCoroutine(VictoryAnimation(FailCanvas, FailBGM));}

                if (ShowScore.playerScore >= CPUScoreInt)
                {
                    StartCoroutine(VictoryAnimation(VictoryCanvas, VictoryBGM));
                    CPUTrashLevel += 1;

                    if (CPUTrashLevel > CPUTrashBest)
                    {PlayerPrefs.SetInt("Level4", CPUTrashLevel);}
                }
            }
        }
        
        //First to 1000
        if (gameMode == 5)
        {
            CountDownTimer = CountDownObject.GetComponent<CountdownTimerScript>().currentCountdownTime;
            
        }
    }

    public void SetTargetInt(int minimumInt, int maximumInt)
    {
        goalInt = Random.Range(minimumInt, maximumInt);
    }

    public void NextLevel()
    {
        CPU.transform.GetChild(1).GetComponent<CPUCounter>().CPULevelUp();
        StartCountDown.timeRemaining = 3f;
        
        for (int i = 0; i <= VictoryCanvas.transform.childCount - 1; i++)
        {VictoryCanvas.transform.GetChild(i).gameObject.SetActive(false);}
        VictoryCanvas.SetActive(false);
        StartCountDownText.SetActive(true);
        
        timeLimit = 45f;
        CountDownObject.GetComponent<Text>().text = "00:45:0";
        CountDownObject.GetComponent<CountdownTimerScript>().currentCountdownTime = timeLimit;
        
        if (SceneManager.GetActiveScene().name == "Level3")
        {
            CPU.transform.GetChild(3).GetComponent<Text>().text = "00:45:0";
            CPU.transform.GetChild(3).GetComponent<CPUTimer>().CPUCountdownTime = timeLimit;
            CPUTimer.CPUcountdownTimerIsRunning = false;
            playerSwitch = true;
        }

        if (SceneManager.GetActiveScene().name == "Level4")
        {
            CPU.transform.GetChild(5).GetComponent<CPUGoal>().UpdateCPUGoal(minTarget, maxTarget);
        }

        Canvas.GetComponent<UIFunctions>().playerNumber = 0;
        CPU.transform.GetChild(1).GetComponent<CPUCounter>().CPUInt = 0;
        SetTargetInt(minTarget, maxTarget);
        goalText.text = goalInt.ToString();

        StartCoroutine(PlayBGM());
        triggeredVictory = false;
    }

    IEnumerator VictoryAnimation(GameObject Canvas, AudioClip VictoryTheme)
    {
        if (gameMode == 3){CPUWaitFade.GetComponent<Animator>().Play("CPUWaitFadeWait");}
        triggeredVictory = true;
        SwipeBlocks.canBeSelected = false;

        BGM.Pause();
        musicPos = BGM.time;
        BGM.loop = false;
        BGM.clip = VictoryTheme;
        BGM.time = 0f;
        BGM.Play();

        Canvas.SetActive(true);
        for (int i = 0; i <= Canvas.transform.childCount - 1; i++)
        {
            Canvas.transform.GetChild(i).gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator PlayBGM()
    {
        BGM.loop = false;
        BGM.clip = CountdownBGM;
        BGM.time = 0f;
        BGM.Play();
        yield return new WaitForSeconds(4f);
        BGM.loop = true;
        BGM.clip = LevelBGM;
        BGM.time = musicPos;
        BGM.Play();
    }
}
