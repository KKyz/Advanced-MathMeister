using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountMoves : MonoBehaviour
{
    public static int moveCounter;
    public static int levelCounter;
    public Text levelCounterText, victoryLevelCounterText;
    private Text moveCounterText;
    private GameManager gameManager;
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameInterface").GetComponent<GameManager>();
        levelCounter = 1;
        moveCounter = 7;
        moveCounterText = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        levelCounterText.text = "Level "+ levelCounter.ToString();
        victoryLevelCounterText.text = "Level "+ levelCounter.ToString();
        moveCounterText.text = moveCounter.ToString();
        if (gameManager.equationResetFlag)
        {
            moveCounter -= 1;
            gameManager.equationResetFlag = false;
        }
    }
}
