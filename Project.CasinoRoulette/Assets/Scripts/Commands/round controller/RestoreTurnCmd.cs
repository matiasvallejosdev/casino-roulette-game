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
            PlayerSound.Instance.gameSound.OnSound.OnNext(1);

            if(characterTable.currentTableCount > 0)
                return;
            
            // Execute this only if the table is before round finished
            Table table = new Table(){
                TableChips = characterTable.lastTable
            };
            PlayerRound.Instance.RestoreTable(table);
        }
    }
}
