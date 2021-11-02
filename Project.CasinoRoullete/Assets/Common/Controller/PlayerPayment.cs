using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViewModel;
using UniRx;
using Components;
using System.Linq;
using System;

namespace Controllers
{
    public class PlayerPayment : Singlenton<PlayerPayment>
    {
        // Payment Controller System
        // He actived when the roullete is finished and finded the number winner
        // Calculate the bet with the number winner and the equation of payment
        private int _payment;
        private int _number;
        private IEnumerable<ChipGame> _chipsWinner;
        private IEnumerable<ChipGame> _chipsLosted;
        
        public int PaymentValue
        {
            get {return _payment;}
            private set {_payment = value;}
        }
        
        void Start()
        {
            DontDestroyOnLoad(this);
        }

        public IObservable<Unit> PaymentSystem(CharacterTable characterTable)
        {
            Debug.Log($"Payment system is being executed in {characterTable.tableName}");
            _payment = 0;
            _number = characterTable.lastNumber;

            _chipsWinner = characterTable.currentTable.Where(chip => chip.HasNumber(_number));
            _chipsLosted = characterTable.currentTable.Where(chip => !chip.HasNumber(_number));

            int paymentWin = CalculateEarnedPayment();
            int paymentLost = GetPaymentBack(_chipsLosted.ToArray());
            int paymentChips = GetPaymentBack(_chipsWinner.ToArray());

            _payment = paymentWin - (paymentLost);
            _payment = _payment + paymentChips;

            return Observable.Return(Unit.Default)
                .Do(_ => Debug.Log($"Win: {paymentWin}, Lost: {paymentLost}, Chips: {paymentChips}"))
                .Do(_ => Debug.Log($"The roullete pay: {_payment}"));
        }

        // Calculate and return the all values of payment.
        private int CalculateEarnedPayment()
        {
            int earnedPayment = 0;

            IEnumerable<ChipGame> plenos =  _chipsWinner.Where(chip => chip.currentButton.isPleno);
            IEnumerable<ChipGame> middles =  _chipsWinner.Where(chip => !chip.currentButton.isPleno);

            int paymentPleno = GetPaymentChips(plenos.ToArray());
            int paymentMiddle = GetPaymentChips(middles.ToArray());

            earnedPayment = paymentPleno + paymentMiddle;
            return earnedPayment;
        }

        // Calculate returned payment of chips
        private int GetPaymentBack(ChipGame[] chips)
        {
            int total = 0;

            foreach(ChipGame chip in chips)
            {
                int value = chip.currentChipData.chipValue;
                total = total + value;
            }

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
