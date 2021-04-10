
[System.Serializable]
public class PlayerData
{
    public int id;
    public int cash;
    public FichasSave[] fichas;

    public PlayerData(int id, FichasSave[] fichas, int cash, bool editRound) 
    {
        this.id = id;
        this.cash = cash;
        if (editRound)
        {
            this.fichas = fichas;
        }
    }
}
