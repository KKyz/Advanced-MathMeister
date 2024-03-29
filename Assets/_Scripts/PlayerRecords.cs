using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRecords : MonoBehaviour
{
    public int bestLevel1;
    public int bestLevel2;
    public int bestLevel3;
    public float bestLevel4;
    public int bestTimeTrial;
    
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SaveSystem.ClearPlayer();

        //Set initial scores if no save is present
        if (!SaveSystem.SaveCheck(this))
        {
            bestTimeTrial = 500;
            bestLevel1 = 400;
            bestLevel2 = 600;
            bestLevel3 = 550;
            bestLevel4 = 120f;
            
            SavePlayer();
        }
        
        LoadPlayer();
    }

    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        bestLevel1 = data.bestLevel1;
        bestLevel2 = data.bestLevel2;
        bestLevel3 = data.bestLevel3;
        bestLevel4 = data.bestLevel4;
        bestTimeTrial = data.bestTimeTrial;

    }
}
