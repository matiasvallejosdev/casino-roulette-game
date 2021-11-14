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

namespace Controllers
{
    public class PlayerReward : Singlenton<PlayerReward>
    {
        // Player Reward System
        // Control the states of reward fortune

        public RewardFortune rewardFortune;
        private float SecondsToWait { get; set; }

        //[SerializeField] private Text rewardTimer = null;
        private ulong lastChestOpen;
        private bool _isReward = false;


        void Start()
        {
            DontDestroyOnLoad(gameObject);
            RewardStart();
        }

        async void RewardStart()
        {
            await Task.Delay(TimeSpan.FromSeconds(2));

            SecondsToWait = PlayerPrefs.GetFloat("SecondsToWaitReward");
            lastChestOpen = ulong.Parse(PlayerPrefs.GetString("LastRewardOpen"));

            rewardFortune.isPlay = false;

            IsRewardReady();
        }

        void Update()
        {
            IsRewardReady();

            if (!rewardFortune.isRewardPossible.Value)
            {
                if (IsRewardReady())
                {            
                    return;
                }
                // Set the timer
                ulong diff = ((ulong)DateTime.Now.Ticks - lastChestOpen);
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

        private bool IsRewardReady()
        {
            lastChestOpen = ulong.Parse(PlayerPrefs.GetString("LastRewardOpen"));

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

        public void OpenReward()
        {
            PlayerPrefs.SetString("LastRewardOpen", DateTime.Now.Ticks.ToString());
        }
    }
}
