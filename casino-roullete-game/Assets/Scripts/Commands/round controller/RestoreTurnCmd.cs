using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using ViewModel;
using Controllers;

namespace Commands
{    
    public class RestoreTurnCmd : ICommand
    {
        private CharacterTable characterTable;

        public RestoreTurnCmd(CharacterTable characterTable)
        {
            this.characterTable = characterTable;
        }

        public void Execute()
        {
            PlayerSound.Instance.gameSound.OnSound.OnNext(PlayerSound.Instance.gameSound.audioReferences[5]);

            if (characterTable.currentTableCount > 0)
                return;

            RestorePreviousRound();
        }

        private void RestorePreviousRound()
        {
            Table table = new Table()
            {
                TableChips = characterTable.lastTable
            };

            characterTable.OnRestoreTable.OnNext(table);
        }
    }
}
