using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowBestMove : MonoBehaviour
{
    [HideInInspector]
    public int displayBestLevel;
    private Text BestLevelCounter;
    public int defaultBestLevel;
    public string playerPref = "Level2";

    void Start()
    {
        BestLevelCounter = gameObject.GetComponent<Text>();
        displayBestLevel = PlayerPrefs.GetInt(playerPref, defaultBestLevel);
        BestLevelCounter.text = "Level " +displayBestLevel.ToString();
    }
}
