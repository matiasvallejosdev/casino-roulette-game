using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using ViewModel;
using Infrastructure;

namespace Commands
{    
    public class SaveRoundCmd : ICommand
    {
        private readonly CharacterTable characterTable;
        private ISaveRound saveRoundGateway;

        public SaveRoundCmd(CharacterTable characterTable, ISaveRound saveRoundGateway)
        {
            this.characterTable = characterTable;
            this.saveRoundGateway = saveRoundGateway;
        }

        public void Execute()
        {
            Round roundData = GetRoundData();

            saveRoundGateway.RoundSequentialSave(roundData)
                .Subscribe();
        }

        private Round GetRoundData()
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
            return roundData;
        }
    }
}
