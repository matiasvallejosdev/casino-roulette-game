using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace ViewModel
{
    [CreateAssetMenu(fileName = "New Character Money", menuName = "Scriptable/Character Money")]
    public class CharacterMoney : ScriptableObject
    {
        public IntReactiveProperty currentBet;
        public IntReactiveProperty currentMoney;

        public int GetCashTotal() 
        {
            int c = currentMoney.Value;
            return c;
        }

        public void AddCash(int cashWinner)
        {
            int aux = currentMoney.Value;
            currentMoney.Value += cashWinner;
        }
        public void SubstractCash(int cashLost)
        {
            if(cashLost < 0) 
            {
                cashLost = cashLost * -1;
            }

            currentMoney.Value -= cashLost;

            if (currentMoney.Value < 0)
            {
                currentMoney.Value = 0;
            }
        }

        public void AddBet(int betSum)
        {
            int aux = currentBet.Value - betSum;
            currentBet.Value  += betSum;
        }
        public void SubstractBet(int betRest)
        {
            int aux = currentBet.Value ;
            currentBet.Value  -= betRest;
        }
        public bool CheckBetValue(int valueFicha)
        {
            bool aux = true;
            if (valueFicha <= currentMoney.Value  && valueFicha != 0)
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
    }
}
