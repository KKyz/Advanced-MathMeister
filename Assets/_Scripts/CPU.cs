using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CPU : MonoBehaviour
{
    
    //Clean this area, yuck
    public int Score;
    public float CPUCountdownTime;
    public bool CPUcountdownTimerIsRunning, CPUScoreAddFlag;
    public GameObject levelText, Explosion;
    public Text CPUTimerText;
    
    private Text scoreText;
    private GameObject CPUCounter;
    private GameManager gameManager;

    [HideInInspector]
    public int maxRand, minRand, CPUInt, CPUCalcInt, respondInt, CPULevel;

    public int respondPassInt;

    private Text Counter, currentLevelText;
    
    public bool canAct;
    
    public int CPUGoal;

    public int minGoal, maxGoal;
    public GameObject SparksPrefab;
    private GameObject sparks;
    private Text goalText;
    [SerializeField] private bool awakenedUpdate = false;

    void Start()
    {
        CPUScoreAddFlag = false;
        CPUCounter = GameObject.Find("CPUCounter");
        scoreText = gameObject.GetComponent<Text>();
        Score = 0; 
        scoreText.text = Score.ToString("00000");
        CPUCountdownTime = 45f;
        CPUTimerText = gameObject.GetComponent<Text>();
        CPUcountdownTimerIsRunning = false;
        gameManager = GameObject.Find("GameInterface").GetComponent<GameManager>();
        gameManager.DisplayTime(CPUCountdownTime, CPUTimerText);
        
        canAct = false;
        Counter = gameObject.GetComponent<Text>();
        currentLevelText = levelText.GetComponent<Text>();
        CPUInt = 0;
        CPUCalcInt = 0;
        CPULevel = 1;
        currentLevelText.text = "Level " + CPULevel.ToString();
        
        minRand = 1;
        maxRand = 4;
        
        if (SceneManager.GetActiveScene().name == "Level3")
        {respondPassInt = 1070;}

        if (SceneManager.GetActiveScene().name == "Level4")
        {respondPassInt = 1170;}
        
        goalText = gameObject.GetComponent<Text>();

        if (awakenedUpdate)
        {
            UpdateCPUGoal(minGoal, maxGoal);
            awakenedUpdate = false;
            Destroy(sparks);
        }
    }

    public void UpdateCPUGoal(int minGoal, int maxGoal)
    {
        CPUGoal = Random.Range(minGoal, maxGoal);
        goalText.text = CPUGoal.ToString();

        sparks = Instantiate(SparksPrefab, transform.position, Quaternion.identity);
        Destroy(sparks, 2f);
    }
    
    void Update()
    {
        //Effectively all CPU game logic is placed here
        if(CPUCountdownTime > 0 && CPUcountdownTimerIsRunning)
        {
            gameManager.DisplayTime(CPUCountdownTime, CPUTimerText);
            CPUCountdownTime -= Time.deltaTime;
        }

        if (CPUCountdownTime <= 0)
        {
            CPUTimerText.text = "00:00:0";
        }

        #region CPU Points Thresholds
        float timeLeft = gameManager.currentCountdownTime;

        if (timeLeft < 30 && CPUInt == CPUGoal)
        {
            CPUAddPoints(100);
        }

        if (timeLeft >= 30 && timeLeft < 60 && CPUInt == CPUGoal)
        {
            CPUAddPoints(150);
        }

        if (timeLeft >= 60 && timeLeft < 90 && CPUInt == CPUGoal)
        {
            CPUAddPoints(200);
        }

        if (timeLeft >= 90 && timeLeft < 120 && CPUInt == CPUGoal)
        {
            CPUAddPoints(250);
        }

        if (timeLeft >= 120 && CPUInt == CPUGoal)
        {
            CPUAddPoints(350);
        }
        #endregion
        
        
        Counter.text = CPUInt.ToString();
        
        if (canAct)
        {
            /* The CPU calculates a random number each frame. If the random number surpasses the response threshold,
             then another random number is generated. This second random number dictates how much is added to the CPUInt*/
            respondInt = Random.Range(1, respondPassInt + 1);

            if (respondInt >= respondPassInt)
            {
                CPUCalcInt = Random.Range(minRand, maxRand + 1);
                
                //If CPU's value is greater than the goal do this (second statement explained below)
                if (CPUInt < CPUGoal && CPUInt != CPUGoal - 1)
                {
                    CPUInt += CPUCalcInt + 1;

                    if (SceneManager.GetActiveScene().name == "Level4")
                    {
                        var TrashInt = Random.Range(1, 20);

                        if (TrashInt >= 19)
                        {
                            var TrashToThrow = Random.Range(1, 3);
                            if (TrashToThrow == 1)
                            {
                                GameObject.Find("ButtonCanvas").GetComponent<GameManager>().playerNumber += 5;
                            }
                            else if (TrashToThrow != 1)
                            {
                                GameObject.Find("ButtonCanvas").GetComponent<GameManager>().playerNumber -= 5;
                            }
                            var thisExplosion = Instantiate(Explosion, GameObject.Find("YourNumberCounter").transform.position, Quaternion.identity);
                            Destroy(thisExplosion, 2f);
                        }
                    }
                }
                
                //CPUInt 

                if (CPUInt > CPUGoal && CPUInt != CPUGoal + 1)
                {
                    CPUInt -= CPUCalcInt - 1;

                    if (SceneManager.GetActiveScene().name == "Level4")
                    {
                        var TrashInt = Random.Range(1, 20);

                        if (TrashInt >= 19)
                        {
                            var TrashToThrow = Random.Range(1, 3);
                            if (TrashToThrow == 1)
                            {
                                GameObject.Find("ButtonCanvas").GetComponent<GameManager>().playerNumber += 5;
                            }
                            else if (TrashToThrow != 1)
                            {
                                GameObject.Find("ButtonCanvas").GetComponent<GameManager>().playerNumber -= 5;
                            }
                            var thisExplosion = Instantiate(Explosion, GameObject.Find("YourNumberCounter").transform.position, Quaternion.identity);
                            Destroy(thisExplosion, 2f);
                        }
                    }
                }

                //CPU Gets a crutch over the player, where it can immediatly round to its goal int if it is one number away from it (Cheeky bastard!)
                else if (CPUInt == CPUGoal - 1 || CPUInt == CPUGoal + 1)
                {
                    CPUInt = CPUGoal;
                }
            }
        }
    }

    #region Add Points & Level Up Functions
    private void CPUAddPoints(int Amount)
    {
        //Whenever the CPU reaches its own target goal, it adds a set amount of score (equal to the values given to the player)
        Score += Amount;
        scoreText.text = Score.ToString("00000");
        CPUScoreAddFlag = false;
    }
    
    public void CPULevelUp()
    {
        //Function to make the CPU harder next round by lowering the random number needed to pass
        //lowers PassInt (increases response rate) whenever a level has been passed, and raises the maximum ans int it can produce by 2 whenever 3 levels have been passed
        CPULevel += 1;
        currentLevelText.text = "Level " + CPULevel;
        respondPassInt -= 10;

        if (CPULevel % 3 == 0)
        {maxRand += 7;}
    }
    #endregion
}
