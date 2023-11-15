using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Simply checks if all conditions are met to enable a button. Nothing more to examine here
public class ShowSuperDuperSecretMessageButton : MonoBehaviour
{
    private float TimeTrialCheck, Level1Check, Level2Check, Level3Check, Level4Check, Level5Check;
    public AudioClip title, completeTitle;
    
    //Simply checks if all conditions are met to enable a button. Nothing more to examine here

    void Start()
    {
        var player = SaveSystem.LoadPlayer();
        
        TimeTrialCheck = player.bestTimeTrial;
        Level1Check = player.bestLevel1;
        Level2Check = player.bestLevel2;
        Level3Check = player.bestLevel3;
        Level4Check = player.bestLevel4;

        var canvas = GameObject.Find("Canvas").GetComponent<AudioSource>();

        if (TimeTrialCheck > 1500 && Level1Check > 1400 && Level2Check > 9 && Level3Check > 8 && Level4Check > 140)
        {
            gameObject.SetActive(true);
            canvas.clip = completeTitle;
            canvas.Play();
        }
        else
        {
            canvas.clip = title;
            canvas.Play();
            gameObject.SetActive(false);
          
        }
    }
}
