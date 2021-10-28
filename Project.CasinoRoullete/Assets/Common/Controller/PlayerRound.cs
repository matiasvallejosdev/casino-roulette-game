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
        public GameCmdFactory gameCmdFactory;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            characterTable.OnDestroyChip
                .Subscribe(DestroyChipTable)
                .AddTo(this);
            
            OnGameOpened();
        }
        
        // Events
        public void OnRoundFinished(int newCash)
        {
        /*
            // Sum Cash
            if (newCash > 0)
            {
                characterTable.characterMoney.AddCash(newCash);
                //CanvasUI.Instance.turnWinOrLost("YOU WIN!", newCash.ToString(), true, 1);
            } else if (newCash < 0)
            {
                characterTable.characterMoney.SubstractCash(newCash * -1);
                //CanvasUI.Instance.turnWinOrLost("YOU LOST!", newCash.ToString(), false, 1);
            }
            // Delete fichas
            PaymentController.Instance.deleteFichasInPayment();
            // Save Rounded
            PaymentController.Instance.saveRounded();
            // Apuesta to zero
            characterTable.characterMoney.SubstractBet(characterTable.characterMoney.currentBet.Value);
        */
        }
        public void OnGameClosed() 
        {
            Debug.Log("Game have been closed! Files was saved!");  

            PlayerSystem.Instance.characterTable.OnSaveGame
            .OnNext(true);

            ResetTable();
            characterTable.currentNumbers.Clear();
        }
        public void OnGameOpened() 
        {
            characterTable.currentTableCounter = 0;
            characterTable.currentTable.Clear();
            characterTable.currentChipSelected = GameObject.Find("Selected_0").GetComponent<ChipSelected>().chipData;

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
            if(characterTable.currentTableCounter <= 0)
                return;

            if(ficha.currentChipData.chipValue > 0)
            {
                characterTable.characterMoney.DeleteChip(ficha.currentChipData.chipValue);
            }   

            ficha.currentButton.SubstractCurrentOffset();
            characterTable.currentTable.Remove(ficha);
            characterTable.currentTableCounter--;

            PlayerSound.Instance.gameSound.OnSound.OnNext(2);
        }   
        public void ResetTable()
        {
            characterTable.OnActiveButton.OnNext(true);
            characterTable.characterMoney.characterBet.Value = 0;
            characterTable.currentTableCounter = 0;

            foreach(ChipGame go in characterTable.currentTable)
            {
                Destroy(go.gameObject);
            }

            characterTable.currentTable.Clear();
        }
    }
}
