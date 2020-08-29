using System;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class PlayerRound
{
    [SerializeField]
    public FichasSave[] fichas;

    public PlayerRound(FichasSave[] fichas, bool editRound)
    {
        if (editRound)
        {
            this.fichas = fichas;
        }
    }
}
