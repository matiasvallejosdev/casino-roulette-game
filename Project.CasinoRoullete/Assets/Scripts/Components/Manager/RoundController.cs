using System.Collections;
using Infrastructure;
using UnityEngine;
using ViewModel;

namespace Components
{
    public class RoundController : Singlenton<RoundController>
    {
        [Header("Round Controller")]
        public CharacterTable characterTable;
        private Round oldRound { get; set; }

        //public EventsRound.EventRoundState OnRoundChanged;
        //public EventsRound.EventApuestaChanged OnApuestaChanged;

        [Header("Variables")]
        public GameObject magnetDestroyer;
       // public HandlerFichas _scManejadorFichas = null;

        private void Start()
        {
            //OnRoundChanged.Invoke(cashTotal, cashTotal);
            //OnApuestaChanged.Invoke(0, 0, betActual);
        }

        public void OnRoundFinished(int newCash)
        {
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
        }
        
        /*public void OnGameClosed() 
        {
            Debug.Log("On Game Closed!");
            // Guarda el archivo
            saveRound(RoundFichas.ReturnFichasToSave(GameObject.FindGameObjectsWithTag("Fichas")));
        }
        public void OnGameSaved() 
        {
            // Guarda el archivo
            saveRound(RoundFichas.ReturnFichasToSave(GameObject.FindGameObjectsWithTag("Fichas")));
        }
        public void OnGameOpened() 
        {
            loadRound();

            if (oldRound.Length > 0) 
            {
                // Charge the old round
                RoundFichas.RestorePlayerRound(oldRound);
            }
        }*/
        
        // Undo the fichas in table one at a time
        public void UndoTable(GameObject ficha) 
        {
            /*GameObject btn = ficha.GetComponent<FichaDisplay>().currentButton.gameObject;
            if (ficha.GetComponent<FichaDisplay>().GetValueFicha() > 0)
            {
                int aux = ficha.GetComponent<FichaDisplay>().GetValueFicha();
                restarBet(aux);
                AddCash(aux);
                Destroy(ficha);
            }
            ResetButtons();*/
        }
        // Reset table and components
        
        public void ResetTable()
        {
            characterTable.OnActiveButton.OnNext(true);
            characterTable.characterMoney.SubstractBet(characterTable.characterMoney.currentBet.Value);
        }   
    }
}
