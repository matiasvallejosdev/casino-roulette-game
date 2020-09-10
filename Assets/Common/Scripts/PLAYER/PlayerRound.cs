
[System.Serializable]
public class PlayerRound
{
    public int cash;
    public FichasSave[] fichas;

    public PlayerRound(int cash,FichasSave[] fichas, bool editRound)
    {
        this.cash = cash;
        if (editRound)
        {
            this.fichas = fichas;
        }
    }
}
