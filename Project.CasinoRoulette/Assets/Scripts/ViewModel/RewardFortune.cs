using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Components;

namespace ViewModel
{
    [CreateAssetMenu(fileName = "New Reward Fortune", menuName = "Scriptable/Reward Fortune")]
    public class RewardFortune : ScriptableObject
    {
        [Header("Fortune")]
        public float totalAngle;
        public int sectionCount;
        public int[] payment;
        public bool isPlay;
        public bool isPayment;

        [Header("Reward")]
        public string rewardLabel;
        public StringReactiveProperty rewardTimer;
        public BoolReactiveProperty isRewardPossible;

        // Observables
        public ISubject<bool> OnFortune = new Subject<bool>();
    }
}
