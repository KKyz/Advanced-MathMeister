[System.Serializable]
public class PlayerData
{
    public int bestLevel1;
    public int bestLevel2;
    public int bestLevel3;
    public float bestLevel4;
    public int bestTimeTrial;
    public PlayerData(PlayerRecords player)
    {
        bestLevel1 = player.bestLevel1;
        bestLevel2 = player.bestLevel2;
        bestLevel3 = player.bestLevel3;
        bestLevel4 = player.bestLevel4;
        bestTimeTrial = player.bestTimeTrial;
    }
}
