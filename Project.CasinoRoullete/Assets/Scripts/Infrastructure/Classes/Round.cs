using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure
{
    public class Round
    {
        public int idPlayer;
        public int cash;
        public ChipSave[] fichas;

        public Round(int idPlayer, int cash, ChipSave[] chips)
        {
            this.idPlayer = idPlayer;
            this.cash = cash;
            if (chips != null)
            {
                this.fichas = chips;
            }
        }
    }
}
