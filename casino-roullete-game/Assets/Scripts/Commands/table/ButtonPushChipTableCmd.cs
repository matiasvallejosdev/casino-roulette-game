using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using ViewModel;
using Components;
using Controllers;

namespace Commands
{
    public class ButtonPushChipTableCmd : ICommand
    {
        private TableManager tableController;
        private GameObject chipInstance;
        private ButtonTable buttonData;
        private Chip chipData;

        public ButtonPushChipTableCmd(TableManager tableController, GameObject chipInstance, ButtonTable buttonData)
        {
            this.tableController = tableController;
            this.chipInstance = chipInstance;
            this.buttonData = buttonData;
            this.chipData = tableController.table.characterTable.currentChipSelected;
        }

        public void Execute()
        {
            ChipGame chipGame = chipInstance.GetComponent<ChipGame>();

            // Detect if is all in one
            if(chipData.chipkey == KeyFicha.ChipAll)
            {
                chipData.chipValue = tableController.table.characterTable.characterMoney.characterMoney.Value;
            }
            
            // Find if is possible bet < totalWinner
            if(tableController.table.characterTable.characterMoney.CheckBetValue(chipData.chipValue))
            {
                PlayerSound.Instance.gameSound.OnSound.OnNext(PlayerSound.Instance.gameSound.audioReferences[3]);

                bool _hasFichasOnTop = buttonData.currentChipsOnTop > 0;
                tableController.tableInteracatable.PushChipInButton(tableController.table.characterTable, buttonData, chipGame, chipData, chipInstance, buttonData.currentSpritePivot, buttonData.GetOffset(), _hasFichasOnTop);
                
                buttonData.currentChipsOnTop++;
            }
            else
            {
                chipGame.DestroyChip();
                Debug.Log("Bet is not possible because the value of ficha is very high");
            }
        }    

    }
}
