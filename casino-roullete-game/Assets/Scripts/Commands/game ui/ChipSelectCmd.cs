using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using ViewModel;
using Controllers;

namespace Commands
{    
    public class ChipSelectCmd : ICommand
    {
        private CharacterTable characterTable;
        private Chip chipData;

        public ChipSelectCmd(CharacterTable characterTable, Chip chipData)
        {
            this.characterTable = characterTable;
            this.chipData = chipData;
        }

        public void Execute()
        {
            PlayerSound.Instance.gameSound.OnSound.OnNext(PlayerSound.Instance.gameSound.audioReferences[5]);
            characterTable.currentChipSelected = chipData;
        }
    }
}
