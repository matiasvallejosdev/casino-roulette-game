using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Controllers;

namespace ViewModel
{
    [CreateAssetMenu(fileName = "New Character Money", menuName = "Scriptable/Character Money")]
    public class CharacterMoney : ScriptableObject
    {
        public IntReactiveProperty characterBet;
        public IntReactiveProperty characterMoney;
        public IntReactiveProperty currentPayment;

        public int GetCashTotal() 
        {
            int c = characterMoney.Value;
            return c;
        }

        void AddCash(int cashWinner)
        {
            int aux = characterMoney.Value;
            characterMoney.Value += cashWinner;
        }

        void SubstractCash(int cashLost)
        {
            if(cashLost < 0) 
            {
                cashLost = cashLost * -1;
            }

            characterMoney.Value -= cashLost;

            if (characterMoney.Value < 0)
            {
                characterMoney.Value = 0;
            }
        }

        void AddBet(int betSum)
        {
            int aux = characterBet.Value - betSum;
            characterBet.Value  += betSum;
        }
        void SubstractBet(int betRest)
        {
            int aux = characterBet.Value ;
            characterBet.Value  -= betRest;
        }

        public bool CheckBetValue(int valueFicha)
        {
            bool aux = true;
            if (valueFicha <= characterMoney.Value  && valueFicha != 0)
            {
                aux = true;
                SubstractCash(valueFicha);
                AddBet(valueFicha);
            }
            else
            {
                aux = false;
            }
            return aux;
        }
        public void DeleteChip(int valueFicha)
        {
            SubstractBet(valueFicha);
            AddCash(valueFicha);
        }
        public void RoundFinish(int payment)
        {
            Debug.Log($"Character player money is now being refresh with payment {payment}!");
            characterBet.Value = 0;

            if(payment > 0)
                AddCash(payment);
            
            if(payment < 0)
                SubstractCash(payment);
        }
    }
}
