using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    public class ButtonTableFxLongPressDisplay : MonoBehaviour
    {
        /*
        private TableController _tableController;
        public backNumber_controller backNumberController;

        private GameObject[] _numberMatchedEffects;

        // Start is called before the first frame update
        void Start()
        {
            _tableController = GetComponent<TableController>();
            backNumberController = GameObject.Find("BackNumberHUD").GetComponent<backNumber_controller>();
        }
        
        /// <summary>
        /// Find the effect of the new number and set the button white.
        /// </summary>
        /// <param name="num"></param>
        public void SetEffectTableToNewNumber(int num)
        {
            _numberMatchedEffects = _tableController.FindEffectNumbers(num);
            
            // Set the new number in HUD
            backNumberController.nuevoNumeroHUD(num);

            // Espera unos segundos y apaga sprites
            StartCoroutine(WaitSecondsToElimanteEffect(16));
        }
        
        /// <summary>
        /// Wait and set off the sprite
        /// </summary>
        /// <param name="seg"></param>
        /// <returns></returns>
        IEnumerator WaitSecondsToElimanteEffect(int seg)
        {
            yield return new WaitForSeconds(seg);
            
            foreach(GameObject number in _numberMatchedEffects)
            {
                ButtonDisplay sc_aux = number.GetComponent<ButtonDisplay>();
                sc_aux.FxFicha(false);
            }
            // Elimina las fichas en mesa
            RoundController.Instance.MagnetDestroyerFichas(3.5f);
        }
        
        public IEnumerator TititlarNumberWinner(ButtonDisplay sc)
        {
            sc.FxNumberWinner(true);
            yield return new WaitForSeconds(0.5f);
            sc.FxNumberWinner(false);
            yield return new WaitForSeconds(0.5f);
            sc.FxNumberWinner(true);
            yield return new WaitForSeconds(0.5f);
            sc.FxNumberWinner(false);
            yield return new WaitForSeconds(0.5f);
            sc.FxNumberWinner(true);
            yield return new WaitForSeconds(0.5f);
            sc.FxNumberWinner(false);
            yield return new WaitForSeconds(0.5f);
            sc.FxNumberWinner(true);
            yield return new WaitForSeconds(0.5f);
            sc.FxNumberWinner(false);
            yield return new WaitForSeconds(0.5f);
            sc.FxNumberWinner(true);
            yield return new WaitForSeconds(0.5f);
            sc.FxNumberWinner(false);
            yield return new WaitForSeconds(0.5f);
            sc.FxNumberWinner(true);
            yield return new WaitForSeconds(0.5f);
            sc.FxNumberWinner(false);
            yield return new WaitForSeconds(0.5f);
            sc.FxNumberWinner(true);
            yield return new WaitForSeconds(0.5f);
            sc.FxNumberWinner(false);
            yield return new WaitForSeconds(0.5f);
            sc.FxNumberWinner(true);
            yield return new WaitForSeconds(0.5f);
            sc.FxNumberWinner(false);
            yield return new WaitForSeconds(0.5f);
            sc.FxNumberWinner(false);
            yield return new WaitForSeconds(0.5f);
            sc.FxNumberWinner(true);
            yield return new WaitForSeconds(0.5f);
            sc.FxNumberWinner(false);
            yield return new WaitForSeconds(0.5f);
            sc.FxNumberWinner(true);
            yield return new WaitForSeconds(0.5f);
            sc.FxNumberWinner(false);
            yield return new WaitForSeconds(0.5f);
            sc.FxNumberWinner(false);
            yield return new WaitForSeconds(0.5f);
            sc.FxNumberWinner(true);
            yield return new WaitForSeconds(0.5f);
            sc.FxNumberWinner(false);
            yield return new WaitForSeconds(0.5f);
            sc.FxNumberWinner(true);
            yield return new WaitForSeconds(0.5f);
            sc.FxNumberWinner(false);
            yield return new WaitForSeconds(0.5f);
            sc.FxNumberWinner(true);
            yield return new WaitForSeconds(0.5f);
            sc.FxNumberWinner(false);
            yield return new WaitForSeconds(0.5f);
            sc.FxNumberWinner(true);
            yield return new WaitForSeconds(0.5f);
            sc.FxNumberWinner(false);
            yield return new WaitForSeconds(0.5f);
        } */ 
    }
}
