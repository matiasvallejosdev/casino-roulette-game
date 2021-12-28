using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Components;
using System;

namespace ViewModel
{
    [CreateAssetMenu(fileName = "New Reward Fortune", menuName = "Scriptable/Reward Fortune")]
    public class RewardFortune : ScriptableObject
    {
        public float secondsToWaitReward;
        public string rewardLabel;
        public float totalAngle;
        public int sectionCount;
        public int[] payment;
        public bool isPlay;
        public bool isPayment;

        public StringReactiveProperty rewardTimer;
        public BoolReactiveProperty isRewardPossible;

        // Observables
        public ISubject<bool> OnFortune = new Subject<bool>();

        public void OpenReward()
        {
            PlayerPrefs.SetString("LastRewardOpen", DateTime.Now.Ticks.ToString());
        }
    }
}
