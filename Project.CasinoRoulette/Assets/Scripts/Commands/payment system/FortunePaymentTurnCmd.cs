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
    public class FortunePaymentTurnCmd : ICommand
    {
        private readonly CharacterCmdFactory characterCmdFactory;
        private readonly CharacterTable characterTable;
        private RewardFortune rewardFortune;
        private int finalPosition;

        public FortunePaymentTurnCmd(CharacterCmdFactory characterCmdFactory, CharacterTable characterTable, RewardFortune rewardFortune, int finalPosition)
        {
            this.characterCmdFactory = characterCmdFactory;
            this.characterTable = characterTable;
            this.rewardFortune = rewardFortune;
            this.finalPosition = finalPosition;
        }

        public void Execute()
        {
            int payment = (int)rewardFortune.payment[finalPosition];
            Debug.Log($"You win: ${payment}");
            characterTable.characterMoney.currentPayment.Value = payment;

            characterCmdFactory.SaveCash(characterCmdFactory, characterTable, payment).Execute();

            rewardFortune.isPlay = false;
            rewardFortune.isPayment = false;

            GameManager.Instance.LoadScene("Game");
            GameManager.Instance.ToggleGame();
        }
    }
}
