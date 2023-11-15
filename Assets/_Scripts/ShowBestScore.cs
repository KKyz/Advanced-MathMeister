using UnityEngine;
using UnityEngine.UI;

public class ShowBestScore : MonoBehaviour
{
    private Text bestScoreCounter;
    public bestScore thisBestScore;

    public enum bestScore
    {
        TimeTrial, 
        Level1,
        Level2,
        Level3,
        Level4
    }

    // Start is called before the first frame update
    void Start()
    {
        var save = GameObject.Find("PlayerRecords").GetComponent<PlayerRecords>();
        
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
        }
    }
}
