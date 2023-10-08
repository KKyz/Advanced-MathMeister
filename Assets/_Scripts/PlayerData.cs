[System.Serializable]
public class PlayerData
{
    public int bestLevel1;
    public int bestLevel2;
    public int bestLevel3;
    public int bestLevel4;
    public float bestLevel5;
    public float bestTimeTrial;
    public PlayerData(PlayerRecords player)
    {
        bestLevel1 = player.bestLevel1;
        bestLevel2 = player.bestLevel2;
        bestLevel3 = player.bestLevel3;
        bestLevel4 = player.bestLevel4;
        bestLevel5 = player.bestLevel5;
        bestTimeTrial = player.bestTimeTrial;

    }
}
