using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDisableBG : MonoBehaviour
{
    private int userInt;
    
    void Awake()
    {
        userInt = PlayerPrefs.GetInt("BGActive", 0);

        if (userInt == 0)
        {
            if (gameObject.transform.childCount >= 1)
            {
                for (int i = 0; i <= gameObject.transform.childCount - 1; i++)
                {
                    gameObject.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
        }

        if (userInt == 1)
        {
            if (gameObject.transform.childCount >= 1)
            {
                for (int i = 0; i <= gameObject.transform.childCount - 1; i++)
                {
                    gameObject.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }    
    }
}
