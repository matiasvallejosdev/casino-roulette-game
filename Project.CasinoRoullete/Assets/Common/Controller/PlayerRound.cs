using System.Collections;
using Infrastructure;
using UnityEngine;
using ViewModel;
using UniRx;
using Commands;
using Components;
using Managers;

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
        }
        public void OnGameOpened() 
        {
            characterTable.currentTableCounter = 0;
            characterTable.currentTable.Clear();
            characterTable.currentChipSelected = GameObject.Find("Selected_0").GetComponent<ChipSelected>().chipData;
            characterTable.currentNumbers.Clear();
            //characterTable.lastTable.Clear();

            PlayerSystem.Instance.LoadRound();
            characterTable.OnActiveButton.OnNext(true);
        }
        
        // Table Controller
        public void DestroyLastChip()
        {
            if(characterTable.currentTableCounter > 0)
            {
                Debug.Log("Undo chip of the table!");
                Destroy(characterTable.currentTable[characterTable.currentTable.Count - 1].gameObject);
            }
        }

        private void DestroyChipTable(ChipGame ficha) 
        {
            if(ficha == null)
                return;
                
            try
            {
                PlayerSound.Instance.gameSound.OnSound.OnNext(2);
            }
            catch
            {
                Debug.Log("Player sound instance is not in scene!");
            }

            if(ficha.currentChipData.chipValue > 0 && characterTable.currentTableCounter > 0)
            {
                characterTable.characterMoney.DeleteChip(ficha.currentChipData.chipValue); // Delete money
                characterTable.currentTableCounter--;
            }   

            ficha.currentButton.SubstractCurrentOffset();
            characterTable.currentTable.Remove(ficha);
        }   

        public void ResetTable(bool destroyChips)
        {
            characterTable.characterMoney.characterBet.Value = 0;
            characterTable.currentTableCounter = 0;

            if(!destroyChips)
                return;

            foreach(ChipGame go in characterTable.currentTable)
            {
                Destroy(go.gameObject);
            }

            characterTable.OnActiveButton.OnNext(true);
            characterTable.currentTable.Clear();
        }
    }
}
