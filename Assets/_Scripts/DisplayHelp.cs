using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHelp : MonoBehaviour
{
    public GameObject CanvasMover;

    public void ActivateScreen(GameObject HelpScreen)
    {
        CanvasMover.transform.GetChild(0).GetComponent<Animator>().Play("ButtonMoveIn");
        HelpScreen.GetComponent<Animator>().Play("CanvasMoveIn");

        for (int i = 0; i <= transform.childCount - 1; i++)
        {
            transform.GetChild(i).GetComponent<Animator>().Play("TopicButtonsMoveOut");
        }
    }

    public void DeactivateScreen()
    {
        CanvasMover.transform.GetChild(0).GetComponent<Animator>().Play("ButtonMoveOut");
        
        for (int i = 1; i <= CanvasMover.transform.childCount - 1; i++)
        {
            CanvasMover.transform.GetChild(i).GetComponent<Animator>().Play("CanvasMoveOut");
        }

        for (int i = 0; i <= transform.childCount - 1; i++)
        {
            transform.GetChild(i).GetComponent<Animator>().Play("TopicButtonsMoveIn");
        }
    }

}
