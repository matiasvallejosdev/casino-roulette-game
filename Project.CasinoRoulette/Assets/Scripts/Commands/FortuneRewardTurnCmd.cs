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
    public class FortuneRewardTurnCmd : ICommand
    {
        private RewardFortune rewardFortune;

        public FortuneRewardTurnCmd(RewardFortune rewardFortune)
        {
            this.rewardFortune = rewardFortune;
        }

        public void Execute()
        {
            rewardFortune.OnFortune.OnNext(true);
            PlayerReward.Instance.OpenReward();
        }
    }
}
