using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowSuperDuperSecretMessageButton : MonoBehaviour
{
    private int TimeTrialCheck, Level1Check, Level2Check, Level3Check, Level4Check;

    void Start()
    {
      TimeTrialCheck = PlayerPrefs.GetInt("TimeTrial", 1000);
      Level1Check = PlayerPrefs.GetInt("Level1", 1000);
      Level2Check = PlayerPrefs.GetInt("Level2", 7);
      Level3Check = PlayerPrefs.GetInt("Level3", 5);
      Level4Check = PlayerPrefs.GetInt("Level4", 4);

      if (TimeTrialCheck > 1500 && Level1Check > 1400 && Level2Check > 9 && Level3Check > 8 && Level4Check > 7)
      {
        gameObject.SetActive(true);
      }
      else
      {
          gameObject.SetActive(false);
      }
    }
}
