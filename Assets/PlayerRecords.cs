using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRecords : MonoBehaviour
{
    public int bestLevel1;
    public int bestLevel2;
    public int bestLevel3;
    public int bestLevel4;
    public float bestLevel5;
    public float bestTimeTrial;
    
    // Start is called before the first frame update
    void Start()
    {
        SaveSystem.SaveCheck(this);
        
        PlayerData data = SaveSystem.LoadPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
