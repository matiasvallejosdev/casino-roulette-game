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

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            characterTable.OnDestroyChip
                .Subscribe(DestroyChipTable)
                .AddTo(this);
            
            characterTable.OnRound
                .Subscribe(OnRoundFinish)
                .AddTo(this);
            
            OnGameOpened();
        }
        
        // Events
        public void OnPayment(int value)
        {
            characterTable.characterMoney.currentPayment.Value = value;
            characterTable.characterMoney.RoundFinish(value);
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
        public void OnGameOpened() 
        {
            // Update round parameters
            characterTable.currentTableCount = 0;
            characterTable.currentTable.Clear();
            characterTable.currentChipSelected = characterTable.chipData.Where(chip => chip.chipkey == KeyFicha.Chip10).First();
            characterTable.currentNumbers.Clear();
            characterTable.currentTableInGame.Clear();

            PlayerSystem.Instance.LoadRound();
            characterTable.OnActiveButton.OnNext(true);

            characterTable.lastNumber = 0;
            characterTable.lastTable.Clear();
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
            }   

            ficha.currentButton.SubstractCurrentOffset();

            characterTable.currentTableInGame.RemoveAt(characterTable.currentTableInGame.Count() - 1);
            characterTable.currentTable.Remove(ficha);
        }   

        public void ResetTable(bool destroyChips)
        {
            characterTable.characterMoney.characterBet.Value = 0;
            characterTable.currentTableCount = 0;

            if(!destroyChips)
                return;

            foreach(ChipGame go in characterTable.currentTable)
            {
                Destroy(go.gameObject);
            }

            characterTable.OnActiveButton.OnNext(true);
            characterTable.currentTable.Clear();
            characterTable.currentTableInGame.Clear();
            characterTable.lastTable.Clear();
        }

        public void RestoreTable(Table table)
        {
            foreach(ButtonChip buttonChip in table.buttonChips)
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
