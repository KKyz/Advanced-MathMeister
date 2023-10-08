using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowBestScore : MonoBehaviour
{
    private Text bestScoreCounter;
    public bestScore thisBestScore;
    public GameManager gameManager;
    
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
        var save = SaveSystem.LoadPlayer();
        
        bestScoreCounter = gameObject.GetComponent<Text>();

        switch (thisBestScore)
        {
           case bestScore.TimeTrial:
               bestScoreCounter.text = save.bestTimeTrial.ToString("00000");
               break;
           case bestScore.Level1:
               bestScoreCounter.text = save.bestLevel1.ToString("00000");
               break;
           case bestScore.Level2:
               bestScoreCounter.text = save.bestLevel2.ToString("00000");
               break;
           case bestScore.Level3:
               bestScoreCounter.text = save.bestLevel3.ToString("00000");
               break;
           case bestScore.Level4:
               bestScoreCounter.text = save.bestLevel4.ToString("00000");
               break;
           case bestScore.Level5:
               bestScoreCounter.text = save.bestLevel5.ToString("00000");
               break;
        }
    }
}
