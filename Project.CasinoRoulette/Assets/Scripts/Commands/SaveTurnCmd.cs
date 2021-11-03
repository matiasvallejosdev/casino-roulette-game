using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using ViewModel;
using Infrastructure;

namespace Commands
{    
    public class SaveTurnCmd : ICommand
    {
        private readonly CharacterTable characterTable;
        private ISaveRound saveRoundGateway;

        public SaveTurnCmd(CharacterTable characterTable, ISaveRound saveRoundGateway)
        {
            this.characterTable = characterTable;
            this.saveRoundGateway = saveRoundGateway;
        }

        public void Execute()
        {
            saveRoundGateway.RoundSequentialSave(characterTable)
                .Subscribe();
        }
    }
}
