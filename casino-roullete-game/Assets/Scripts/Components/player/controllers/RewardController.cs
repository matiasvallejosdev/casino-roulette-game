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

        public RewardFortune rewardFortune;
        private float SecondsToWait { get; set; }

        void Start()
        {
            RewardStart();
        }

        async void RewardStart()
        {
            await Task.Delay(TimeSpan.FromSeconds(2));

            SecondsToWait = PlayerPrefs.GetFloat("SecondsToWaitReward");
            var lastChestOpen = ulong.Parse(PlayerPrefs.GetString("LastRewardOpen"));

            rewardFortune.isPlay = false;

            RewardHandler.IsRewardReady(rewardFortune, SecondsToWait);
        }

        void Update()
        {
            RewardHandler.IsRewardReady(rewardFortune, SecondsToWait);

            if (!rewardFortune.isRewardPossible.Value)
            {
                if (RewardHandler.IsRewardReady(rewardFortune, SecondsToWait))
                {            
                    return;
                }
                // Set the timer
                ulong diff = ((ulong)DateTime.Now.Ticks - RewardHandler.LastChestOpen());
                ulong m = diff / TimeSpan.TicksPerSecond;
                float secondsLeft = (float)(SecondsToWait - m);
                string t = "";

                // Hours
                t += ((int)secondsLeft / 3600).ToString() + "h:";
                secondsLeft -= ((int)secondsLeft / 3600) * 3600;
                // Minutes
                t += ((int)secondsLeft / 60).ToString("00") + "m:";
                // Seconds
                t += (secondsLeft % 60).ToString("00") + "s";

                rewardFortune.rewardTimer.Value = t;
            }
        }
    }

    public static class RewardHandler
    {
        public static ulong LastChestOpen()
        {
            return ulong.Parse(PlayerPrefs.GetString("LastRewardOpen"));
        }
        public static bool IsRewardReady(RewardFortune rewardFortune, float SecondsToWait)
        {
            var lastChestOpen = LastChestOpen();

            ulong diff = ((ulong)DateTime.Now.Ticks - lastChestOpen);
            ulong m = diff / TimeSpan.TicksPerSecond;

            float secondsLeft = (float)(SecondsToWait - m);

            if (secondsLeft < 0)
            {
                rewardFortune.rewardTimer.Value = rewardFortune.rewardLabel;
                rewardFortune.isRewardPossible.Value = true;
                return true;
            }
            else
            {
                rewardFortune.isRewardPossible.Value = false;
                return false;
            }
        }
    }
}
