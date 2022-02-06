using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using ViewModel;
using Components;
using Controllers;

namespace Commands
{    
    public class ButtonTurnCmd : ICommand
    {
        private GameObject chipInstance;
        private CharacterTable characterTable;
        private ButtonTable buttonData;
        private Chip chipData;

        public ButtonTurnCmd(CharacterTable characterTable, GameObject chipInstance, ButtonTable buttonData)
        {
            this.chipInstance = chipInstance;
            this.characterTable = characterTable;
            this.buttonData = buttonData;
            this.chipData = characterTable.currentChipSelected;
        }

        public void Execute()
        {
            ChipGame chipGame = chipInstance.GetComponent<ChipGame>();

            // Detect if is all in one
            if(chipData.chipkey == KeyFicha.ChipAll)
            {
                chipData.chipValue = characterTable.characterMoney.characterMoney.Value;
            }
            
            // Find if is possible bet < totalWinner
            if(characterTable.characterMoney.CheckBetValue(chipData.chipValue))
            {
                PlayerSound.Instance.gameSound.OnSound.OnNext(PlayerSound.Instance.gameSound.audioReferences[3]);

                // Instiate New Chip Instance
                bool _hasFichasOnTop = buttonData.currentChipsOnTop > 0;
                InstantiateFicha(chipGame, chipData, chipInstance, buttonData.currentSpritePivot, buttonData.GetOffset(), _hasFichasOnTop);
                
                // Top controller
                buttonData.currentChipsOnTop++;
            }
            else
            {
                chipGame.DestroyChip();
                Debug.Log("Bet is not possible because the value of ficha is very high");
            }
        }    
        public void InstantiateFicha(ChipGame chipGame, Chip chipData, GameObject chipInstance, Vector2 spritePivot, Vector2 offsetPosition, bool fichasOnTop)
        {
            characterTable.currentTableCount++;

            chipInstance.SetActive(true);
            chipInstance.name = $"{chipData.chipName}_{characterTable.currentTableCount.ToString()}";
            
            if(buttonData.isPleno)
                chipInstance.transform.SetParent(chipGame.chipsContainer.transform.GetChild(0));
            else 
                chipInstance.transform.SetParent(chipGame.chipsContainer.transform.GetChild(1));
            
            Vector2 position = Vector2.zero;

            if (fichasOnTop)
            {
                position = spritePivot + offsetPosition;
                chipInstance.transform.position = position;
            }
            else
            {
                position = spritePivot;
                chipInstance.transform.position = position;
            }
            
            chipGame._chipRuntime.StartChip(chipData, position, buttonData, chipGame.spriteRenderer);
            
            characterTable.currentTable.Add(chipGame);

            TableChips buttonChip = new TableChips(){
                idButton = buttonData.buttonName, 
                idChip = chipData.chipkey.ToString()
            };

            characterTable.currentTableInGame.Add(buttonChip);
        }
    }
}
