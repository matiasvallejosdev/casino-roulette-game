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
    public class FortunePlayCmd : ICommand
    {
        private RewardFortune rewardFortune;

        public FortunePlayCmd(RewardFortune rewardFortune)
        {
            this.rewardFortune = rewardFortune;
        }

        public void Execute()
        {
            rewardFortune.OnFortune.OnNext(true);
            rewardFortune.OpenReward();
        }
    }
}
