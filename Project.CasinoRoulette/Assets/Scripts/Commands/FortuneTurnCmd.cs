using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using ViewModel;
using Controllers;
using Infrastructure;
using System;
using Managers;

namespace Commands
{
    public class FortuneTurnCmd : ICommand
    {
        private RewardFortune rewardFortune;
        private int finalPosition;

        public FortuneTurnCmd(RewardFortune rewardFortune, int finalPosition)
        {
            this.rewardFortune = rewardFortune;
            this.finalPosition = finalPosition;
        }

        public void Execute()
        {
            Debug.Log("You win: $ " + rewardFortune.payment[finalPosition]);
            //Ui.Instance.turnWinOrLost("Excelent!", _payment[_payment.Length - 1].ToString(), true, _payment[_payment.Length - 1]);
            //moneySystem._cashNew = _payment[_payment.Length - 1];
            //moneySystem.SavePlayerCash();
        }
    }
}
