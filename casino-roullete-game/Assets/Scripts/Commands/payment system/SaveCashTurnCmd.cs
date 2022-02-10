using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using ViewModel;
using Infrastructure;
using System;

namespace Commands
{    
    public class SaveCashTurnCmd : ICommand
    {
        private CharacterCmdFactory characterCmdFactory;
        private CharacterTable characterTable;
        private int payment;
        private ISaveRound saveRoundGateway;

        public SaveCashTurnCmd(CharacterCmdFactory characterCmdFactory, CharacterTable characterTable, int payment, ISaveRound saveRoundGateway)
        {
            this.characterCmdFactory = characterCmdFactory;
            this.characterTable = characterTable;
            this.payment = payment;
            this.saveRoundGateway = saveRoundGateway;
        }

        public void Execute()
        {
            // First Load Player Table
            saveRoundGateway.RoundSequentialLoad()
                .Do(_ => characterTable.characterMoney.characterMoney.Value = saveRoundGateway.roundData.playerMoney)
                .Do(_ => UpdateTable(payment))
                .Do(_ => characterCmdFactory.SavePlayer(characterTable).Execute())
                .Do(_ => characterTable.currentTableInGame.Clear())
                .Subscribe();
        }

        private void UpdateTable(int payment)
        {
            Table tableLoaded = JsonUtility.FromJson<Table>(saveRoundGateway.roundData.playerTable);
            characterTable.currentTableInGame = tableLoaded.TableChips;
            characterTable.characterMoney.PaymentSystem(payment);
        }
    }
}
