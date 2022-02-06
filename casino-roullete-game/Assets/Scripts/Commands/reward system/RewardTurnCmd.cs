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
    public class RewardTurnCmd : ICommand
    {
        private string rewardScene;

        public RewardTurnCmd(string rewardScene)
        {
            this.rewardScene = rewardScene;
        }

        public void Execute()
        {
            PlayerSound.Instance.gameSound.OnSound.OnNext(PlayerSound.Instance.gameSound.audioReferences[5]);
            Debug.Log($"Opening game reward to get more money!");
            GameManager.Instance.ToggleRewardSystem();
            GameManager.Instance.LoadScene(rewardScene);
        }
    }
}
