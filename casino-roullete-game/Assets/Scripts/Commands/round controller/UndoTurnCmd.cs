using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using ViewModel;
using Controllers;

namespace Commands
{    
    public class UndoTurnCmd : ICommand
    {
        private CharacterTable characterTable;

        public UndoTurnCmd(CharacterTable characterTable)
        {
            this.characterTable = characterTable;
        }

        public void Execute()
        {
            PlayerSound.Instance.gameSound.OnSound.OnNext(PlayerSound.Instance.gameSound.audioReferences[4]);
            characterTable.OnDestroyLastChip.OnNext(true);
        }
    }
}
