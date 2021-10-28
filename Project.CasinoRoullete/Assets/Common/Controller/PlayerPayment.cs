using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViewModel;
using UniRx;
using Components;
using System.Linq;

namespace Controllers
{
    public class PlayerPayment : Singlenton<PlayerPayment>
    {
        // Payment Controller System
        // He actived when the roullete is finished and finded the number winner
        // Calculate the bet with the number winner and the equation of payment

        private int _payment;
        private CharacterTable _characterTable;
        
        void Start()
        {
            DontDestroyOnLoad(this);
        }

        public void PaymentSystem(CharacterTable characterTable)
        {
            Debug.Log($"Payment system is being executed in {characterTable.tableName}");
            _payment = 0;
            _characterTable = characterTable;

            int paymentWin = CalculateEarnedPayment();
            Debug.Log($"Win: {paymentWin}");

            int paymentLost = CalculateLostPayment();
            Debug.Log($"Lost: {paymentLost}");

            int paymentChips = CalculateChipsBack();
            Debug.Log($"Chips: {paymentChips}");

            _payment = paymentWin - (paymentLost * - 1);
            _payment = _payment + paymentChips;

            Debug.Log($"The roullete pay: {_payment}");
        }

        // Calculate and return the all values of payment.
        private int CalculateEarnedPayment()
        {
            int earnedPayment = 0;

            ChipGame[] currentTable = _characterTable.currentTable.ToArray();

            IEnumerable<ChipGame> plenos =  from chip
                                            in currentTable
                                            where chip.currentButton.isPleno == true
                                            select chip;

            IEnumerable<ChipGame> middles =  from chip
                                            in currentTable
                                            where chip.currentButton.isPleno == false
                                            select chip;

            int paymentPleno = GetPaymentChips(plenos.ToArray());
            int paymentMiddle = GetPaymentChips(middles.ToArray());

            earnedPayment = paymentPleno + paymentMiddle;
            return earnedPayment;
        }
        private int CalculateLostPayment()
        {
            int totalLost = 0;
            /*
            foreach(var element in _fichasLosted)
            {
                //FichaDisplay scFichas = element.GetComponent<FichaDisplay>();
                //int value = scFichas.GetValueFicha();
                //totalLost = totalLost - value;
            }
            */
            return totalLost;
        }
        private int CalculateChipsBack()
        {
            int total = 0;
            /*
            foreach(var ficha in _fichasWinnerPlenos)
            {
                //FichaDisplay scFichas = ficha.GetComponent<FichaDisplay>();
                //int value = scFichas.GetValueFicha();
                //total = total + value;
            }
            foreach(var ficha in _fichasWinnerMedios)
            {
                //FichaDisplay scFichas = ficha.GetComponent<FichaDisplay>();
                //int value = scFichas.GetValueFicha();
                //total = total + value;
            }
            */
            return total;
        }
        
        // Calculate the payment of middle or pleno with equation
        private int GetPaymentChips(ChipGame[] chips)
        {
            int total = 0;
            foreach (ChipGame chip in chips)
            {
                int value = EquationRoullete.EquationPayment(chip.currentButton.buttonValue.Count(), chip.currentChipData.chipValue);
                total = total + value;
            }
            return total;
        }
    }
}
