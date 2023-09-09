using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowBestScore : MonoBehaviour
{
    public static int displayBestScore;
    private Text bestScoreCounter;
    public bestScore thisBestScore;
    
    public enum bestScore
    {
        TimeTrial, 
        Level1,
        Level2,
        Level3,
        Level4,
        Level5
    }

    // Start is called before the first frame update
    void Start()
    {
        bestScoreCounter = gameObject.GetComponent<Text>();
        displayBestScore = PlayerPrefs.GetInt(thisBestScore.ToString(), 1000);
        bestScoreCounter.text = displayBestScore.ToString("00000");
    }
}
