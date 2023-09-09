using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")] 
    public int minTarget;
    public int maxTarget;
    public GameMode gameMode;
    public float currentCountdownTime = 150f;

    [Header("Game Objects")]
    public GameObject StartCountDownText;
    public GameObject ChangeParticle;
    public GameObject sparkleParticle; 
    public GameObject explosionParticle;
   
    [Header("Game Audio")] 
    public AudioClip equateSound;
    public AudioClip shuffleSound;
    public AudioClip StartCountdownSound;
    public AudioClip FinalCountdownSound;
    public AudioClip VictoryBGM;
    public AudioClip LevelBGM;
    public AudioClip FailBGM;
    public AudioClip GoalChange;

    [HideInInspector] public bool equationResetFlag, scoreAddFlag, triggeredVictory, playerSwitch, countdownTimerIsRunning;
    [HideInInspector] public int equationCounter, trashCounter, goalInt, playerNumber, currentPlayerInt;

    private bool canShuffle, countDownSoundActive;
    private AudioSource SFXSource;
    private Button shuffleButton, equateButton, trashButton;
    private Text playerCounter, goalText, trashCountText, timerText;
    private ColorBlock shuffleBlock, equateBlock;
    private SpawnBlocks spawnBlocks;
    private EquateScore equateScore;
    private SwipeBlocks swipeBlocks;
    private StartCountDown startCountDown;
    private AudioSource BGMSource;
    private float bestTime, musicPos, CPCountdownTime, timeLimit, CountDownTimer;
    private int CPUTrashBest, CPUTrashLevel, CPUScoreInt, Level2Best, CPCounter, CPUTimeLevel, CPUTimeBest;
    private GameObject changeParticle, CPUTrashLevelText, CPUWaitFade, resultsCanvas;

    public enum GameMode
    {
        TimeTrial,
        Level1,
        Level2,
        Level3,
        Level4,
        Level5
    }
    
    void Awake()
    {
        var spawner = GameObject.Find("GameInterface").transform.Find("Spawner");
        equateScore = spawner.GetComponent<EquateScore>();
        spawnBlocks = spawner.GetComponent<SpawnBlocks>();
        
        equationCounter = 0;
        trashCounter = 0;
        shuffleButton = transform.Find("ButtonCanvas").Find("ShuffleButton").GetComponent<Button>();
        equateButton = transform.Find("ButtonCanvas").Find("EquateButton").GetComponent<Button>();
        resultsCanvas = transform.Find("BGCanvas").Find("ResultsScreen").gameObject;
        
        if (gameMode == GameMode.Level4)
        {
            trashButton = transform.Find("ButtonCanvas").Find("TrashButton").GetComponent<Button>();
            //trashCounterText = transform.parent.Find("LabelCanvas").Find("TrashCounter").GetComponent<Text>();
        }
        
        playerCounter = transform.Find("LabelCanvas").Find("YourNumberCounter").GetComponent<Text>();
        shuffleBlock = shuffleButton.GetComponent<Button>().colors;
        equateBlock = equateButton.GetComponent<Button>().colors;
        equationResetFlag = false;
        canShuffle = true;
        
        goalInt = Random.Range(minTarget, maxTarget);
        
        scoreAddFlag = false;
        triggeredVictory = false;
        goalText = transform.Find("LabelCanvas").Find("GoalNumberCounter").GetComponent<Text>();
        goalText.text = goalInt.ToString();
        
        SFXSource = gameObject.GetComponent<AudioSource>();
        BGMSource = transform.Find("BGMSource").GetComponent<AudioSource>();
        
        timeLimit = 45f;
        musicPos = 0f;
        StartCoroutine(PlayBGMSource());
        playerSwitch = true;
        CPUTimeLevel = 1;
        CPUTrashLevel = 1;
        CPUTimeBest = PlayerPrefs.GetInt("Level3", 5);
        CPUTrashBest = PlayerPrefs.GetInt("Level4", 4);
        Level2Best = PlayerPrefs.GetInt("Level2", 7);

        spawnBlocks.canBeSelected = false;
        
        countDownSoundActive = false;
        timerText = transform.Find("LabelCanvas").Find("TimeCounter").GetComponent<Text>();
        countdownTimerIsRunning = false;
        DisplayTime(currentCountdownTime, timerText);
    }

    public void ShuffleBlocks(float shuffleTime)
    {
        if (canShuffle)
        {
            equationResetFlag = true;
            spawnBlocks.RemoveBlock(1, false);

            spawnBlocks.SetUp(5, 4);
            StartCoroutine(RefreshShuffle(shuffleTime));

            SFXSource.PlayOneShot(shuffleSound, 0.7f);
        }
    }

    IEnumerator RefreshShuffle(float secs)
    {
        canShuffle = false;
        shuffleBlock.selectedColor = new Color(0.68f, 0.68f, 0.68f, 1);
        shuffleBlock.pressedColor = new Color(0.68f, 0.68f, 0.68f, 1);
        shuffleBlock.normalColor = new Color(0.68f, 0.68f, 0.68f, 1);
        shuffleButton.GetComponent<Button>().colors = shuffleBlock;
        shuffleButton.GetComponent<Animator>().Play("ShuffleSpinOut");
        yield return new WaitForSeconds(secs);
        shuffleBlock.selectedColor = new Color(1, 0, 0, 1);
        shuffleBlock.pressedColor = new Color(1, 0, 0, 1);
        shuffleBlock.normalColor = new Color(1, 0, 0, 1);
        shuffleButton.GetComponent<Button>().colors = shuffleBlock;
        shuffleButton.GetComponent<Animator>().Play("ShuffleSpinIn");
        canShuffle = true;
    }

    public void EquateCurrent()
    {
        if (equateScore.output != 0 && equateScore.calcs.Count != 1)
        {
            if (SwitchSubtractionAddition.AdditionMode){playerNumber += equateScore.output;}
            if (!SwitchSubtractionAddition.AdditionMode){playerNumber -= equateScore.output;}

            spawnBlocks.RemoveBlock(1, true);

            equationCounter += 1;
            equationResetFlag = true;

            SFXSource.PlayOneShot(equateSound, 0.7f);
            SwitchSubtractionAddition.haveGasped = false;
            var thisSparkleParticle = Instantiate(sparkleParticle, playerCounter.gameObject.transform.position, Quaternion.identity);
            Destroy(thisSparkleParticle, 2f);
        }
    }

    public void ThrowTrash()
    {
        if (trashCounter != 0)
        {
            var CPU = transform.Find("LabelCanvas").Find("CPU").GetComponent<CPU>();
            CPU.CPUInt += trashCounter;

            var explosion = Instantiate(explosionParticle, GameObject.Find("CPUCounter").transform.position, Quaternion.identity);
            trashButton.GetComponent<Animator>().Play("TrashSpinIn");
            Destroy(explosion, 2f);
            trashCounter = 0;
        }
    }

    
    public void NextLevel()
    {
        var CPU = transform.Find("LabelCanvas").Find("CPU").GetComponent<CPU>();
        CPU.CPULevelUp();
        //startCountdown.timeRemaining = 3f;

        for (int i = 0; i <= resultsCanvas.transform.childCount - 1; i++)
        {
            resultsCanvas.transform.GetChild(i).gameObject.SetActive(false);
        }
        resultsCanvas.SetActive(false);
        StartCountDownText.SetActive(true);
        
        timeLimit = 45f;
        timerText.text = "00:45:0";
        currentCountdownTime = timeLimit;
        
        if (SceneManager.GetActiveScene().name == "Level3")
        {
            CPU.CPUTimerText.text = "00:45:0";
            CPU.CPUCountdownTime = timeLimit;
            CPU.CPUcountdownTimerIsRunning = false;
            playerSwitch = true;
        }

        if (SceneManager.GetActiveScene().name == "Level4")
        {
            CPU.UpdateCPUGoal(minTarget, maxTarget);
        }

        playerNumber = 0;
        CPU.CPUInt = 0;
        goalInt = Random.Range(minTarget, maxTarget);
        goalText.text = goalInt.ToString();

        StartCoroutine(PlayBGMSource());
        triggeredVictory = false;
    }

    private IEnumerator DisplayResults()
    {
        if (gameMode == GameMode.Level3)
        {
            CPUWaitFade.GetComponent<Animator>().Play("CPUWaitFadeWait");
        }
        triggeredVictory = true;
        spawnBlocks.canBeSelected = false;

        BGMSource.Pause();
        musicPos = BGMSource.time;
        BGMSource.loop = false;
        BGMSource.time = 0f;
        BGMSource.Play();

        resultsCanvas.SetActive(true);
        for (int i = 0; i <= resultsCanvas.transform.childCount - 1; i++)
        {
            resultsCanvas.transform.GetChild(i).gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public IEnumerator PlayBGMSource()
    {
        BGMSource.loop = false;
        BGMSource.clip = StartCountdownSound;
        BGMSource.time = 0f;
        BGMSource.Play();
        yield return new WaitForSeconds(4f);
        BGMSource.loop = true;
        BGMSource.clip = LevelBGM;
        BGMSource.time = musicPos;
        BGMSource.Play();
    }
    
    void AddTime(int timeAdded)
    {
        currentCountdownTime += timeAdded;
    }

    public void DisplayTime(float timeToDisplay, Text textToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        float milliSeconds = (timeToDisplay % 1) * 1000;

        textToDisplay.text = string.Format("{0:00}:{1:00}:{2:0}", minutes, seconds, milliSeconds);
    }
    
    public void Update()
    {
        playerCounter.text = playerNumber.ToString();
        
        if(currentCountdownTime > 0 && countdownTimerIsRunning)
        {
            currentCountdownTime -= Time.deltaTime;
            DisplayTime(currentCountdownTime, timerText);
        }

        if (currentCountdownTime <= 0)
        {
            timerText.text = "00:00:0"; countdownTimerIsRunning = false;
        }

        if (scoreAddFlag && equationCounter <= 5)
        {
            AddTime(7);
        }

        else if (scoreAddFlag && equationCounter < 7)
        {
            AddTime(5);
        }

        else if (scoreAddFlag && equationCounter >= 9)
        {
            AddTime(3);
        }

        if (currentCountdownTime <= 10 && !countDownSoundActive)
        {
            SFXSource.PlayOneShot(FinalCountdownSound);
            countDownSoundActive = true;
        }

        if (currentCountdownTime > 10 || !countdownTimerIsRunning)
        {
            SFXSource.Stop();
            countDownSoundActive = false;
        }

        if (SceneManager.GetActiveScene().name == "Level4")
        {
            if (trashCounter != 0)
            {
                trashButton.GetComponent<Animator>().Play("TrashSpinIn");
                trashCountText.text = trashCounter.ToString();
            }

            if (trashButton.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("TrashSpinIn") && trashCounter == 0)
            {
                trashButton.GetComponent<Animator>().Play("TrashSpinOut");
                trashCountText.text = "0";
            }
        }

        if (equateScore.output != 0)
        {
            if (equateScore.calcs.Count == 3 || equateScore.calcs.Count == 5 || equateScore.calcs.Count == 7)
            {
                equateBlock.selectedColor = new Color(0, 1, 0, 1);
                equateBlock.pressedColor = new Color(0, 0.32f, 0.078f, 1);
                equateBlock.normalColor = new Color(0, 1, 0, 1); 
                equateButton.enabled = true;
                equateButton.GetComponent<Animator>().Play("EquateSpinIn");
            }
            
        }
        else
        {
            equateBlock.selectedColor = new Color(0.68f, 0.68f, 0.68f, 1);
            equateBlock.pressedColor = new Color(0.68f, 0.68f, 0.68f, 1);
            equateBlock.normalColor = new Color(0.68f, 0.68f, 0.68f, 1);
            equateButton.enabled = false;

            if (equateButton.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("EquateSpinIn"))
            {
                equateButton.GetComponent<Animator>().Play("EquateSpinOut");
            }
        }

        if (scoreAddFlag)
        {
            equationCounter = 0;
        }
        
        //Pasted from GameManager, should clean up soon
        currentPlayerInt = playerNumber;

        //Time Trial Mode
        if (gameMode == GameMode.TimeTrial)
        {
            if (currentPlayerInt == goalInt)
            {
                scoreAddFlag = true;
                SFXSource.PlayOneShot(GoalChange, 0.5f);
                changeParticle = Instantiate(ChangeParticle, gameObject.transform.position, Quaternion.identity);
                changeParticle.transform.SetParent(null);
                Destroy(changeParticle, 2f);
                goalInt = Random.Range(minTarget, maxTarget);
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
                BGMSource.clip = VictoryBGM;
                StartCoroutine(DisplayResults());
            }

            
        }

        //Unknown Goal Mode
        if (gameMode == GameMode.Level1)
        {
            if (currentPlayerInt == goalInt)
            {
                scoreAddFlag = true;
                SFXSource.PlayOneShot(GoalChange, 0.5f);
                changeParticle = Instantiate(ChangeParticle, gameObject.transform.position, Quaternion.identity);
                changeParticle.transform.SetParent(null);
                Destroy(changeParticle, 2f);
                goalInt = Random.Range(minTarget, maxTarget);
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
                BGMSource.clip = VictoryBGM;
                StartCoroutine(DisplayResults());
            }

            
        }

        //Move Counter Mode
        if (gameMode == GameMode.Level2)
        {
            if (currentPlayerInt == goalInt)
            {
                CountMoves.moveCounter += 3;
                CountMoves.levelCounter += 1;
                SFXSource.PlayOneShot(GoalChange, 0.5f);
                changeParticle = Instantiate(ChangeParticle, gameObject.transform.position, Quaternion.identity);
                changeParticle.transform.SetParent(null);
                Destroy(changeParticle, 2f);
                goalInt = Random.Range(minTarget, maxTarget);
                goalText.text = goalInt.ToString();
            }

            if (CountMoves.moveCounter <= 0 && !triggeredVictory)
            {
                BGMSource.clip = VictoryBGM;
                StartCoroutine(DisplayResults());
                
                if (CountMoves.levelCounter > Level2Best)
                {PlayerPrefs.SetInt("Level2", CountMoves.levelCounter);}
            }
        }

        //CPU Time Attack Mode
        if (gameMode == GameMode.Level3)
        {
            var CPU = transform.Find("LabelCanvas").Find("CPU").GetComponent<CPU>();
            CPCountdownTime = CPU.CPUCountdownTime;
            CPCounter = CPU.CPUInt;
            CPUWaitFade = GameObject.Find("CPUWaitFade");

            if (playerSwitch)
            {   
                if (currentPlayerInt == goalInt)
                {
                    CPCountdownTime = timeLimit;
                    SFXSource.PlayOneShot(GoalChange, 0.5f);
                    CPU.UpdateCPUGoal(minTarget, maxTarget);
                    CPU.canAct = true;
                    CPU.CPUcountdownTimerIsRunning = true;

                    countdownTimerIsRunning = false;
                    spawnBlocks.canBeSelected = false;
                    playerSwitch = false;

                    CPUWaitFade.GetComponent<Animator>().Play("CPUWaitFadeIdle");
                }

                if (CountDownTimer <= 0 && !triggeredVictory)
                {
                    CPU.canAct = false;
                    BGMSource.clip = FailBGM;
                    StartCoroutine(DisplayResults());
                }
            }

            if (!playerSwitch)
            {   
                if (CPCounter == CPU.CPUGoal)
                {
                    timeLimit -= 5f;
                    CountDownTimer = timeLimit;
                    
                    countdownTimerIsRunning = true;
                    spawnBlocks.canBeSelected = true;
                    playerSwitch = true;
                    SFXSource.PlayOneShot(GoalChange, 0.5f);
                    changeParticle = Instantiate(ChangeParticle, gameObject.transform.position, Quaternion.identity);
                    changeParticle.transform.SetParent(null);
                    Destroy(changeParticle, 2f);
                    goalInt = Random.Range(minTarget, maxTarget);
                    goalText.text = goalInt.ToString();

                    CPU.canAct = false;
                    CPU.CPUcountdownTimerIsRunning = false;

                    CPUWaitFade.GetComponent<Animator>().Play("CPUWaitFadeExit");
                }

                if (CPCountdownTime <= 0 && !triggeredVictory)
                {
                    CPU.canAct = false;
                    BGMSource.clip = VictoryBGM;
                    StartCoroutine(DisplayResults());
                    CPUTimeLevel += 1;

                    if (CPUTimeLevel > CPUTimeBest)
                    {PlayerPrefs.SetInt("Level3", CPUTimeLevel);}
                }
            }
        }
        
        //CPU Trash Attack Mode
        if (gameMode == GameMode.Level4)
        {
            var CPU = transform.Find("LabelCanvas").Find("CPU").GetComponent<CPU>();
            CPUTrashLevelText = GameObject.Find("LevelCounter");
            CPUScoreInt = CPU.Score;
            CPCounter = CPU.CPUInt;

            CPUTrashLevelText.GetComponent<Text>().text = ("Level "+ CPUTrashLevel.ToString());

            if (currentPlayerInt == goalInt)
            {
                scoreAddFlag = true;
                SFXSource.PlayOneShot(GoalChange, 0.5f);
                goalInt = Random.Range(minTarget, maxTarget);
                goalText.text = goalInt.ToString();
            }

            if (CPCounter == CPU.CPUGoal)
            {
                CPU.CPUScoreAddFlag = true;
                CPU.UpdateCPUGoal(minTarget, maxTarget);
            }

            if (CountDownTimer <= 0 && !triggeredVictory)
            {
                CPU.canAct = false;

                if (CPUScoreInt > ShowScore.playerScore)
                {
                    BGMSource.clip = FailBGM;
                    StartCoroutine(DisplayResults());
                }

                if (ShowScore.playerScore >= CPUScoreInt)
                {
                    BGMSource.clip = FailBGM;
                    StartCoroutine(DisplayResults());
                    
                    CPUTrashLevel += 1;

                    if (CPUTrashLevel > CPUTrashBest)
                    {PlayerPrefs.SetInt("Level4", CPUTrashLevel);}
                }
            }
        }
        
        //First to 1000
        if (gameMode == GameMode.Level5)
        {
            
        }
    }
}
