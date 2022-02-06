using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Commands;
using UnityEngine;
using ViewModel;
using UniRx;
using Managers;
using System.Threading.Tasks;

namespace Components
{
    public class RewardController : MonoBehaviour
    {
        // Player Reward System
        // Control the states of reward fortune
        public RewardFortune rewardFortuneData;

        private float secondsToWait;
        private IRewardTimer _rewardTimer;

        void Awake()
        {
            _rewardTimer = GetComponent<RewardHandler>();
        }

        void Start()
        {
            RewardStart();
        }

        async void RewardStart()
        {
            await Task.Delay(TimeSpan.FromSeconds(2));

            secondsToWait = PlayerPrefs.GetFloat("SecondsToWaitReward");
            var lastChestOpen = ulong.Parse(PlayerPrefs.GetString("LastRewardOpen"));

            rewardFortuneData.isPlay = false;

            _rewardTimer.IsRewardReady(rewardFortuneData, secondsToWait);
        }

        void Update()
        {
            _rewardTimer.IsRewardReady(rewardFortuneData, secondsToWait);

            if (!rewardFortuneData.isRewardPossible.Value)
            {
                if (_rewardTimer.IsRewardReady(rewardFortuneData, secondsToWait))
                {
                    return;
                }
                rewardFortuneData.rewardTimer.Value = _rewardTimer.CalculateTimer(secondsToWait);
            }
        }
    }
}
