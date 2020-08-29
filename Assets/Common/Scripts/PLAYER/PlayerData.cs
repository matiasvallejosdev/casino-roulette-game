
[System.Serializable]
public class PlayerData 
{
    public int id;
    public int cash;
    public PlayerRound round;

    public PlayerData(int id, int[] cash, PlayerRound round) 
    {
        this.id = id;
        this.cash = cash[0];
        this.round = round;
    }
}
