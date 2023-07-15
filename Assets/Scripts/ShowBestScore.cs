using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowBestScore : MonoBehaviour
{
    public static int displayBestScore;
    private Text BestScoreCounter;
    public string bestScore;

    // Start is called before the first frame update
    void Start()
    {
        BestScoreCounter = gameObject.GetComponent<Text>();
        displayBestScore = PlayerPrefs.GetInt(bestScore, 1000);
        BestScoreCounter.text = displayBestScore.ToString("00000");
    }
}
