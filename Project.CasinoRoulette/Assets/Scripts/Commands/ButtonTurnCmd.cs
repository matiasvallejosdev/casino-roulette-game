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
        private GameObject buttonInstance;
        private GameObject chipInstance;
        private GameObject chipsContainer;
        private CharacterTable characterTable;
        private ButtonTable buttonData;
        private Chip chipData;

        public ButtonTurnCmd(GameObject buttonInstance, GameObject chipInstance, GameObject chipsContainer, CharacterTable characterTable, ButtonTable buttonData, Chip chipData)
        {
            this.buttonInstance = buttonInstance;
            this.chipInstance = chipInstance;
            this.chipsContainer = chipsContainer;
            this.characterTable = characterTable;
            this.buttonData = buttonData;
            this.chipData = chipData;
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
                Debug.Log("Bet is possible!");
                PlayerSound.Instance.gameSound.OnSound.OnNext(1);

                // Instiate New Chip Instance
                bool HasFichasOnTop = buttonData.currentChipsOnTop > 0;
                InstantiateFicha(chipGame, chipData, chipInstance, buttonData.currentSpritePivot, buttonData.AddCurrentOffset(), HasFichasOnTop);
                
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
            //Debug.Log($"Instantiate chip {chipData.chipName} in the table {characterTable.tableName}");
            
            characterTable.currentTableCount++;

            chipInstance.SetActive(true);
            chipInstance.name = $"{chipData.chipName}_{characterTable.currentTableCount.ToString()}";
            
            if(buttonData.isPleno)
                chipInstance.transform.SetParent(chipsContainer.transform.GetChild(0));
            else 
                chipInstance.transform.SetParent(chipsContainer.transform.GetChild(1));
            
            Vector2 position = Vector2.zero;

            if (fichasOnTop)
            {
                // Set position
                position = spritePivot + offsetPosition;
                chipInstance.transform.position = position;
            }
            else
            {
                // Set position
                position = spritePivot;
                chipInstance.transform.position = position;

            }
            
            chipGame.StartChip(chipData, position, buttonData);
            
            characterTable.currentTable.Add(chipGame);

            TableChips buttonChip = new TableChips(){
                idButton = buttonInstance.name, 
                idChip = chipData.chipkey.ToString()
            };

            characterTable.currentTableInGame.Add(buttonChip);
        }
    }
}
