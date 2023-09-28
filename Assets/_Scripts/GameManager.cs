using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using B83.LogicExpressionParser;
using Random=UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")] 
    public int minTarget;
    public int maxTarget;
    public GameMode gameMode;
    public int playerNumber;
    public float currentCountdownTime = 150f;

    [Header("Game Objects")]
    public GameObject StartCountDownText;
    public GameObject flashObj;
    //public GameObject ChangeParticle;
    //public GameObject sparkleParticle; 
    //public GameObject explosionParticle;
   
    [Header("Game Audio")] 
    public AudioClip equateSound;
    public AudioClip shuffleSound;
    public AudioClip StartCountdownSound;
    public AudioClip FinalCountdownSound;
    public AudioClip VictoryBGM;
    public AudioClip LevelBGM;
    public AudioClip FailBGM;
    public AudioClip GoalChange;
    public AudioClip audienceGaspFar;
    public AudioClip audienceGaspClose;
    
   [HideInInspector] public bool equationResetFlag, scoreAddFlag, countdownTimerIsRunning;
   [HideInInspector] public int trashCounter;

    private bool canShuffle, countDownSoundActive, triggeredVictory, playerSwitch, additionMode, newCurrentTextFading;
    private AudioSource SFXSource;
    private Button shuffleButton, equateButton, trashButton;
    private Text currentCounter, trashCountText, timerText, operationText, equateButtonText, newCurrentCounter, newCurrentBack;
    private ColorBlock shuffleColorBlock, equateColorBlock;
    private Color newCurrentColor, newCurrentBackColor;
    private SpawnBlocks spawnBlocks;
    private SwipeBlocks swipeBlocks;
    private StartCountDown startCountDown;
    private AudioSource BGMSource;
    private float bestTime, musicPos, CPCountdownTime, timeLimit, CountDownTimer;
    private int CPUTrashBest, CPUTrashLevel, CPUScoreInt, Level2Best, CPCounter, CPUTimeLevel, CPUTimeBest, equationCounter;
    private GameObject CPUTrashLevelText, CPUWaitFade, resultsCanvas;

    public static bool haveGasped;
    
    public List<string> calcs = new();
    public int output;
    public string calcsString;
    private Parser parser = new();
    private NumberExpression numExpression;

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
        //getting scripts for necessary game functions (equating items, spawning blocks, etc.)
        var spawner = GameObject.Find("GameInterface").transform.Find("Spawner");
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
        
        shuffleColorBlock = shuffleButton.GetComponent<Button>().colors;
        equateColorBlock = equateButton.GetComponent<Button>().colors;
        equationResetFlag = false;
        canShuffle = true;
        
        playerNumber = Random.Range(minTarget, maxTarget);
        
        scoreAddFlag = false;
        newCurrentTextFading = false;
        triggeredVictory = false;
        currentCounter = transform.Find("LabelCanvas").Find("YourNumberCounter").GetComponent<Text>();
        currentCounter.text = playerNumber.ToString();
        newCurrentCounter = transform.Find("LabelCanvas").Find("CalculatedNumberCounter").GetComponent<Text>();
        newCurrentBack = transform.Find("LabelCanvas").Find("CalculatedNumberCounterBack").GetComponent<Text>();
        newCurrentColor = newCurrentCounter.color;
        newCurrentBackColor = newCurrentBack.color;

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
        
        haveGasped = false;
        equateButtonText = equateButton.transform.Find("EquateText").GetComponent<Text>();
        operationText = transform.Find("ButtonCanvas").Find("SwitchButton").Find("Text").GetComponent<Text>();
        additionMode = true;
        SFXSource.Stop();
        flashObj = GameObject.Find("FlashBG");
        
        //Initialise output and calc string to be 0
        output = 0;
        calcsString = "0+0";

        for (int i = 0; i <= calcs.Count - 1; i++)
        {
            calcs.RemoveAt(i);
        }
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
        shuffleColorBlock.selectedColor = new Color(0.68f, 0.68f, 0.68f, 1);
        shuffleColorBlock.pressedColor = new Color(0.68f, 0.68f, 0.68f, 1);
        shuffleColorBlock.pressedColor = new Color(0.68f, 0.68f, 0.68f, 1);
        shuffleColorBlock.highlightedColor = new Color(0.68f, 0.68f, 0.68f, 1);
        shuffleButton.colors = shuffleColorBlock;
        LeanTween.rotateZ(shuffleButton.gameObject, -180, 0.3f);
        yield return new WaitForSeconds(secs);
        shuffleColorBlock.selectedColor = new Color(1, 0, 0, 1);
        shuffleColorBlock.pressedColor = new Color(1, 0, 0, 1);
        shuffleColorBlock.normalColor = new Color(1, 0, 0, 1);
        shuffleColorBlock.highlightedColor = new Color(1, 0, 0, 1);
        shuffleButton.colors = shuffleColorBlock;
        LeanTween.rotateZ(shuffleButton.gameObject, 0, 0.3f);
        canShuffle = true;
    }

    public void EquateCurrent()
    {
        if (output != 0 && calcs.Count != 1)
        {
            if (additionMode)
            {
                playerNumber += output;
            }

            if (!additionMode)
            {
                playerNumber -= output;
            }

            spawnBlocks.RemoveBlock(1, true);

            equationCounter += 1;
            equationResetFlag = true;

            SFXSource.PlayOneShot(equateSound, 0.7f);
            newCurrentCounter.text = "0";
            haveGasped = false;
            //var thisSparkleParticle = Instantiate(sparkleParticle, playerNumber.gameObject.transform.position, Quaternion.identity);
            //Destroy(thisSparkleParticle, 2f);
        }
    }

    public void ThrowTrash()
    {
        if (trashCounter != 0)
        {
            var CPU = transform.Find("LabelCanvas").Find("CPU").GetComponent<CPU>();
            CPU.CPUInt += trashCounter;

            //var explosion = Instantiate(explosionParticle, GameObject.Find("CPUCounter").transform.position, Quaternion.identity);
            LeanTween.rotateZ(trashButton.gameObject, 0, 1f);
            //Destroy(explosion, 2f);
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
        
        if (gameMode == GameMode.Level3)
        {
            CPU.CPUTimerText.text = "00:45:0";
            CPU.CPUCountdownTime = timeLimit;
            CPU.CPUcountdownTimerIsRunning = false;
            playerSwitch = true;
        }

        if (gameMode == GameMode.Level4)
        {
            CPU.UpdateCPUGoal(minTarget, maxTarget);
        }

        playerNumber = 0;
        CPU.CPUInt = 0;
        playerNumber = Random.Range(minTarget, maxTarget);
        currentCounter.text = playerNumber.ToString();

        StartCoroutine(PlayBGMSource());
        triggeredVictory = false;
    }

    private IEnumerator DisplayResults()
    {
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

    private IEnumerator PlayBGMSource()
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
        //currentCountdownTime += timeAdded;
    }

    public void DisplayTime(float timeToDisplay, Text textToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        float milliSeconds = (timeToDisplay % 1) * 1000;

        textToDisplay.text = string.Format("{0:00}:{1:00}:{2:0}", minutes, seconds, milliSeconds);
    }

    private IEnumerator fadeTextInOut()
    {
        newCurrentTextFading = true;
        LeanTween.textColor(newCurrentCounter.rectTransform, Color.clear,1.5f);
        LeanTween.textColor(newCurrentCounter.rectTransform, newCurrentColor,1.5f);
        yield return new WaitForSeconds(1.5f);
        LeanTween.textColor(newCurrentBack.rectTransform, Color.clear,1.5f);
        LeanTween.textColor(newCurrentBack.rectTransform, newCurrentBackColor, 1.5f);
    }
    
    public void Update()
    {
        int topboundNum = 5;
        int bottomboundNum = -5;
        
        for(int i = 0; i < calcs.Count; i++)
        {
            calcsString = string.Join("", calcs.ToArray());
        }

        if (calcs.Count == 1 || calcs.Count == 3 || calcs.Count == 5 || calcs.Count == 7)
        {
            numExpression = parser.ParseNumber(calcsString);
            output = Convert.ToInt32(numExpression.GetNumber());
        }

        else
        {
            output = 0;
        }

        //Fix this later
        if (calcsString != "0+0")
        {
            //currentCounter.text = calcsString;
        }
        else
        {
            //currentCounter.text = "0";
        }

        if (calcs.Count >= 1)
        {
            if (calcs[0] == "+" || calcs[0] == "-" || calcs[0] == "*" || calcs[0] == "/")
            {
                calcs.Remove(calcs[0]);
            }
        }

        if (calcs.Count <= 0)
        {
            calcsString = "0+0";
        }

        if (gameMode == GameMode.Level1)
        {
            currentCounter.text = "???";
            newCurrentCounter.text = "???";
        }
        else if (output != 0)
        {
            if (newCurrentTextFading)
            {
                StopCoroutine(fadeTextInOut());
            }
            if (additionMode)
            {
                newCurrentCounter.text = (output + playerNumber).ToString();
            }
            else
            {
                newCurrentCounter.text = (output - playerNumber).ToString();
            }
            
            //StartCoroutine(fadeTextInOut());
        }
        else
        {
            newCurrentCounter.text = "0";
            newCurrentBack.color = newCurrentBackColor;
            newCurrentCounter.color = newCurrentColor;
        }

        if (playerNumber < 0)
        {
            operationText.text = "+";
            equateButtonText.text = "Add";
            additionMode = true; 
        }

        if (playerNumber > 0)
        {
            operationText.text = "-";
            equateButtonText.text = "Subtract";
            additionMode = false;
        }

        //Change BG color during event
        if (flashObj != null)
        {
            if (playerNumber <= topboundNum && playerNumber > bottomboundNum && playerNumber != 0)
            {
                StopCoroutine(RedFlash());
                StartCoroutine(GreenFlash());
            }

            else if (playerNumber >= topboundNum && playerNumber != 0)
            {
                StopCoroutine(GreenFlash());
                StartCoroutine(RedFlash());
            }

            else if (playerNumber <= bottomboundNum || playerNumber == 0)
            {
                StopCoroutine(GreenFlash());
                StopCoroutine(RedFlash());
                StartCoroutine(NullFlash());
            }
        }

        //Play low pitch gasping sound if close
        if (playerNumber == 3 || playerNumber == - 3)
        {   if (!haveGasped)
            {
                SFXSource.PlayOneShot(audienceGaspFar, 0.7f);
                haveGasped = true;
            }
        }
        
        //Play high pitch gasping sound if very close
        if (playerNumber == 1 || playerNumber == - 1)
        {   if (!haveGasped)
            {
                SFXSource.PlayOneShot(audienceGaspClose, 0.7f);
                haveGasped = true;
            }
        }
        
        if(currentCountdownTime > 0 && countdownTimerIsRunning)
        {
            currentCountdownTime -= Time.deltaTime;
            DisplayTime(currentCountdownTime, timerText);
        }

        if (currentCountdownTime <= 0)
        {
            timerText.text = "00:00:0"; countdownTimerIsRunning = false;
        }

        if (scoreAddFlag && equationCounter <= 2)
        {
            AddTime(5);
        }

        else if (scoreAddFlag && equationCounter == 3)
        {
            AddTime(3);
        }

        else if (scoreAddFlag && equationCounter == 4)
        {
            AddTime(1);
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

        if (gameMode == GameMode.Level4)
        {
            if (trashCounter != 0)
            {
                LeanTween.rotateZ(trashButton.gameObject, 0, 1f);
                trashCountText.text = trashCounter.ToString();
            }

            if (trashButton.transform.rotation.z == 0f && trashCounter == 0)
            {
                LeanTween.rotateZ(trashButton.gameObject, -180, 1f);
                trashCountText.text = "0";
            }
        }

        if (output != 0)
        {
            if (calcs.Count == 3 || calcs.Count == 5 || calcs.Count == 7)
            {
                equateColorBlock.selectedColor = new Color(0, 1, 0, 1);
                equateColorBlock.pressedColor = new Color(0, 0.32f, 0.078f, 1);
                equateColorBlock.normalColor = new Color(0, 1, 0, 1); 
                equateButton.enabled = true;
                LeanTween.rotateZ(equateButton.gameObject, -180, 0.3f);
            }
            
        }
        else
        {
            equateColorBlock.selectedColor = new Color(0.68f, 0.68f, 0.68f, 1);
            equateColorBlock.pressedColor = new Color(0.68f, 0.68f, 0.68f, 1);
            equateColorBlock.normalColor = new Color(0.68f, 0.68f, 0.68f, 1);
            equateButton.enabled = false;
            LeanTween.rotateZ(equateButton.gameObject, 0, 0.3f);

            if (equateButton.transform.rotation.z == 0f)
            {
                LeanTween.rotateZ(equateButton.gameObject, -180, 0.3f);
            }
        }

        if (scoreAddFlag)
        {
            equationCounter = 0;
        }

        //Time Trial Mode
        if (gameMode == GameMode.TimeTrial)
        {
            if (playerNumber == 0)
            {
                scoreAddFlag = true;
                SFXSource.PlayOneShot(GoalChange, 0.5f);
                //var thisChangeParticle = Instantiate(ChangeParticle, gameObject.transform.position, Quaternion.identity);
                //thisChangeParticle.transform.SetParent(null);
                //Destroy(thisChangeParticle, 2f);
                playerNumber = Random.Range(minTarget, maxTarget);
                currentCounter.text = playerNumber.ToString();
                if (ShowScore.playerScore > (ShowBestScore.displayBestScore - 100))
                {PlayerPrefs.SetInt("TimeTrial", ShowScore.playerScore + 100);}
            }

            else
            {
                scoreAddFlag = false;
            }
            Debug.Log("triggered victory = " + triggeredVictory);
            if(CountDownTimer <= 0 && !triggeredVictory)
            {
                //BGMSource.clip = VictoryBGM;
                //StartCoroutine(DisplayResults());
            }

            
        }

        //Unknown Goal Mode
        if (gameMode == GameMode.Level1)
        {
            if (playerNumber == 0)
            {
                scoreAddFlag = true;
                SFXSource.PlayOneShot(GoalChange, 0.5f);
                //var thisChangeParticle = Instantiate(ChangeParticle, gameObject.transform.position, Quaternion.identity);
                //thisChangeParticle.transform.SetParent(null);
                //Destroy(thisChangechangeParticle, 2f);
                playerNumber = Random.Range(minTarget, maxTarget);
                currentCounter.text = playerNumber.ToString();
                if (ShowScore.playerScore > (ShowBestScore.displayBestScore - 100))
                {PlayerPrefs.SetInt("Level1", ShowScore.playerScore + 100);}
            }

            else
            {
                scoreAddFlag = false;
            }
            if(CountDownTimer <= 0 && triggeredVictory)
            {
                BGMSource.clip = VictoryBGM;
                StartCoroutine(DisplayResults());
            }

            
        }

        //Move Counter Mode
        if (gameMode == GameMode.Level2)
        {
            if (playerNumber == 0)
            {
                CountMoves.moveCounter += 3;
                CountMoves.levelCounter += 1;
                SFXSource.PlayOneShot(GoalChange, 0.5f);
                //var thisChangeParticle = Instantiate(ChangeParticle, gameObject.transform.position, Quaternion.identity);
                //thisChangeParticle.transform.SetParent(null);
                //Destroy(thisChangeParticle, 2f);
                playerNumber = Random.Range(minTarget, maxTarget);
                currentCounter.text = playerNumber.ToString();
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
                if (playerNumber == 0)
                {
                    CPCountdownTime = timeLimit;
                    SFXSource.PlayOneShot(GoalChange, 0.5f);
                    CPU.UpdateCPUGoal(minTarget, maxTarget);
                    CPU.canAct = true;
                    CPU.CPUcountdownTimerIsRunning = true;

                    countdownTimerIsRunning = false;
                    spawnBlocks.canBeSelected = false;
                    playerSwitch = false;
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
                    //thisChangeParticle = Instantiate(ChangeParticle, gameObject.transform.position, Quaternion.identity);
                    //thisChangeParticle.transform.SetParent(null);
                    //Destroy(thisChangeParticle, 2f);
                    playerNumber = Random.Range(minTarget, maxTarget);
                    currentCounter.text = playerNumber.ToString();

                    CPU.canAct = false;
                    CPU.CPUcountdownTimerIsRunning = false;
                    
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

            if (playerNumber == 0)
            {
                playerNumber = Random.Range(minTarget, maxTarget);
                currentCounter.text = playerNumber.ToString();
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

    #region BG Flashes
    private IEnumerator RedFlash()
    {
        flashObj.SetActive(true);
        LeanTween.color(flashObj, Color.red, 1f).setDelay(1f);
        LeanTween.color(flashObj, Color.clear, 1f).setDelay(1f);
        yield return null;
    }

    private IEnumerator GreenFlash()
    {
        flashObj.SetActive(true);
        LeanTween.color(flashObj, Color.green, 1f).setDelay(1f);
        LeanTween.color(flashObj, Color.clear, 1f).setDelay(1f);
        yield return null;
    }

    private IEnumerator NullFlash()
    {
        flashObj.SetActive(false);
        yield return null;
    }
    #endregion
}
