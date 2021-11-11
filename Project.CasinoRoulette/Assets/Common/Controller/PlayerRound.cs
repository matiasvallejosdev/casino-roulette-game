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

namespace Controllers
{
    public class PlayerRound : Singlenton<PlayerRound>
    {
        // Player round controller
        // Control table
        // Control iteractions
        public CharacterTable characterTable;
        public GameRoullete gameRoullete;
        public GameCmdFactory gameCmdFactory;

        public int _lastNumber = 0;
        private bool _isTableActive;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            characterTable.OnDestroyChip
                .Subscribe(DestroyChipTable)
                .AddTo(this);
            
            characterTable.OnRound
                .Subscribe(OnRoundFinish)
                .AddTo(this);
        }

        // Events
        public void OnPayment(int value)
        {
            characterTable.characterMoney.currentPayment.Value = value;
            characterTable.characterMoney.PaymentSystem(value);
        }
        public void OnRoundFinish(bool isRound)
        {
            if(isRound)
                return;

            foreach(var item in characterTable.currentTableInGame)
            {
                characterTable.lastTable.Add(item);   
            }
            characterTable.lastNumber = _lastNumber;
            characterTable.currentNumbers.Add(characterTable.lastNumber);
            ResetTable(false);
        }
        public void OnGameClosed() 
        {
            Debug.Log("Game have been closed! Files was saved!");  

            PlayerSystem.Instance.characterTable.OnSaveGame
                .OnNext(true);

            ResetTable(true);

            characterTable.currentNumbers.Clear();
            characterTable.currentTableInGame.Clear();
        }
        public async Task OnGameOpened() 
        { 
            // Update round parameters
            characterTable.currentTableActive.Value = false; 
            characterTable.currentTableCount = 0;
            characterTable.currentTable.Clear();
            characterTable.currentNumbers.Clear();
            characterTable.currentTableInGame.Clear();


            characterTable.lastNumber = 0;
            characterTable.lastTable.Clear();
            
            await Task.Delay(TimeSpan.FromSeconds(2));

            characterTable.currentTableActive.Value = true; 
            characterTable.currentChipSelected = characterTable.chipData.Where(chip => chip.chipkey == KeyFicha.Chip10).First();

            await Task.Yield();
        }
        // Table Controller
        public void DestroyLastChip()
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
