using System;
using UnityEngine;
using ViewModel;

namespace Components
{
    public class RewardHandler : MonoBehaviour, IRewardTimer
    {
        public ulong LastChestOpen()
        {
            return ulong.Parse(PlayerPrefs.GetString("LastRewardOpen"));
        }
        public bool IsRewardReady(RewardFortune rewardFortune, float SecondsToWait)
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
        public string CalculateTimer(float secondsToWait)
        {
            // Set the timer
            ulong diff = ((ulong)DateTime.Now.Ticks - LastChestOpen());
            ulong m = diff / TimeSpan.TicksPerSecond;
            float secondsLeft = (float)(secondsToWait - m);
            string t = "";

            // Hours
            t += ((int)secondsLeft / 3600).ToString() + "h:";
            secondsLeft -= ((int)secondsLeft / 3600) * 3600;
            // Minutes
            t += ((int)secondsLeft / 60).ToString("00") + "m:";
            // Seconds
            t += (secondsLeft % 60).ToString("00") + "s";

            return t;
        }
    }
}
