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
            Table table = new Table()
            {
                TableChips = characterTable.currentTableInGame
            };

            string json = JsonUtility.ToJson(table);
            
            Round roundData = new Round()
            {
                idPlayer = characterTable.tableName,
                playerMoney = characterTable.characterMoney.characterMoney.Value + characterTable.characterMoney.characterBet.Value,
                playerTable = json
            };
            
            saveRoundGateway.RoundSequentialSave(roundData)
                .Subscribe();
        }
    }
}
