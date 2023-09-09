using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CPU : MonoBehaviour
{
    public int Score;
    public float CPUCountdownTime;
    public bool CPUcountdownTimerIsRunning, CPUScoreAddFlag;
    public GameObject levelText, Explosion;
    public Text CPUTimerText;
    
    private Text scoreText;
    private GameObject CPUCounter;
    private GameManager gameManager;
    private float TimeLeft;

    [HideInInspector]
    public int maxRand, minRand, CPUInt, CPUCalcInt, RespondInt, CPULevel;

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
        if(CPUCountdownTime > 0 && CPUcountdownTimerIsRunning)
        {
            gameManager.DisplayTime(CPUCountdownTime, CPUTimerText);
            CPUCountdownTime -= Time.deltaTime;
        }

        if (CPUCountdownTime <= 0)
        {
            CPUTimerText.text = "00:00:0";
        }
        
        TimeLeft = gameManager.currentCountdownTime;

        if (TimeLeft < 30 && CPUInt == CPUGoal)
        {
            CPUAddPoints(100);
        }

        if (TimeLeft >= 30 && TimeLeft < 60 && CPUInt == CPUGoal)
        {
            CPUAddPoints(150);
        }

        if (TimeLeft >= 60 && TimeLeft < 90 && CPUInt == CPUGoal)
        {
            CPUAddPoints(200);
        }

        if (TimeLeft >= 90 && TimeLeft < 120 && CPUInt == CPUGoal)
        {
            CPUAddPoints(250);
        }

        if (TimeLeft >= 120 && CPUInt == CPUGoal)
        {
            CPUAddPoints(350);
        }
        
        Counter.text = CPUInt.ToString();
        
        if (canAct)
        {
            RespondInt = Random.Range(1, respondPassInt + 1);

            if (RespondInt >= respondPassInt)
            {
                CPUCalcInt = Random.Range(minRand, maxRand + 1);

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

                if (CPUInt > CPUGoal)
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

                else if (CPUInt == CPUGoal - 1 || CPUInt == CPUGoal + 1)
                {CPUInt = CPUGoal;}
            }
        }
    }

    private void CPUAddPoints(int Amount)
    {
        Score += Amount;
        scoreText.text = Score.ToString("00000");
        CPUScoreAddFlag = false;
    }
    
    public void CPULevelUp()
    {
        CPULevel += 1;
        currentLevelText.text = "Level " + CPULevel.ToString();

        if (CPULevel % 2 == 0)
        {respondPassInt -= 10;}

        if (CPULevel % 5 == 0)
        {maxRand += 2;}
    }
}
