using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using ViewModel;
using Infrastructure;

namespace Commands
{    
    public class LoadTurnCmd : ICommand
    {
        private CharacterTable characterTable;
        private ISaveRound loadRoundGateway;

        public LoadTurnCmd(CharacterTable characterTable, ISaveRound loadRoundGateway)
        {
            this.characterTable = characterTable;
            this.loadRoundGateway = loadRoundGateway;
        }

        public void Execute()
        {
            loadRoundGateway.RoundSequentialLoad(characterTable)
                .Do(_ => characterTable.characterMoney.characterMoney.Value = loadRoundGateway.roundData.playerMoney)
                .Subscribe();
        }
    }
}
