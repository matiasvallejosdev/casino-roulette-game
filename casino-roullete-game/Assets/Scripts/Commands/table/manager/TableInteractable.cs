
using Commands;
using Components;
using UnityEngine;
using ViewModel;

public class TableInteractable : ITableInteractable
{
    public void PushChipInButton(CharacterTable characterTable, ButtonTable buttonData, ChipGame chipGame, Chip chipData, GameObject chipInstance, Vector2 spritePivot, Vector2 offsetPosition, bool fichasOnTop)
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