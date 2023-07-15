using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchSubtractionAddition : MonoBehaviour
{
    public static bool AdditionMode;
    public GameObject Camera, Canvas, EquateButton;
    private int playerNum, goalNum, topboundNum, bottomboundNum;
    private Text operationText, equateButtonText;
    private AudioSource gaspSound;
    public AudioClip audienceGasp;
    public static bool haveGasped;

    void Start()
    {
        haveGasped = false;
        equateButtonText = EquateButton.GetComponent<Text>();
        operationText = gameObject.GetComponent<Text>();
        AdditionMode = true;
        gaspSound = gameObject.GetComponent<AudioSource>();
        gaspSound.Stop();
    }

    public void Update()
    {
        goalNum = SelectTargetNumber.goalInt;
        playerNum = Canvas.GetComponent<UIFunctions>().playerNumber;

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

        if (playerNum <= topboundNum && playerNum > bottomboundNum && playerNum != goalNum)
        {
            Camera.GetComponent<Animator>().Play("CameraGreenAlert"); 
        }

        else if (playerNum >= topboundNum && playerNum != goalNum)
        {
            Camera.GetComponent<Animator>().Play("CameraRedAlert");
        }

        else if (playerNum <= bottomboundNum || playerNum == goalNum)
        {
            Camera.GetComponent<Animator>().Play("CameraIdle");
        }
        
        if (playerNum == goalNum + 1 || playerNum == goalNum - 1)
        {   if (!haveGasped)
            {
                gaspSound.PlayOneShot(audienceGasp, 0.7f);
                haveGasped = true;
            }
        } 
    }
}
