using UnityEngine;
using UnityEngine.UI;

public class ShowScore : MonoBehaviour
{

    public static int playerScore;
    public GameObject Timer, SparksParticle;
    private GameObject Sparks;
    private Text playerScoreText;
    public bool canReset;
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.Find("GameInterface").GetComponent<GameManager>();
        playerScoreText = gameObject.GetComponent<Text>();

        if (canReset)
        {
            playerScore = 0;
        } 
    }

    void Update()
    {
        if (gameManager.currentCountdownTime < 30 && gameManager.scoreAddFlag)
        {
            AddPoints(100);
        }

        if (gameManager.currentCountdownTime >= 30 && gameManager.currentCountdownTime < 60 && gameManager.scoreAddFlag)
        {
            AddPoints(150);
        }

        if (gameManager.currentCountdownTime >= 60 && gameManager.currentCountdownTime < 90 && gameManager.scoreAddFlag)
        {
            AddPoints(200);
        }

        if (gameManager.currentCountdownTime >= 90 && gameManager.currentCountdownTime < 120 && gameManager.scoreAddFlag)
        {
            AddPoints(250);
        }

        if (gameManager.currentCountdownTime >= 120 && gameManager.scoreAddFlag)
        {
            AddPoints(350);
        }

        playerScoreText.text = playerScore.ToString("00000");
        
    }

    public void AddPoints(int points)
    {
        Sparks = Instantiate(SparksParticle, gameObject.transform.position, Quaternion.identity);
        Sparks.transform.SetParent(null);
        playerScore += points;
        gameManager.scoreAddFlag = false;
        Destroy(Sparks, 2f);
    }
}
