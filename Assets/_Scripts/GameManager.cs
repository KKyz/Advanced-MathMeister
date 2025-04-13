using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using B83.LogicExpressionParser;
using Unity.Mathematics;
using Random=UnityEngine.Random;

public enum GameMode
{
    TimeTrial,
    Level1,
    Level2,
    Level3,
    Level4
}

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")] 
    public int minTarget;
    public int maxTarget;
    public GameMode gameMode;
    public int playerNumber;
    public float countDownTimer = 60f;
    public int width;
    public int height;

    [Header("Particles")]
    public GameObject goalParticle;
    public GameObject sparkleParticle; 
    public GameObject explosionParticle;

    public GameObject tilePrefab;
    public GameObject linePrefab;
    public GameObject playerRecords;
    
    

    public GameObject[] Blocks;
    public SwipeBlocks[,] allBlocks;
    List<List<string>> lists = new List<List<string>>();
    public List<SwipeBlocks> selectedBlocks = new();
    public bool canBeSelected;

    private Vector2 tempPosition;

    [SerializeField]private bool canAddBlock;

    [Header("Game Audio")] public AudioClip equateSound;
    public AudioClip shuffleSound;
    public AudioClip StartCountdownSound;
    public AudioClip FinalCountdownSound;
    public AudioClip VictoryBGM;
    public AudioClip LevelBGM;
    public AudioClip FailBGM;
    public AudioClip GoalChange;
    public AudioClip audienceGaspFar;
    public AudioClip audienceGaspClose;
    public AudioClip audienceGaspMid;
    public bool countDownTimerIsRunning;

    [HideInInspector] public bool equationResetFlag;
    [HideInInspector] public float bestTimeTrial;
    [HideInInspector] public Transform spawner;

    private bool canShuffle, countDownSoundActive, newCurrentTextFading;
    [SerializeField] private bool additionMode, triggeredVictory;
    private AudioSource SFXSource, finalCountdownSource;
    private Button shuffleButton, equateButton, trashButton;
    private string currentFlash;

    private Text currentCounter,
        trashCountText,
        timerText,
        timerLabel,
        operationText,
        equateButtonText,
        shuffleButtonText,
        newCurrentCounter,
        newCurrentBack,
        playerScoreText,
        playerScoreLabel,
        moveCounterText,
        moveCounterLabel,
        uncalculatedText,
        startCountdownText;

    private ColorBlock shuffleColorBlock, equateColorBlock;
    private Color newCurrentColor, newCurrentBackColor, goldColor;
    private SwipeBlocks swipeBlocks;
    private AudioSource BGMSource;
    private float musicPos, timeLimit, startCountDownTime;
    private int equationCounter, shuffleCounter, playerScore, moveCounter, moveCounterAdd, sprintCounter;
    private GameObject CPUTrashLevelText, resultsCanvas;

    [SerializeField] private int shuffleCooldown;

    public List<string> calcs = new();
    public int output;
    public string calcsString;
    private Parser parser = new();
    private NumberExpression numExpression;
    private PlayerRecords playerSave;
    private int debugFlashCount = 0;
    private int debugParticleCount = 0;
    private SpriteRenderer flashObj;

    void Awake()
    {
        try
        { 
            playerSave = GameObject.Find("PlayerRecords").GetComponent<PlayerRecords>();
        }
        catch (Exception e)
        {
            playerSave = Instantiate(playerRecords).GetComponent<PlayerRecords>();
            playerSave.name = "PlayerRecords";
        }

        //getting scripts for necessary game functions (equating items, spawning blocks, etc.)

        equationCounter = 0;
        shuffleCounter = 5;
        shuffleCooldown = 3;
        moveCounter = 6;
        moveCounterAdd = 4;
        sprintCounter = 5;
        startCountDownTime = 4f;
        spawner = transform.Find("Spawner");
        shuffleButton = transform.Find("ButtonCanvas").Find("ShuffleButton").GetComponent<Button>();
        equateButton = transform.Find("ButtonCanvas").Find("EquateButton").GetComponent<Button>();
        resultsCanvas = transform.Find("BGCanvas").Find("ResultsScreen").gameObject;

        shuffleColorBlock = shuffleButton.GetComponent<Button>().colors;
        equateColorBlock = equateButton.GetComponent<Button>().colors;
        equationResetFlag = false;
        canShuffle = true;

        playerNumber = Random.Range(minTarget, maxTarget);
        
        newCurrentTextFading = false;
        triggeredVictory = false;
        currentCounter = transform.Find("LabelCanvas").Find("YourNumberCounter").GetComponent<Text>();
        currentCounter.text = playerNumber.ToString();
        uncalculatedText = transform.Find("LabelCanvas").Find("UncalculatedNumberCounter").GetComponent<Text>();
        newCurrentCounter = transform.Find("LabelCanvas").Find("CalculatedNumberCounter").GetComponent<Text>();
        newCurrentBack = transform.Find("LabelCanvas").Find("CalculatedNumberCounterBack").GetComponent<Text>();
        startCountdownText = transform.Find("BGCanvas").Find("StartCountdown").GetComponent<Text>();
        newCurrentColor = newCurrentCounter.color;
        newCurrentBackColor = newCurrentBack.color;
        goldColor = new Color(255, 215, 0);

        SFXSource = gameObject.GetComponent<AudioSource>();
        BGMSource = transform.Find("BGMSource").GetComponent<AudioSource>();
        finalCountdownSource = transform.Find("BGCanvas").GetComponent<AudioSource>();

        timeLimit = 45f;
        musicPos = 0f;
        StartCoroutine(PlayBGMSource());

        canBeSelected = false;

        countDownSoundActive = false;
        timerText = transform.Find("LabelCanvas").Find("TimeCounter").GetComponent<Text>();
        timerLabel = transform.Find("LabelCanvas").Find("TimerLabel").GetComponent<Text>();
        countDownTimerIsRunning = false;
        DisplayTime(countDownTimer, timerText);

        equateButtonText = equateButton.transform.Find("EquateText").GetComponent<Text>();
        shuffleButtonText = shuffleButton.transform.Find("ShuffleText").GetComponent<Text>();
        operationText = transform.Find("ButtonCanvas").Find("SwitchButton").Find("Text").GetComponent<Text>();
        moveCounterText = transform.Find("LabelCanvas").Find("MoveCounter").GetComponent<Text>();
        moveCounterLabel = transform.Find("LabelCanvas").Find("MoveLabel").GetComponent<Text>();
        additionMode = true;
        flashObj = GameObject.Find("FlashBG").GetComponent<SpriteRenderer>();

        //Initialise output and calc string to be 0
        output = 0;
        calcsString = "0+0";

        calcs.Clear();

        playerScoreText = transform.Find("LabelCanvas").Find("ScoreCounter").GetComponent<Text>();
        playerScoreLabel = transform.Find("LabelCanvas").Find("ScoreLabel").GetComponent<Text>();

        canAddBlock = true;
        canBeSelected = false;
        allBlocks = new SwipeBlocks[width, height];
        AddAllBlocks();
        
        if (gameMode == GameMode.Level2)
        {
            timerText.gameObject.SetActive(false);
            timerLabel.gameObject.SetActive(false);
            moveCounterText.gameObject.SetActive(true);
            moveCounterLabel.gameObject.SetActive(true);

            moveCounterText.text = moveCounter.ToString();
        }
        else
        {
            timerText.gameObject.SetActive(true);
            timerLabel.gameObject.SetActive(true);
            moveCounterText.gameObject.SetActive(false);
            moveCounterLabel.gameObject.SetActive(false);
        }

        if (gameMode == GameMode.Level4)
        {
            timerLabel.text = "Timer: ";
            playerScoreText.text = sprintCounter.ToString();
        }

        StartCoroutine(RefreshShuffle());
        
        FlashScreen();
    }

    //AddAllBlocks instantiates all blocks SIMULTANEOUSLY, while AddBlock instantiates ONE BY ONE
    public void AddAllBlocks()
    {
        //Initial setup w/o any special rules, used in start and when shuffling
        canAddBlock = false;

        //Clears out all contents in game object
        foreach (Transform child in spawner)
        {
            Destroy(child.gameObject);
        }

        //Creating grid of blocks
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {

                tempPosition = new Vector2(i, j);

                //First, it creates all of the background tiles
                var backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity);
                backgroundTile.transform.SetParent(spawner);
                backgroundTile.name = "(" + i + "," + j + ")B";

                /*Secondly, the block to be placed on the background tile position is decided. This is done through
                 two random variables. randBlock1 is an arbitrary probability counter 
                 that settles on a tier of blocks (e.g. most powerful blocks have a 15% chance
                 of appearing). randBlock2 decides which one from that tier block (e.g. choosing *, 8, 9, etc.).*/
                
                var randBlock1 = Random.Range(0, 100);
                var randBlock2 = 0;

                if (randBlock1 < 45)
                {
                    randBlock2 = Random.Range(0, 5);
                }
                else if (randBlock1 >= 45 && randBlock1 < 90)
                {
                    randBlock2 = Random.Range(5, 9);
                }
                else if (randBlock1 >= 90)
                {
                    randBlock2 = Random.Range(9, 13);
                }

                //Creates and places these blocks into the grid
                var currentBlock = Instantiate(Blocks[randBlock2].gameObject, tempPosition, Quaternion.identity);
                currentBlock.transform.localScale = new Vector3(0, 0, 0);
                LeanTween.scale(currentBlock.gameObject, new Vector3(1.25f, 1.25f, 1.25f), 1f).setEaseOutBounce();
                currentBlock.transform.SetParent(spawner);
                currentBlock.name = "(" + i + "," + j + ")";

                //The block is tracked by placing it into an array called allBlocks
                allBlocks[i, j] = currentBlock.GetComponent<SwipeBlocks>();
            }
        }

        canAddBlock = true;
        
        List<String> allBlockNames = new();
        foreach (var block in allBlocks)
        {
            if (block != null)
                allBlockNames.Add(block.gameObject.name);
        }

        Debug.Log($"[{string.Join(",", allBlockNames)}]");
        selectedBlocks.Clear();
    }

    private IEnumerator AddBlock(int width, int height)
    {
        canBeSelected = true;
        tempPosition = new Vector2(width, 3);

        /*Very similar to doing AddAllBlocks(). The difference is that AddBlock prevents blocks from moving when new ones are
         being made*/
        var randBlock1 = Random.Range(0, 100);
        var randBlock2 = 0;

        if (randBlock1 < 45)
        {
            randBlock2 = Random.Range(0, 5);
        }

        else if (randBlock1 < 75 && randBlock1 >= 45)
        {
            randBlock2 = Random.Range(5, 9);
        }

        else if (randBlock1 >= 75)
        {
            randBlock2 = Random.Range(9, 13);
        }
        
        yield return new WaitForSeconds(0.08f);
        var currentBlock = Instantiate(Blocks[randBlock2], tempPosition, Quaternion.identity);
        currentBlock.name = "(" + width + "," + height + ")";
        currentBlock.transform.SetParent(spawner);
        currentBlock.transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(currentBlock.gameObject, new Vector3(1.25f, 1.25f, 1.25f), 1f).setEaseOutBounce();

        var currentSwipeBlock = currentBlock.GetComponent<SwipeBlocks>();
        currentSwipeBlock.lockBlock = true;
        yield return new WaitForSeconds(0.25f);
        
        currentSwipeBlock.lockBlock = false;
        canAddBlock = true;
        canBeSelected = true;
        allBlocks[width, height] = currentSwipeBlock;

        yield return new WaitForSeconds(0.3f);

    }

    public void DeleteSelectedBlocks()
    {
        if (calcs.Count > 0)
        {
            //Remove all blocks from list
            foreach (var block in selectedBlocks)
            {
                //var destroyParticleInst = Instantiate(destroyParticle, block.gameObject.transform.position, Quaternion.identity);
                //destroyParticleInst.transform.SetParent(block.gameObject.transform);

                foreach (Transform child in block.transform)
                {
                    Destroy(child.gameObject);
                }

                Destroy(block.gameObject, 1f);
                block.IDCounter = 0;
                block.GetComponent<SpriteRenderer>().material.color = block.myColor;
                block.selected = false;
                LeanTween.scale(block.gameObject, new Vector2(0, 0), 0.9f).setEaseInBounce();
                
            }
            
            //Clear all pointers to blocks
            selectedBlocks.Clear();
            calcs.Clear();
            calcsString = "0+0";
        }
    }
    
    public void RemoveBlock(int index)
    {
        //Code to remove blocks (NOT delete them)
        if (selectedBlocks.Count > 0)
        {
            //Remove calculation from calc (e.g. 9 + 10 + 8)
            for (int i = selectedBlocks.Count - 1; i >= index - 1; i--)
            {
                //Destroy All children objects (line, sphere) and reset variables of blocks
                foreach (Transform blockChild in selectedBlocks[i].transform)
                {
                    Destroy(blockChild.gameObject);
                }
                
                selectedBlocks[i].GetComponent<SpriteRenderer>().color = selectedBlocks[i].myColor;
                selectedBlocks[i].selected = false;
                selectedBlocks[i].IDCounter -= 1;
                
                calcs.RemoveAt(i);
                selectedBlocks.RemoveAt(i);
            }
        }
    }

    public void ShuffleBlocks()
    {
        if ((gameMode == GameMode.Level3 && shuffleCounter > 0) || gameMode != GameMode.Level3)
        {
            if (canShuffle)
            {
                equationResetFlag = true;
           
                //Select all blocks
                selectedBlocks.Clear();
            
                foreach (var block in allBlocks)
                {
                    selectedBlocks.Add(block);
                }
            
                DeleteSelectedBlocks();

                AddAllBlocks();
                StartCoroutine(RefreshShuffle());

                if (gameMode == GameMode.Level3)
                {
                    shuffleCounter -= 1;
                }
                
                else if (gameMode == GameMode.Level2)
                {
                    moveCounter--;
                    moveCounterText.text = moveCounter.ToString();
                }

                SFXSource.PlayOneShot(shuffleSound, 0.7f);

                shuffleCooldown = 3;
            }
        }
    }

    private IEnumerator RefreshShuffle()
    {
        canShuffle = false;
        shuffleColorBlock.selectedColor = new Color(0.68f, 0.68f, 0.68f, 1);
        shuffleColorBlock.pressedColor = new Color(0.68f, 0.68f, 0.68f, 1);
        shuffleColorBlock.pressedColor = new Color(0.68f, 0.68f, 0.68f, 1);
        shuffleColorBlock.highlightedColor = new Color(0.68f, 0.68f, 0.68f, 1);
        shuffleButton.colors = shuffleColorBlock;
        LeanTween.rotateZ(shuffleButton.gameObject, -180, 0.3f);
        yield return new WaitForSeconds(1f);
        equationResetFlag = true;

        if (gameMode == GameMode.Level3)
        {
            shuffleColorBlock.selectedColor = new Color(1, 0, 0, 1);
            shuffleColorBlock.pressedColor = new Color(1, 0, 0, 1);
            shuffleColorBlock.normalColor = new Color(1, 0, 0, 1);
            shuffleColorBlock.highlightedColor = new Color(1, 0, 0, 1);
            shuffleButton.colors = shuffleColorBlock;
            LeanTween.rotateZ(shuffleButton.gameObject, 0, 0.3f);
            canShuffle = true;
        }
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

            //Audience Gasp Sounds
            switch (playerNumber)
            {
                case 10:
                    SFXSource.PlayOneShot(audienceGaspFar, 1f);
                    break;
                case 5:
                    SFXSource.PlayOneShot(audienceGaspMid, 1f);
                    break;
                case 1:
                    SFXSource.PlayOneShot(audienceGaspClose, 1f);
                    break;
                case -10:
                    SFXSource.PlayOneShot(audienceGaspFar, 1f);
                    break;
                case -5:
                    SFXSource.PlayOneShot(audienceGaspMid, 1f);
                    break;
                case -1:
                    SFXSource.PlayOneShot(audienceGaspClose, 1f);
                    break;
                
            }

            if (gameMode != GameMode.Level4)
            {
                foreach (var block in selectedBlocks)
                {
                    if (block.isBonusBlock)
                    {
                        AddPoints(100);
                    }
                }
            }

            
            //Identify rows in which blocks will break in Level3
            if (gameMode == GameMode.Level3)
            {
                foreach (var breakBlock in allBlocks)
                {
                    if (breakBlock != null && breakBlock.myString == "")
                    {
                        StartCoroutine(breakBlock.ReduceBreakCounter());
                    }
                }
                
                List<Char> breakColumns = new();
                foreach (var block in selectedBlocks)
                {
                    breakColumns.Add(block.name[3]);
                }
                
                //Debug.Log($"[{string.Join(",", breakColumns)}]");

                /*foreach (var breakBlock in allBlocks)
                {
                    if (breakBlock != null && breakBlock.myString == "")
                    {
                        foreach (var column in breakColumns)
                        {
                            if (breakBlock.name[3] == column)
                            {
                                StartCoroutine(breakBlock.ReduceBreakCounter());
                            }
                        }
                    }
                }*/
            }

            //Aims to destroy all objects in selectedBlocks
            DeleteSelectedBlocks();

            equationCounter += 1;
            equationResetFlag = false;

            SFXSource.PlayOneShot(equateSound, 0.7f);
            newCurrentCounter.text = "???";

            if (gameMode != GameMode.Level3)
            {
                if (shuffleCooldown > 0)
                {
                    shuffleCooldown--;
                }
                
                if (shuffleCooldown == 0)
                {
                    shuffleColorBlock.selectedColor = new Color(1, 0, 0, 1);
                    shuffleColorBlock.pressedColor = new Color(1, 0, 0, 1);
                    shuffleColorBlock.normalColor = new Color(1, 0, 0, 1);
                    shuffleColorBlock.highlightedColor = new Color(1, 0, 0, 1);
                    shuffleButton.colors = shuffleColorBlock;
                    LeanTween.rotateZ(shuffleButton.gameObject, 0, 0.3f);
                    canShuffle = true;
                }
            }
            
            //var thisSparkleParticle = Instantiate(sparkleParticle, playerNumber.gameObject.transform.position, Quaternion.identity);
            //Destroy(thisSparkleParticle, 2f);
        }
        
        //Add Points and time depending on time left
        if (playerNumber == 0)
        {
            if (gameMode == GameMode.TimeTrial || gameMode == GameMode.Level1 || gameMode == GameMode.Level3)
            {
                switch (countDownTimer)
                {
                    case var timer when countDownTimer < 10:
                        AddPoints(100);
                        break;
            
                    case var timer when (countDownTimer >= 30 && countDownTimer < 60):
                        AddPoints(150);
                        break;
            
                    case var timer when (countDownTimer >= 60 && countDownTimer < 90):
                        AddPoints(200);
                        break;
                
                    case var timer when (countDownTimer >= 90 && countDownTimer < 120):
                        AddPoints(250);
                        break;

                    case var timer when countDownTimer >= 120:
                        AddPoints(350);
                        break;
                }
                
                switch (equationCounter)
                {
                    case 2:
                        countDownTimer += 15;
                        break;
                
                    case 3:
                        countDownTimer += 10;
                        break;
                
                    case 4:
                        countDownTimer += 5;
                        break;
                }

                equationCounter = 0;
            }
            
        }

        if (gameMode == GameMode.Level2)
        {
            moveCounter--;
            moveCounterText.text = moveCounter.ToString();
        }
        
        //Change BG color during event
        FlashScreen();
    }
    
    private void FlashScreen()
    {
        if (flashObj != null && gameMode != GameMode.Level4)
        {
            switch (playerNumber)
            {
                case var number when (playerNumber >= 20):
                    currentFlash = "YellowFlash";
                    break;
                case var number when (playerNumber <= 3 && playerNumber > 0):
                    currentFlash = "RapidYellowFlash";
                    break;
                case var number when (playerNumber >= -3 && playerNumber < 0):
                    currentFlash = "RapidVioletFlash";
                    break;
                case var number when (playerNumber <= -20):
                    currentFlash = "VioletFlash";
                    break;
                default:
                    currentFlash = "NullFlash";
                    break;
            }
            
            StartCoroutine(currentFlash);
        } 
    }

    private IEnumerator DisplayResults()
    {
        canBeSelected = false;
        triggeredVictory = true;
        finalCountdownSource.Pause();
        BGMSource.Pause();
        StartCoroutine(Fade.FadeIn());
        yield return new WaitForSeconds(1.5f);
        
        BGMSource.clip = VictoryBGM;
        musicPos = BGMSource.time;
        BGMSource.loop = false;
        BGMSource.time = 0f;
        BGMSource.Play();
        
        resultsCanvas.SetActive(true);
        
        Text yourResults = transform.Find("BGCanvas").Find("ResultsScreen").Find("YourTimeLabel").Find("YourTimeCounter")
            .GetComponent<Text>();

        if (gameMode == GameMode.Level4)
        {
            DisplayTime(countDownTimer, yourResults);
        }

        else
        {
            yourResults.text = playerScore.ToString("00000");
        }
    }

    private IEnumerator PlayBGMSource()
    {
        yield return new WaitForSeconds(0.1f);
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

    public void DisplayTime(float timeToDisplay, Text textToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        float milliSeconds = (timeToDisplay % 1) * 1000;

        textToDisplay.text = string.Format("{0:00}:{1:00}:{2:0}", minutes, seconds, milliSeconds);
    }

    //FIX THIS SHIT
    private IEnumerator fadeTextInOut()
    {
        newCurrentTextFading = true;
        LeanTween.textColor(newCurrentCounter.rectTransform, Color.clear,1.5f);
        LeanTween.textColor(newCurrentCounter.rectTransform, newCurrentColor,1.5f);
        yield return new WaitForSeconds(1.5f);
        LeanTween.textColor(newCurrentBack.rectTransform, Color.clear,1.5f);
        LeanTween.textColor(newCurrentBack.rectTransform, newCurrentBackColor, 1.5f);
    }
    
    private void AddPoints(int points)
    {
        //var thisSpark = Instantiate(SparksParticle, gameObject.transform.position, Quaternion.identity);
        //thisSpark.transform.SetParent(null);
        playerScore += points;
        playerScoreText.text = playerScore.ToString("00000");
        //Destroy(thisSpark, 2f);
    }

    public void DEBUGTriggerEnd()
    {
        StartCoroutine(DisplayResults());
    }

    public void Update()
    {
        #region Initial Countdown

        if (startCountDownTime > 1)
        {
            countDownTimerIsRunning = false;
            canBeSelected = false;
            startCountDownTime -= Time.deltaTime;
            startCountdownText.text = "" + (int)startCountDownTime;
        }
        else
        {
            if (!countDownTimerIsRunning && startCountdownText.gameObject.activeInHierarchy)
                StartCoroutine(Fade.SemiFadeOut());
            
            countDownTimerIsRunning = true;
            startCountDownTime = 0;
            startCountdownText.gameObject.SetActive(false);
            canBeSelected = true;
            countDownTimerIsRunning = true;
        }



        #endregion

        #region Calculating Items in Calcs List

        foreach (string item in calcs)
        {
            calcsString = string.Join("", calcs.ToArray());
        }

        if (calcs.Count == 3 || calcs.Count == 5 || calcs.Count == 7)
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
            uncalculatedText.text = "(" + calcsString + ")";
        }
        else
        {
            uncalculatedText.text = "";
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

        #endregion

        #region Update Current Counter (Based on Output from Calcs List)

        //During Unknown goal mode, your number counters should not show their numbers
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
                newCurrentCounter.text = (playerNumber + output).ToString();
            }
            else
            {
                newCurrentCounter.text = (playerNumber - output).ToString();
            }

            if (newCurrentCounter.text == "0")
            {
                newCurrentCounter.color = goldColor;
            }
            else
            {
                newCurrentCounter.color = newCurrentColor;
            }

            //StartCoroutine(fadeTextInOut());
        }
        else
        {
            newCurrentCounter.text = "???";
            newCurrentBack.color = newCurrentBackColor;
            newCurrentCounter.color = newCurrentColor;
        }

        if (gameMode != GameMode.Level1)
        {
            currentCounter.text = playerNumber.ToString();
        }

        #endregion

        #region Activate Countdown Time

        if (countDownTimer >= 0)
        {
            if (countDownTimerIsRunning)
            {
                if (gameMode != GameMode.Level4)
                {
                    countDownTimer -= Time.deltaTime;
                }
                else
                {
                    countDownTimer += Time.deltaTime;
                }
                
                DisplayTime(countDownTimer, timerText);
            }
        }

        else
        {
            timerText.text = "00:00:0";
            countDownTimerIsRunning = false;
        }

        #endregion

        #region Countdown Sound Triggers

        if (countDownTimer <= 10 && !countDownSoundActive && gameMode != GameMode.Level4)
        {
            finalCountdownSource.PlayOneShot(FinalCountdownSound);
            countDownSoundActive = true;
        }

        if (countDownTimer > 10 || !countDownTimerIsRunning)
        {
            finalCountdownSource.Stop();
            countDownSoundActive = false;
        }

        #endregion

        #region Equate Button Text and Appearance Triggers

        if (calcs.Count == 3 || calcs.Count == 5 || calcs.Count == 7)
        {
            if ((newCurrentCounter.text != "???" && gameMode != GameMode.Level1) || gameMode == GameMode.Level1)
            {
                if (!equateButton.enabled)
                {
                    equateColorBlock.selectedColor = new Color(0, 1, 0, 1);
                    equateColorBlock.pressedColor = new Color(0, 0.32f, 0.078f, 1);
                    equateColorBlock.normalColor = new Color(0, 1, 0, 1);

                    LeanTween.rotateZ(equateButton.gameObject, 0, 0.3f);
                    equateButton.enabled = true;
                }
            }
        }
        else if (equateButton.enabled)
        {
            equateColorBlock.selectedColor = new Color(0.68f, 0.68f, 0.68f, 1);
            equateColorBlock.pressedColor = new Color(0.68f, 0.68f, 0.68f, 1);
            equateColorBlock.normalColor = new Color(0.68f, 0.68f, 0.68f, 1);

            LeanTween.rotateZ(equateButton.gameObject, -180, 0.3f);
            equateButton.enabled = false;
        }

        if (playerNumber <= 0)
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

        #endregion

        #region Game Mode Conditions

        //Time Trial Mode
        if (gameMode == GameMode.TimeTrial)
        {
            if (playerNumber == 0)
            {
                SFXSource.PlayOneShot(GoalChange, 0.5f);
                //var thisChangeParticle = Instantiate(ChangeParticle, gameObject.transform.position, Quaternion.identity);
                //thisChangeParticle.transform.SetParent(null);
                //Destroy(thisChangeParticle, 2f);
                playerNumber = Random.Range(minTarget, maxTarget);
                currentCounter.text = playerNumber.ToString();
            }

            if (countDownTimer <= 0)
            {
                if (!triggeredVictory)
                {
                    StartCoroutine(DisplayResults());
                }
                

                if (playerScore > playerSave.bestTimeTrial)
                {
                    playerSave.bestTimeTrial = playerScore;
                    playerSave.SavePlayer();
                }
            }
        }

        //Unknown Goal Mode
        if (gameMode == GameMode.Level1)
        {
            if (playerNumber == 0)
            {
                SFXSource.PlayOneShot(GoalChange, 0.5f);
                //var thisChangeParticle = Instantiate(ChangeParticle, gameObject.transform.position, Quaternion.identity);
                //thisChangeParticle.transform.SetParent(null);
                //Destroy(thisChangechangeParticle, 2f);
                while (playerNumber == 0) 
                    playerNumber = Random.Range(minTarget, maxTarget);
                currentCounter.text = playerNumber.ToString();
            }

            if (countDownTimer <= 0 && !triggeredVictory)
            {
                StartCoroutine(DisplayResults());
                
                if (playerScore > playerSave.bestLevel1)
                {
                    playerSave.bestLevel1 = playerScore;
                    playerSave.SavePlayer();
                }
            }
        }

        //Move Counter Mode
        if (gameMode == GameMode.Level2)
        {
            if (playerNumber == 0)
            {
                moveCounter += moveCounterAdd;
                moveCounterText.text = moveCounter.ToString();

                if (moveCounterAdd > 1)
                {
                    moveCounterAdd--;
                }

                AddPoints(100);
                SFXSource.PlayOneShot(GoalChange, 0.5f);
                //var thisChangeParticle = Instantiate(ChangeParticle, gameObject.transform.position, Quaternion.identity);
                //thisChangeParticle.transform.SetParent(null);
                //Destroy(thisChangeParticle, 2f);
                while (playerNumber == 0) 
                    playerNumber = Random.Range(minTarget, maxTarget);
                currentCounter.text = playerNumber.ToString();
            }

            if (moveCounter <= 0 && !triggeredVictory)
            {
                StartCoroutine(DisplayResults());

                if (playerScore > playerSave.bestLevel2)
                {
                    playerSave.bestLevel2 = playerScore;
                    playerSave.SavePlayer();
                }
            }
        }

        if (gameMode == GameMode.Level3)
        {
            if (playerNumber == 0)
            {
                SFXSource.PlayOneShot(GoalChange, 0.5f);
                //var thisChangeParticle = Instantiate(ChangeParticle, gameObject.transform.position, Quaternion.identity);
                //thisChangeParticle.transform.SetParent(null);
                //Destroy(thisChangechangeParticle, 2f);
                while (playerNumber == 0) 
                    playerNumber = Random.Range(minTarget, maxTarget);
                currentCounter.text = playerNumber.ToString();
            }
            
            shuffleButtonText.text = "Shuffle: " + shuffleCounter;
            
            if (countDownTimer <= 0 && !triggeredVictory)
            {
                StartCoroutine(DisplayResults());
                
                if (playerScore > playerSave.bestLevel3)
                {
                    playerSave.bestLevel3 = playerScore;
                    playerSave.SavePlayer();
                }
            }
        }

        //5 Sprint
        if (gameMode == GameMode.Level4)
        {
            if (playerNumber == 0)
            {
                if (sprintCounter > 0)
                {
                    sprintCounter--;
                    playerScoreText.text = sprintCounter.ToString();
                    
                    while (playerNumber == 0) 
                        playerNumber = Random.Range(minTarget, maxTarget);
                    currentCounter.text = playerNumber.ToString();
                }
                else if (!triggeredVictory)
                {
                    StartCoroutine(DisplayResults());

                    if (countDownTimer < playerSave.bestLevel4)
                    {
                        playerSave.bestLevel4 = countDownTimer;
                        playerSave.SavePlayer();
                    }
                }
            }
        }

        #endregion

        #region Adding/Removing/Conecting Blocks

        //If a block is missing, insert one from above

        if (spawner.childCount < 40)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (allBlocks[i, j] == null && canAddBlock)
                    {
                        StartCoroutine(AddBlock(i, j));
                        canAddBlock = false;
                    }
                }
            }
        }

        if (selectedBlocks.Count >= 2 && selectedBlocks[selectedBlocks.Count - 1] != null &&
            selectedBlocks[selectedBlocks.Count - 1].transform.childCount < 3 && selectedBlocks[selectedBlocks.Count - 1].transform.localScale.x > 1.2f)
        {
            var currentLine = Instantiate(linePrefab, selectedBlocks[selectedBlocks.Count - 1].transform.position,
                Quaternion.identity);
            currentLine.transform.SetParent(selectedBlocks[selectedBlocks.Count - 1].transform);
            DrawLine currentLinePoints = currentLine.GetComponent<DrawLine>();
            currentLinePoints.origin = selectedBlocks[selectedBlocks.Count - 1].gameObject.transform.transform;
            currentLinePoints.destination = selectedBlocks[selectedBlocks.Count - 2].gameObject.transform.transform;
        }
    }

        #endregion

    #region BG Flash Commands
    private IEnumerator VioletFlash()
    {
        LeanTween.color(flashObj.gameObject, Color.magenta, 1.5f);
        LeanTween.color(flashObj.gameObject, Color.clear, 1.5f).setDelay(1.5f);
        yield return new WaitForSeconds(3f);
        
        if (currentFlash == "VioletFlash") 
            StartCoroutine(VioletFlash());
    }
    
    private IEnumerator RapidVioletFlash()
    {
        LeanTween.color(flashObj.gameObject, Color.magenta, 1f);
        LeanTween.color(flashObj.gameObject, Color.clear, 1f).setDelay(1f);
        yield return new WaitForSeconds(2f);
        
        if (currentFlash == "RapidVioletFlash") 
            StartCoroutine(RapidVioletFlash());
    }
    
    private IEnumerator YellowFlash()
    {
        LeanTween.color(flashObj.gameObject, Color.yellow, 1.5f);
        LeanTween.color(flashObj.gameObject, Color.clear, 1.5f).setDelay(1.5f);
        yield return new WaitForSeconds(3f);
        
        if (currentFlash == "YellowFlash") 
            StartCoroutine(YellowFlash());
    }
    
    private IEnumerator RapidYellowFlash()
    {
        LeanTween.color(flashObj.gameObject, Color.yellow, 1f);
        LeanTween.color(flashObj.gameObject, Color.clear, 1f).setDelay(1f);
        yield return new WaitForSeconds(2f);
        
        if (currentFlash == "RapidYellowFlash") 
            StartCoroutine(RapidYellowFlash());
    }

    private IEnumerator NullFlash()
    {
        if (flashObj.color != Color.clear)
            LeanTween.color(flashObj.gameObject, Color.clear, 1f);
        yield return null;
    }
    #endregion

    public void DEBUGTestParticles()
    {
        GameObject[] particles = {goalParticle, sparkleParticle, explosionParticle};

        debugParticleCount++;

        if (debugParticleCount > 2)
            debugParticleCount = 0;
        
        GameObject currentParticle = Instantiate(particles[debugParticleCount], transform);
        currentParticle.transform.position = transform.Find("BGCanvas").Find("StartCountdown").position;
        Destroy(currentParticle, 2f);
    }

    public void DEBUGIncrementFlash()
    {
        string[] flashes = {"YellowFlash", "RapidYellowFlash", "RapidVioletFlash", "VioletFlash", "NullFlash"};

        if (flashObj != null)
        {
            currentFlash = flashes[debugFlashCount];
        }

        StopCoroutine(currentFlash);
        StartCoroutine(currentFlash);

        debugFlashCount++;

        if (debugFlashCount >= 5)
            debugFlashCount = 0;
    }
}