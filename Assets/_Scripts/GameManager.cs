using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using B83.LogicExpressionParser;
using Random=UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")] public int minTarget;
    public int maxTarget;
    public GameMode gameMode;
    public int playerNumber;
    public float currentCountdownTime = 150f;
    public int width;
    public int height;

    [Header("Game Objects")] public GameObject StartCountDownText;

    public GameObject flashObj;

    //public GameObject ChangeParticle;
    //public GameObject sparkleParticle; 
    //public GameObject explosionParticle;
    public GameObject tilePrefab;
    public GameObject linePrefab;
    public GameObject destroyParticle;
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

    [HideInInspector] public bool equationResetFlag, scoreAddFlag, countdownTimerIsRunning, scoreReset;
    [HideInInspector] public float bestTimeTrial;
    [HideInInspector] public Transform spawner;

    private bool canShuffle, countDownSoundActive, triggeredVictory, newCurrentTextFading;
    [SerializeField] private bool additionMode;
    private AudioSource SFXSource;
    private Button shuffleButton, equateButton, trashButton;

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
        hiScoreCounter,
        hiScoreLabel,
        uncalculatedText;

    private ColorBlock shuffleColorBlock, equateColorBlock;
    private Color newCurrentColor, newCurrentBackColor, goldColor;
    private SwipeBlocks swipeBlocks;
    private StartCountDown startCountDown;
    private AudioSource BGMSource;
    private float musicPos, CPCountdownTime, timeLimit, CountDownTimer;
    private int equationCounter, shuffleCounter, playerScore;
    private GameObject CPUTrashLevelText, resultsCanvas;

    [SerializeField] private int shuffleCooldown;

    public static bool haveGasped;

    public List<string> calcs = new();
    public int output;
    public string calcsString;
    private Parser parser = new();
    private NumberExpression numExpression;
    private PlayerRecords playerSave;

    public enum GameMode
    {
        TimeTrial,
        Level1,
        Level2,
        Level3,
        Level4
    }

    void Awake()
    {
        try
        { 
            playerSave = GameObject.Find("PlayerRecords").GetComponent<PlayerRecords>();
        }
        catch (Exception e)
        {
            playerSave = Instantiate(playerRecords).GetComponent<PlayerRecords>();
        }

        //getting scripts for necessary game functions (equating items, spawning blocks, etc.)

        equationCounter = 0;
        shuffleCounter = 5;
        shuffleCooldown = 3;
        spawner = transform.Find("Spawner");
        shuffleButton = transform.Find("ButtonCanvas").Find("ShuffleButton").GetComponent<Button>();
        equateButton = transform.Find("ButtonCanvas").Find("EquateButton").GetComponent<Button>();
        resultsCanvas = transform.Find("BGCanvas").Find("ResultsScreen").gameObject;

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
        uncalculatedText = transform.Find("LabelCanvas").Find("UncalculatedNumberCounter").GetComponent<Text>();
        newCurrentCounter = transform.Find("LabelCanvas").Find("CalculatedNumberCounter").GetComponent<Text>();
        newCurrentBack = transform.Find("LabelCanvas").Find("CalculatedNumberCounterBack").GetComponent<Text>();
        newCurrentColor = newCurrentCounter.color;
        newCurrentBackColor = newCurrentBack.color;
        goldColor = new Color(255, 215, 0);

        SFXSource = gameObject.GetComponent<AudioSource>();
        BGMSource = transform.Find("BGMSource").GetComponent<AudioSource>();

        timeLimit = 45f;
        musicPos = 0f;
        StartCoroutine(PlayBGMSource());

        canBeSelected = false;

        countDownSoundActive = false;
        timerText = transform.Find("LabelCanvas").Find("TimeCounter").GetComponent<Text>();
        timerLabel = transform.Find("LabelCanvas").Find("TimerLabel").GetComponent<Text>();
        countdownTimerIsRunning = false;
        DisplayTime(currentCountdownTime, timerText);

        haveGasped = false;
        equateButtonText = equateButton.transform.Find("EquateText").GetComponent<Text>();
        shuffleButtonText = shuffleButton.transform.Find("ShuffleText").GetComponent<Text>();
        operationText = transform.Find("ButtonCanvas").Find("SwitchButton").Find("Text").GetComponent<Text>();
        hiScoreCounter = transform.Find("LabelCanvas").Find("Hi-ScoreCounter").GetComponent<Text>();
        hiScoreLabel = transform.Find("LabelCanvas").Find("Hi-ScoreLabel").GetComponent<Text>();
        additionMode = true;
        SFXSource.Stop();
        flashObj = GameObject.Find("FlashBG");

        //Initialise output and calc string to be 0
        output = 0;
        calcsString = "0+0";

        calcs.Clear();

        playerScoreText = transform.Find("LabelCanvas").Find("ScoreCounter").GetComponent<Text>();

        if (scoreReset)
        {
            playerScore = 0;
        }

        canAddBlock = true;
        canBeSelected = false;
        allBlocks = new SwipeBlocks[width, height];
        AddAllBlocks(width, height);
        
        if (gameMode == GameMode.Level3)
        {
            timerText.gameObject.SetActive(false);
            timerLabel.gameObject.SetActive(false);
            hiScoreCounter.gameObject.SetActive(true);
            hiScoreLabel.gameObject.SetActive(true);
        }
        else
        {
            timerText.gameObject.SetActive(true);
            timerLabel.gameObject.SetActive(true);
            hiScoreCounter.gameObject.SetActive(false);
            hiScoreLabel.gameObject.SetActive(false);
        }

        StartCoroutine(RefreshShuffle());
    }

    //AddAllBlocks instantiates all blocks SIMULTANEOUSLY, while AddBlock instantiates ONE BY ONE
    public void AddAllBlocks(int width, int height)
    {
        //Initial setup w/o any special rules, used in start and when shuffling
        canAddBlock = false;
        canBeSelected = true;

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
        
        yield return new WaitForSeconds(0.05f);
        var currentBlock = Instantiate(Blocks[randBlock2], tempPosition, Quaternion.identity);
        currentBlock.name = "(" + width + "," + height + ")";
        currentBlock.transform.SetParent(spawner);
        currentBlock.transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(currentBlock.gameObject, new Vector3(1.25f, 1.25f, 1.25f), 1f).setEaseOutBounce();

        var currentSwipeBlock = currentBlock.GetComponent<SwipeBlocks>();
        //currentSwipeBlock.lockBlock = true;
        yield return new WaitForSeconds(0.25f);
        
        currentSwipeBlock.lockBlock = false;
        canAddBlock = true;
        canBeSelected = true;
        allBlocks[width, height] = currentSwipeBlock;

        yield return new WaitForSeconds(0.3f);
        checkForDuplicates();
        
    }

    private void checkForDuplicates()
    {
        var index = 0;
        List<SwipeBlocks> allBlocksToCheck = new();
        
        foreach (var block in allBlocks)
        {
            allBlocksToCheck.Add(block);
            index++;
        }
        
        Debug.LogWarning(allBlocksToCheck.Count);

        for (int i = 0; i < allBlocksToCheck.Count - 1; i++)
        {
            var block1 = allBlocksToCheck[i];
            var block2 = allBlocksToCheck[i + 1];

            if (block1 != null && block2 != null && block1.name == block2.name)
            {
                Debug.LogWarning("DUPLICATE FOUND!");
                Destroy(block2.gameObject);
                allBlocksToCheck.Remove(block2);
            }
        }
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

                AddAllBlocks(5, 4);
                StartCoroutine(RefreshShuffle());

                if (gameMode == GameMode.Level4)
                {
                    shuffleCounter -= 1;
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

        if (gameMode == GameMode.Level3)
        {
            shuffleColorBlock.selectedColor = new Color(1, 0, 0, 1);
            shuffleColorBlock.pressedColor = new Color(1, 0, 0, 1);
            shuffleColorBlock.normalColor = new Color(1, 0, 0, 1);
            shuffleColorBlock.highlightedColor = new Color(1, 0, 0, 1);
            shuffleButton.colors = shuffleColorBlock;
            LeanTween.rotateZ(shuffleButton.gameObject, 0, 0.3f);
            equationResetFlag = true;
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

            //Aims to destroy all objects in selectedBlocks
            DeleteSelectedBlocks();

            equationCounter += 1;
            equationResetFlag = false;

            SFXSource.PlayOneShot(equateSound, 0.7f);
            newCurrentCounter.text = "???";
            haveGasped = false;

            foreach (var block in allBlocks)
            {
                if (block != null && block.myString == "") 
                    block.ReduceBreakCounter();
            }

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
                    equationResetFlag = true;
                    canShuffle = true;
                }
            }
            
            //var thisSparkleParticle = Instantiate(sparkleParticle, playerNumber.gameObject.transform.position, Quaternion.identity);
            //Destroy(thisSparkleParticle, 2f);
        }
    }

    private IEnumerator DisplayResults()
    {
        triggeredVictory = true;
        canBeSelected = false;

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
        currentCountdownTime += timeAdded;
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
        scoreAddFlag = false;
        //Destroy(thisSpark, 2f);
    }
    
    public void Update()
    {
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

        #region BG Flash Triggers
        //Change BG color during event
        if (flashObj != null)
        {
            //Player very close to goal
            if (playerNumber == 1 || playerNumber == -1)
            {
                StopCoroutine(RedFlash());
                StopCoroutine(GreenFlash());
                StartCoroutine(RapidGreenFlash());
            }
            
            else if (playerNumber <= 3 && playerNumber > -3 && playerNumber != 0)
            {
                StopCoroutine(RapidGreenFlash());
                StopCoroutine(RedFlash());
                StartCoroutine(GreenFlash());
            }

            //Player falling far below target
            else if (playerNumber <= -3 && playerNumber != 0)
            {
                StopCoroutine(RapidGreenFlash());
                StopCoroutine(GreenFlash());
                StartCoroutine(RedFlash());
            }

            else if ((playerNumber >= -3 && playerNumber != 0) || playerNumber == 0)
            {
                StopCoroutine(RapidGreenFlash());
                StopCoroutine(GreenFlash());
                StopCoroutine(RedFlash());
                StartCoroutine(NullFlash());
            }
        }
        #endregion

        #region Add Point Flags
        if (currentCountdownTime < 30 && scoreAddFlag)
        {
            AddPoints(100);
        }

        if (currentCountdownTime >= 30 && currentCountdownTime < 60 && scoreAddFlag)
        {
            AddPoints(150);
        }

        if (currentCountdownTime >= 60 && currentCountdownTime < 90 && scoreAddFlag)
        {
            AddPoints(200);
        }

        if (currentCountdownTime >= 90 && currentCountdownTime < 120 && scoreAddFlag)
        {
            AddPoints(250);
        }

        if (currentCountdownTime >= 120 && scoreAddFlag)
        {
            AddPoints(350);
        }

        playerScoreText.text = playerScore.ToString("00000");
        #endregion

        #region Gasp Triggers
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
        #endregion

        #region Activate Countdown Time
        if(currentCountdownTime > 0)
        {
            if (countdownTimerIsRunning)
            {
                currentCountdownTime -= Time.deltaTime;
                DisplayTime(currentCountdownTime, timerText);
            }
        }
        
        else
        {
            timerText.text = "00:00:0"; 
            countdownTimerIsRunning = false;
        }
        #endregion

        #region Time Bonus Triggers
        if (scoreAddFlag)
        {
            if (equationCounter <= 2)
            {
                AddTime(5);
            }

            else if (equationCounter == 3)
            {
                AddTime(3);
            }

            else if (equationCounter == 4)
            {
                AddTime(1);
            }
            
            equationCounter = 0;
        }
        #endregion

        #region Countdown Sound Triggers
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
        #endregion

        #region Equate Button Text and Appearance Triggers
        if (newCurrentCounter.text != "???")
        {
            if (!equateButton.enabled)
            {
                if (calcs.Count == 3 || calcs.Count == 5 || calcs.Count == 7)
                {
                    equateColorBlock.selectedColor = new Color(0, 1, 0, 1);
                    equateColorBlock.pressedColor = new Color(0, 0.32f, 0.078f, 1);
                    equateColorBlock.normalColor = new Color(0, 1, 0, 1);
                
                    LeanTween.rotateZ(equateButton.gameObject, 0, 0.3f);
                    equateButton.enabled = true;
                }
            }
        }
        else
        {
            if (equateButton.enabled)
            {

                //equateButton.enabled = true;
                equateColorBlock.selectedColor = new Color(0.68f, 0.68f, 0.68f, 1);
                equateColorBlock.pressedColor = new Color(0.68f, 0.68f, 0.68f, 1);
                equateColorBlock.normalColor = new Color(0.68f, 0.68f, 0.68f, 1);

                LeanTween.rotateZ(equateButton.gameObject, -180, 0.3f);
                equateButton.enabled = false;
            }
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
                scoreAddFlag = true;
                SFXSource.PlayOneShot(GoalChange, 0.5f);
                //var thisChangeParticle = Instantiate(ChangeParticle, gameObject.transform.position, Quaternion.identity);
                //thisChangeParticle.transform.SetParent(null);
                //Destroy(thisChangeParticle, 2f);
                playerNumber = Random.Range(minTarget, maxTarget);
                currentCounter.text = playerNumber.ToString();
            }

            else
            {
                scoreAddFlag = false;
            }
            //Debug.Log("triggered victory = " + triggeredVictory);
            if(CountDownTimer <= 0 && !triggeredVictory)
            {
                //BGMSource.clip = VictoryBGM;
                //StartCoroutine(DisplayResults());

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
                scoreAddFlag = true;
                SFXSource.PlayOneShot(GoalChange, 0.5f);
                //var thisChangeParticle = Instantiate(ChangeParticle, gameObject.transform.position, Quaternion.identity);
                //thisChangeParticle.transform.SetParent(null);
                //Destroy(thisChangechangeParticle, 2f);
                playerNumber = Random.Range(minTarget, maxTarget);
                currentCounter.text = playerNumber.ToString();
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

                if (CountMoves.levelCounter > playerSave.bestLevel2)
                {
                    playerSave.bestLevel2 = CountMoves.levelCounter;
                    //SaveSystem.SavePlayer(playerSave);
                }
            }
        }

        if (gameMode == GameMode.Level3)
        {
            shuffleButtonText.text = "Shuffle: " + shuffleCounter;
        }

        //First to 1000
        if (gameMode == GameMode.Level4)
        {
            if (playerNumber == 0 && !triggeredVictory)
            {
                //BGMSource.clip = VictoryBGM;
                //StartCoroutine(DisplayResults());
                //Debug.Log("triggered victory = " + triggeredVictory);
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
                        Debug.Log("spawned blocks: " + spawner.childCount);
                    }
                }
            }
        }

        if (selectedBlocks.Count >= 2 && selectedBlocks[selectedBlocks.Count - 1] != null && selectedBlocks[selectedBlocks.Count - 1].myID > 1 && selectedBlocks[selectedBlocks.Count - 1].transform.childCount < 3)
        {
            if(selectedBlocks[selectedBlocks.Count - 1].transform.localScale.x > 1.2f)
            {
                Debug.Log("Draw a Line");
                var currentLine = Instantiate(linePrefab, selectedBlocks[selectedBlocks.Count - 1].transform.position, Quaternion.identity);
                currentLine.transform.SetParent(selectedBlocks[selectedBlocks.Count - 1].transform);
                DrawLine currentLinePoints = currentLine.GetComponent<DrawLine>();
                currentLinePoints.origin = selectedBlocks[selectedBlocks.Count - 1].gameObject.transform.transform;
                currentLinePoints.destination = selectedBlocks[selectedBlocks.Count - 2].gameObject.transform.transform;
            }
        }
        #endregion
    }

    #region BG Flash Commands
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
    
    private IEnumerator RapidGreenFlash()
    {
        flashObj.SetActive(true);
        LeanTween.color(flashObj, Color.green, 0.5f).setDelay(0.5f);
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
