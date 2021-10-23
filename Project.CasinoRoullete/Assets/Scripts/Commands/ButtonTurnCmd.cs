using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using ViewModel;
using Components;

namespace Commands
{    
    public class ButtonTurnCmd : ICommand
    {
        private GameObject chipInstance;
        private GameObject chipsContainer;
        private CharacterTable characterTable;
        private ButtonTable buttonData;

        public ButtonTurnCmd(GameObject chipInstance, GameObject chipsContainer, CharacterTable characterTable, ButtonTable buttonData)
        {
            this.chipInstance = chipInstance;
            this.chipsContainer = chipsContainer;
            this.characterTable = characterTable;
            this.buttonData = buttonData;
        }

        public void Execute()
        {
            Chip chipData = characterTable.currentChipSelected;
            ChipGame chipGame = chipInstance.GetComponent<ChipGame>();
            
            // Find if is possible bet < totalWinner
            if(characterTable.characterMoney.CheckBetValue(chipData.chipValue))
            {
                Debug.Log("Bet is possible!");
                //SoundContoller.Instance.PlayFxSound(1);

                // Instiate New Chip Instance
                bool HasFichasOnTop = buttonData.currentChipsOnTop > 0;
                InstantiateFicha(chipGame, chipData, chipInstance, buttonData.currentSpritePivot, buttonData.AddCurrentOffset(), HasFichasOnTop);
                
                // Top controller
                buttonData.currentChipsOnTop++;
            }
            else
            {
                chipGame.DestroyMagnet();
                Debug.Log("Bet is not possible because the value of ficha is very high");
            }
        }    
        public void InstantiateFicha(ChipGame chipGame, Chip chipData, GameObject chipInstance, Vector2 spritePivot, Vector2 offsetPosition, bool fichasOnTop)
        {
            Debug.Log($"Instantiate chip {chipData.chipName} in the table {characterTable.tableName}");
            
            characterTable.currentTableCounter++;

            chipInstance.SetActive(true);
            chipInstance.name = $"{chipData.chipName}_{characterTable.currentTableCounter.ToString()}";
            
            if(buttonData.isPleno)
                chipInstance.transform.SetParent(chipsContainer.transform.GetChild(0));
            else 
                chipInstance.transform.SetParent(chipsContainer.transform.GetChild(1));
        
            if (fichasOnTop)
            {
                // Set position
                Vector2 position = spritePivot + offsetPosition;
                chipInstance.transform.position = position;

                chipGame.StartChip(chipData, position, buttonData);
            }
            else
            {
                // Set position
                Vector2 position = spritePivot;
                chipInstance.transform.position = position;

                chipGame.StartChip(chipData, position, buttonData);
            }

            characterTable.currentTable.Add(chipGame.gameObject);
        }
    }
}
