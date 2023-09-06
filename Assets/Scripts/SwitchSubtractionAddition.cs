using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchSubtractionAddition : MonoBehaviour
{
    public static bool AdditionMode;
    public GameObject EquateButton;
    private int playerNum, goalNum, topboundNum, bottomboundNum;
    private Text operationText, equateButtonText;
    private AudioSource gaspSound;
    private GameManager gameManager;
    public AudioClip audienceGasp;
    public static bool haveGasped;
    public GameObject FlashObj;

    void Start()
    {
        haveGasped = false;
        gameManager = GameObject.Find("GameInterface").GetComponent<GameManager>();
        equateButtonText = EquateButton.GetComponent<Text>();
        operationText = gameObject.GetComponent<Text>();
        AdditionMode = true;
        gaspSound = gameObject.GetComponent<AudioSource>();
        gaspSound.Stop();
        FlashObj = GameObject.Find("FlashBG");
    }

    public void Update()
    {
        goalNum = gameManager.goalInt;
        playerNum = gameManager.playerNumber;

        topboundNum = goalNum + 5;
        bottomboundNum = goalNum - 5;

        if (playerNum < goalNum)
        {
           operationText.text = "+";
           equateButtonText.text = "Add";
           AdditionMode = true; 
        }

        if (playerNum > goalNum)
        {
           operationText.text = "-";
           equateButtonText.text = "Subtract";
           AdditionMode = false;
        }

        //Change BG color during event
        if (FlashObj != null)
        {
            if (playerNum <= topboundNum && playerNum > bottomboundNum && playerNum != goalNum)
            {
                StopCoroutine(RedFlash());
                StartCoroutine(GreenFlash());
            }

            else if (playerNum >= topboundNum && playerNum != goalNum)
            {
                StopCoroutine(GreenFlash());
                StartCoroutine(RedFlash());
            }

            else if (playerNum <= bottomboundNum || playerNum == goalNum)
            {
                StopCoroutine(GreenFlash());
                StopCoroutine(RedFlash());
                StartCoroutine(NullFlash());
            }
        }

        //Play gasping sound if very close
        if (playerNum == goalNum + 1 || playerNum == goalNum - 1)
        {   if (!haveGasped)
            {
                gaspSound.PlayOneShot(audienceGasp, 0.7f);
                haveGasped = true;
            }
        }
    }
    
    private IEnumerator RedFlash()
    {
        FlashObj.SetActive(true);
        LeanTween.color(FlashObj, Color.red, 1f).setDelay(1f);
        LeanTween.color(FlashObj, Color.clear, 1f).setDelay(1f);
        yield return null;
    }

    private IEnumerator GreenFlash()
    {
        FlashObj.SetActive(true);
        LeanTween.color(FlashObj, Color.green, 1f).setDelay(1f);
        LeanTween.color(FlashObj, Color.clear, 1f).setDelay(1f);
        yield return null;
    }

    private IEnumerator NullFlash()
    {
        FlashObj.SetActive(false);
        yield return null;
    }
}
