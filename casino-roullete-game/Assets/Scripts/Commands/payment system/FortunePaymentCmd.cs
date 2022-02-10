using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using ViewModel;
using Controllers;
using Infrastructure;
using System;
using Managers;
using System.Threading.Tasks;

namespace Commands
{
    public class FortunePaymentCmd : ICommand
    {
        private readonly CharacterCmdFactory characterCmdFactory;
        private readonly CharacterTable characterTable;
        private RewardFortune rewardFortune;
        private int finalPosition;

        public FortunePaymentCmd(CharacterCmdFactory characterCmdFactory, CharacterTable characterTable, RewardFortune rewardFortune, int finalPosition)
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

            characterCmdFactory.SaveCash(characterCmdFactory, characterTable, payment).Execute();

            rewardFortune.isPlay = false;
            rewardFortune.isPayment = false;

            OpenScene(payment);
        }
        public async void OpenScene(int payment)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            GameManager.Instance.LoadScene("Game");
            characterTable.characterMoney.currentPayment.Value = payment;
            await Task.Delay(TimeSpan.FromSeconds(2));
            GameManager.Instance.ToggleGame();
        }
    }
}
