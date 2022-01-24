using System.Collections;
using Infrastructure;
using UnityEngine;
using ViewModel;
using UniRx;
using Commands;
using Components;
using Managers;
using System.Linq;
using System.IO;
using System;
using System.Threading.Tasks;

namespace Components
{
    public class PlayerRoundInput : MonoBehaviour
    {
        // Player round controller
        // Control table
        // Control iteractions
        public CharacterTable characterTable;
        public GameCmdFactory gameCmdFactory;

        private void Start()
        {
            characterTable.OnDestroyChip
                .Subscribe(DestroyChipTable)
                .AddTo(this);

            characterTable.OnDestroyLastChip
                .Subscribe(DestroyLastChip)
                .AddTo(this);
            
            characterTable.OnRound
                .Subscribe(OnRoundFinish)
                .AddTo(this);
            
            characterTable.OnResetGame
                .Subscribe(ResetTable)
                .AddTo(this);

            characterTable.OnRestoreTable
                .Subscribe(RestoreTable)
                .AddTo(this);
        }

        // Events
        public async void OnRoundFinish(bool isRound)
        {
            if(isRound)
                return;

            foreach(var item in characterTable.currentTableInGame)
            {
                characterTable.lastTable.Add(item);   
            }

            //characterTable.lastNumber = _lastNumber;
            characterTable.currentNumbers.Add(characterTable.lastNumber);

            await Task.Delay(TimeSpan.FromSeconds(2));
            ResetTable(false);
        }
        
        // Table Controller
        public void DestroyLastChip(bool value)
        {
            if(characterTable.currentTableCount > 0)
            {
                Debug.Log("Undo chip of the table!");
                Destroy(characterTable.currentTable[characterTable.currentTable.Count - 1].gameObject);
            }
        }

        private void DestroyChipTable(ChipGame ficha) 
        {
            // Destroy chip of the table

            if(ficha.currentChipData.chipValue > 0 && characterTable.currentTableCount > 0)
            {
                // Only if is called from a chip
                characterTable.characterMoney.DeleteChip(ficha.currentChipData.chipValue); // Delete money
                characterTable.currentTableCount--;
                characterTable.currentTableInGame.RemoveAt(characterTable.currentTableInGame.Count() - 1);     
            }   

            ficha.currentButton.SubstractCurrentOffset();

            characterTable.currentTable.Remove(ficha);
        }   

        public void ResetTable(bool destroyChips)
        {
            characterTable.characterMoney.characterBet.Value = 0;
            characterTable.currentTableCount = 0;
            characterTable.currentTableInGame.Clear();

            if(!destroyChips)
                return;

            foreach(ChipGame go in characterTable.currentTable)
            {
                Destroy(go.gameObject);
            }

            characterTable.currentTableActive.Value = true;
            characterTable.currentTable.Clear();
            characterTable.lastTable.Clear();
        }

        public void RestoreTable(Table table)
        {
            if(!characterTable.currentTableActive.Value)
                return;
            
            Debug.Log($"Loading current player table with {table.TableChips.Count()} chips");

            foreach(TableChips buttonChip in table.TableChips)
            {
                GameObject buttonInstance = GameObject.Find(buttonChip.idButton);
                GameObject chipInstance = Instantiate(characterTable.chipPrefab);
                chipInstance.SetActive(false);
                GameObject chipContainer = GameObject.FindGameObjectWithTag("ChipContainer");
                ButtonTable buttonData = buttonInstance.GetComponent<ButtonTableInput>().buttonData;
                Chip chipData = characterTable.chipData.Where(chip => chip.chipkey.ToString() == buttonChip.idChip).First();

                gameCmdFactory.ButtonTableTurn(buttonInstance, chipInstance, chipContainer, characterTable, buttonData, chipData).Execute();
            }
        }
    }
}
