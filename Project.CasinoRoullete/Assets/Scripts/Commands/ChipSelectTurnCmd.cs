using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using ViewModel;

namespace Commands
{    
    public class ChipSelectTurnCmd : ICommand
    {
        private CharacterTable characterTable;
        private Chip chipData;

        public ChipSelectTurnCmd(CharacterTable characterTable, Chip chipData)
        {
            this.characterTable = characterTable;
            this.chipData = chipData;
        }

        public void Execute()
        {
            //Debug.Log($"New selection input in position: {chipData.name}");
            characterTable.currentChipSelected = chipData;
        }
    }
}
